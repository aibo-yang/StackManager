using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Communication.HTTP;
using Common.Toolkits.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Prism.Events;
using StackManager.Context.Domain;
using StackManager.Context.Event;
using StackManager.Context.MES;
using StackManager.Context.PLC;
using StackManager.Repositories;

#pragma warning disable 649

namespace StackManager.Workers
{
    class FlowlineWorker : BackgroundWorker
    {
        readonly ILogger<FlowlineWorker> logger;
        readonly IEventAggregator eventAggregator;
        readonly IUnitOfWork unitOfWork;
        readonly IMapper mapper;

        readonly IRepository<ProductCategory> productCategoryRepository;
        readonly IRepository<Box> boxRepository;
        readonly IRepository<Pallet> palletRepository;
        readonly IRepository<Flowline> flowlineRepository;
        readonly IRepository<Setting> settingRepository;
        
        Setting setting;

        string curlog = string.Empty;

        FlowlineRequest flowlineRequest = null;
        FlowlineResponse flowlineResponse = null;

        readonly BoxInfoRequest boxInfoRequest = new BoxInfoRequest();

        bool waitingBarcode = false;

        string curBarcode = string.Empty;

        public FlowlineWorker(ILogger<FlowlineWorker> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;

            this.productCategoryRepository = this.unitOfWork.GetRepository<ProductCategory>();
            this.boxRepository = this.unitOfWork.GetRepository<Box>();
            this.palletRepository = this.unitOfWork.GetRepository<Pallet>();
            this.flowlineRepository = this.unitOfWork.GetRepository<Flowline>();
            this.settingRepository = this.unitOfWork.GetRepository<Setting>();

            this.setting = this.settingRepository.TrackingQuery().SingleOrDefault();
            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                mapper.Map(x.Getter<Setting>(), this.setting);
            }, ThreadOption.BackgroundThread, true, x => x.EventType == EventType.ProfileChanged);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                if (waitingBarcode)
                { 
                    curBarcode = x.Getter<string>();
                }
            }, ThreadOption.BackgroundThread, true, x => x.EventType == EventType.InputScannerResponse);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                flowlineRequest = x.Getter<FlowlineRequest>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.FlowlineRequest);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                flowlineResponse = x.Getter<FlowlineResponse>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.FlowlineResponse);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var logList = new List<string>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var log = string.Join(" ", logList);
                if (log != curlog)
                {
                    curlog = log;
                    logger.LogInformation(curlog);
                    logList.Clear();
                }

                await Task.Delay(300);

                logList.Clear();
                logList.Add($"{nameof(FlowlineWorker)}:");
                logList.Add($"{flowlineRequest == null},{flowlineResponse == null}");

                if (flowlineRequest == null || flowlineResponse == null || setting == null)
                {
                    continue;
                }

                try
                {
                    unitOfWork.ClearDbContext();
                    
                    for (int i = 0; i < flowlineRequest.ScanRequests.Length; i++)
                    {
                        logList.Add($"{i}-{flowlineRequest.ScanRequests[i]}-{flowlineResponse.ScanResults[i]}");

                        if (flowlineRequest.ScanRequests[i] != 0 && flowlineResponse.ScanResults[i] == 0)
                        {
                            // 开始扫码
                            if (!await ScannerRequestAsync(i))
                            {
                                logger.LogError($"扫码枪[{i + 1}]，扫码失败");
                                flowlineResponse.ScanResults[i] = 2;
                                continue;
                            }

                            var barcode = curBarcode;
                            curBarcode = string.Empty;

                            if (flowlineRequest.ScanRequests[i] == 1)
                            {
                                // 请求MES
                                var mesContent = await MesRequestAsync(barcode);
                                BoxInfoResponse boxInfo = null;
                                try
                                {
                                    if (!string.IsNullOrEmpty(mesContent))
                                    {
                                        boxInfo = JsonConvert.DeserializeObject<BoxInfoResponse>(mesContent);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex, "转换MES结果失败");
                                }
                                
                                if (boxInfo == null || boxInfo.Result.ToUpper() != "OK")
                                {
                                    logger.LogError($"MES请求失败:{barcode}, {mesContent}");
                                    flowlineResponse.ScanResults[i] = 4;
                                    continue;
                                }

                                // 判断产品类型
                                var flowlines = await flowlineRepository.TrackingQuery()
                                    .Include(x=>x.Elevator)
                                    .Where(x=>x.Name  == boxInfo.Description.LineNo)
                                    .ToListAsync();

                                if (flowlines == null || flowlines.Count != 1)
                                {
                                    logger.LogError($"进站箱子条码与产品配置不符:{i}, {boxInfo.Description.LineNo}");
                                    flowlineResponse.ScanResults[i] = 8;
                                    continue;
                                }

                                var productCategory = await productCategoryRepository.TrackingQuery()
                                    .SingleOrDefaultAsync(x=>x.Name == boxInfo.Description.ProductName);

                                if (productCategory == null)
                                {
                                    logger.LogError($"箱子规格不存在:{i}, {boxInfo.Description.LineNo},{boxInfo.Description.ProductName}");
                                    flowlineResponse.ScanResults[i] = 16;
                                    continue;
                                }

                                // 保存数据
                                if (!await BoxEntryToCaching(barcode, boxInfo, flowlines.Single(), productCategory))
                                {
                                    logger.LogError($"箱子[{i + 1}]入站失败:{barcode}");
                                    continue;
                                }
                            }

                            flowlineResponse.ScanResults[i] = 1;
                        }
                        else if (flowlineRequest.ScanRequests[i] != 0 && flowlineResponse.ScanResults[i] != 0)
                        {
                            // 等待复位
                        }
                        else if (flowlineRequest.ScanRequests[i] == 0 && flowlineResponse.ScanResults[i] != 0)
                        {
                            // 自复位 
                            flowlineResponse.ScanResults[i] = 0;
                        }
                        else if (flowlineRequest.ScanRequests[i] == 0 && flowlineResponse.ScanResults[i] == 0)
                        {
                            // 空闲
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{nameof(FlowlineWorker)}");
                }
            }
        }

        async Task<bool> ScannerRequestAsync(int idx)
        {
            var ec = new EventContext { EventType = EventType.ScannerRequest };
            ec.Setter(idx);
            this.eventAggregator.GetEvent<EventHub>().Publish(ec);

            waitingBarcode = true;

            var retry = 7;
            do
            {
                await Task.Delay(300);
            } while ((string.IsNullOrEmpty(curBarcode) && retry-- > 0));

            waitingBarcode = false;
            return !string.IsNullOrEmpty(curBarcode);
        }

        async Task<string> MesRequestAsync(string barcode)
        {
#if !MOCK_ENABLE
            boxInfoRequest.RoutingData = barcode;
            var jsonReq = JsonConvert.SerializeObject(boxInfoRequest);
            var sign = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes($"{setting.MesSecret}{jsonReq}")).Select(x => x.ToString("X2")));

            var res = string.Empty;
            try
            {
                res = await HttpRequest.PostJsonAsync($"{setting.MesUri}?sign={sign}", jsonReq, new Dictionary<string, string>() { { "TokenID", setting.MesTokenId }, });
                logger.LogInformation($"RES: {res}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MES请求失败");
            }

            return res;
#else
            var s = barcode.Substring(0, 2).TrimStart('0');
            if (string.IsNullOrEmpty(s))
            {
                s = "0";
            }
            var idx = int.Parse(s);

            var flowline = await flowlineRepository.TrackingQuery()
                .SingleOrDefaultAsync(x => x.Index == idx);

            var boxes = await boxRepository.TrackingQuery()
                .Include(x => x.Flowline)
                .Where(x=>x.Flowline ==flowline)
                .OrderBy(x=>x.Code)
                .ToListAsync();

            var newPalletNo = string.Empty;

            if (boxes.Any())
            {
                if (boxes.Count < 30)
                {
                    newPalletNo = boxes.First().Code;
                }
                else if (boxes.Count == 30)
                {
                    newPalletNo = barcode;
                }
                else
                {
                    newPalletNo = boxes.OrderBy(x => x.Code).Skip(30).Take(1).First().Code;
                }
            }
            else
            {
                newPalletNo = barcode;
            }

            var res = new BoxInfoResponse
            {
                Result = "OK",
                Description = new BoxInfoDescription
                {
                    CartonQty = 30,
                    CartonIsFull = "Y",
                    PalletNo = $"P{newPalletNo}",
                    PalletIsFull = (boxes.Count + 1) == 30 ? "Y" : "N",
                    PalletCartonQty = 30,
                    LineNo = flowline.Name,
                    OrderNo = new List<string>() { $"M{barcode.Substring(0, 2)}" },
                    ProductName = "ADP-240EB BD",
                }
            };
            return JsonConvert.SerializeObject(res);
#endif
        }

        async Task<bool> BoxEntryToCaching(string barcode, BoxInfoResponse boxInfo, Flowline flowline, ProductCategory productCategory)
        {
            try
            {
                if (boxRepository.TrackingQuery().Any(x => x.Code == barcode && x.Status != BoxStatus.Caching))
                {
                    logger.LogError($"箱子码重复:{barcode}");
                    return false;
                }

                if (boxRepository.TrackingQuery().Any(x => x.Code == barcode && x.Status == BoxStatus.Caching))
                {
                    return true;
                }

                var box = new Box
                {
                    Code = barcode,
                    Status = BoxStatus.Caching,
                    BoxIsFull = boxInfo.Description.CartonIsFull.ToUpper() == "Y",
                    PalletIsFull = boxInfo.Description.PalletIsFull.ToUpper() == "Y",
                    PalletNo = boxInfo.Description.PalletNo,
                    OrderNo = boxInfo.Description.OrderNo.FirstOrDefault(),
                    ProdcutCount = boxInfo.Description.CartonQty,
                    BoxCount = boxInfo.Description.PalletCartonQty,
                    Flowline = flowline,
                    ProductCategory = productCategory,
                };

                await boxRepository.AddAsync(box);
                if (!await unitOfWork.SaveChangesAsync(async entry =>
                {
                    entry.Reload();
                    return await Task.FromResult(false);
                }))
                {
                    return false;
                }

                // PQM上报
                var ec = new EventContext
                {
                    EventType = EventType.ReportPQM,
                };
                ec.Setter(box);
                this.eventAggregator.GetEvent<EventHub>().Publish(ec);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }

            return true;
        }

        async Task<bool> BoxLeaveFromCachingAsync(string barcode)
        {
            var box = await boxRepository.TrackingQuery()
                .SingleOrDefaultAsync(x => x.Status == BoxStatus.Caching && x.Code == barcode);

            if (box == null)
            {
                logger.LogError($"箱子码不存在:{barcode}");
                return false;
            }

            box.Status = BoxStatus.Caching_Leave;
            box.SoftDeleted = true;

            if (!await unitOfWork.SaveChangesAsync(async entry =>
            {
                entry.Reload();
                return await Task.FromResult(false);
            }))
            {
                return false;
            }

            return true;
        }
    }
}

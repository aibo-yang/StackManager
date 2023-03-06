using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Common.Communication;
using Common.Toolkits.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prism.Events;
using StackManager.Context.Domain;
using StackManager.Context.Event;
using StackManager.Context.PLC;
using StackManager.Repositories;

namespace StackManager.Workers
{
    class StackingWorker : BackgroundWorker
    {
        readonly ILogger<StackingWorker> logger;
        readonly IEventAggregator eventAggregator;
        readonly IUnitOfWork unitOfWork;
        readonly IMapper mapper;

        readonly IRepository<Box> boxRepository;
        readonly IRepository<Pallet> palletRepository;
        readonly IRepository<Flowline> flowlineRepository;

        StackingRequest stackingRequest = null;
        StackingResponse stackingResponse = null;

        string curBarcode = string.Empty;
        string curlog = string.Empty;
        bool waitingBarcode = false;

        public StackingWorker(ILogger<StackingWorker> logger,
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;

            this.boxRepository = this.unitOfWork.GetRepository<Box>();
            this.palletRepository = this.unitOfWork.GetRepository<Pallet>();
            this.flowlineRepository = this.unitOfWork.GetRepository<Flowline>();

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                if (waitingBarcode)
                { 
                    curBarcode = x.Getter<string>();
                }
            }, ThreadOption.BackgroundThread, true, x => x.EventType == EventType.OutputScannerResponse);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                stackingRequest = x.Getter<StackingRequest>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.StackingRequest);

            this.eventAggregator.GetEvent<EventHub>().Subscribe(x =>
            {
                stackingResponse = x.Getter<StackingResponse>();
            }, ThreadOption.BackgroundThread, true, x => x.EventId == DataAddress.StackingResponse);
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
                logList.Add($"{nameof(StackingWorker)}:");
                logList.Add($"{stackingRequest == null},{stackingResponse == null}");

                if (stackingRequest == null || stackingResponse == null)
                {
                    continue;
                }

                logList.Add($"{stackingRequest.StowStartRequest}-{stackingResponse.StowStartResult.Done} {stackingResponse.StowStartResult.PalletRemove}-{stackingResponse.StowStartResult.PalletException}-{stackingResponse.StowStartResult.BoxType}-{stackingResponse.StowStartResult.BoxLayer}-{stackingResponse.StowStartResult.BoxRow}-{stackingResponse.StowStartResult.BoxCol}");

                try
                {
                    unitOfWork.ClearDbContext();

                    #region 初始化栈板
                    logList.Add($"{Environment.NewLine}");
                    for (int i = 0; i < stackingRequest.InitPalletRequests.Length; i++)
                    {
                        logList.Add($"{i}-{stackingRequest.InitPalletRequests[i]}-{stackingResponse.InitPalletResults[i]}");

                        if (stackingRequest.InitPalletRequests[i] == true && stackingResponse.InitPalletResults[i] == false)
                        {
                            var flowline = await flowlineRepository.TrackingQuery()
                                .Include(x => x.Pallets)
                                .ThenInclude(x => x.Boxes)
                                .SingleOrDefaultAsync(x => x.Index == i);

                            if (flowline.Pallets == null || flowline.Pallets.Count == 0)
                            {
                                stackingResponse.InitPalletResults[i] = true;
                            }
                            else
                            {
                                if (flowline.Pallets.Count != 1)
                                {
                                    logger.LogError($"初始化栈板[{i + 1}]数据不唯一");
                                    continue;
                                }

                                var pallet = flowline.Pallets.Single();
                                foreach (var palletBox in pallet.Boxes)
                                {
                                    palletBox.SoftDeleted = true;
                                }
                                pallet.SoftDeleted = true;
                                // pallet.Flowline = null;

                                if (!await unitOfWork.SaveChangesAsync(async entry =>
                                {
                                    entry.Reload();
                                    return await Task.FromResult(false);
                                }))
                                {
                                    logger.LogError($"更换栈板[{i + 1}]保存数据失败");
                                    continue;
                                }
                            }
                            stackingResponse.InitPalletResults[i] = true;
                        }
                        else if (stackingRequest.InitPalletRequests[i] == true && stackingResponse.InitPalletResults[i] == true)
                        {
                            // 等待复位
                        }
                        else if (stackingRequest.InitPalletRequests[i] == false && stackingResponse.InitPalletResults[i] == true)
                        {
                            // 自复位 
                            stackingResponse.InitPalletResults[i] = false;
                        }
                        else if (stackingRequest.InitPalletRequests[i] == false && stackingResponse.InitPalletResults[i] == false)
                        {
                            // 空闲
                        }
                    }
                    #endregion

                    #region 更换新栈板
                    logList.Add($"{Environment.NewLine}");
                    for (int i = 0; i < stackingRequest.NewPalletRequests.Length; i++)
                    {
                        logList.Add($"{i}-{stackingRequest.NewPalletRequests[i]}-{stackingResponse.NewPalletResults[i]}");

                        if (stackingRequest.NewPalletRequests[i] == true && stackingResponse.NewPalletResults[i] == false)
                        {
                            var flowline = await flowlineRepository.TrackingQuery()
                                .Include(x => x.Pallets)
                                .SingleOrDefaultAsync(x => x.Index == i);

                            if (flowline.Pallets.Any())
                            {
                                logger.LogError($"更换栈板[{i + 1}]异常");
                                continue;
                            }

                            var pallet = new Pallet
                            {
                                Name = flowline.Name,
                                Index = i,
                                Status = PalletStatus.Stacking,
                                Flowline = flowline,
                            };

                            await palletRepository.AddAsync(pallet);
                            if (!await unitOfWork.SaveChangesAsync(async entry =>
                            {
                                entry.Reload();
                                return await Task.FromResult(false);
                            }))
                            {
                                logger.LogError($"更换栈板[{i + 1}]保存数据失败");
                                continue;
                            }

                            stackingResponse.NewPalletResults[i] = true;
                        }
                        else if (stackingRequest.NewPalletRequests[i] == true && stackingResponse.NewPalletResults[i] == true)
                        {
                            // 等待复位
                        }
                        else if (stackingRequest.NewPalletRequests[i] == false && stackingResponse.NewPalletResults[i] == true)
                        {
                            // 自复位 
                            stackingResponse.NewPalletResults[i] = false;
                        }
                        else if (stackingRequest.NewPalletRequests[i] == false && stackingResponse.NewPalletResults[i] == false)
                        {
                            // 空闲
                        }
                    }

                    #endregion

                    #region 请求扫码并码垛
                    if (stackingRequest.StowStartRequest == 1 && stackingResponse.StowStartResult.Done == 0)
                    {
                        // 开始扫码
                        if (!await ScannerRequestAsync(4))
                        {
                            logger.LogError($"扫码枪[4]，扫码失败");
                            stackingResponse.StowStartResult.Done = 8;
                            continue;
                        }
                        var barcode = curBarcode;
                        curBarcode = string.Empty;

                        // 判断是否有箱子正在码垛
                        if (boxRepository.TrackingQuery().Any(x=>x.Status == BoxStatus.Stacking))
                        {
                            logger.LogError($"正在码垛的箱子未处理");
                            continue;
                        }

                        // 绑定栈板
                        var box = await boxRepository.TrackingQuery()
                            .Include(x => x.ProductCategory)
                            .Include(x => x.Pallet)
                            .ThenInclude(x => x.Boxes)
                            .Include(x => x.Flowline)
                            .ThenInclude(x => x.Pallets.Where(x => x.Status == PalletStatus.Stacking))
                            .ThenInclude(x => x.Boxes)
                            .SingleOrDefaultAsync(x => x.Code == barcode && x.Status == BoxStatus.Caching);

                        if (box == null)
                        {
                            stackingResponse.StowStartResult.BoxType = 0;
                            stackingResponse.StowStartResult.PalletIndex = 0;
                            stackingResponse.StowStartResult.BoxLayer = 0;
                            stackingResponse.StowStartResult.BoxRow = 0;
                            stackingResponse.StowStartResult.BoxCol = 0;
                            stackingResponse.StowStartResult.PalletRemove = 0;
                            stackingResponse.StowStartResult.PalletException = 0;
                            stackingResponse.StowStartResult.BoxHeight = 0;
                            stackingResponse.StowStartResult.BoxWidth = 0;
                            stackingResponse.StowStartResult.BoxLength = 0;
                            stackingResponse.PalletType = 0;
                            stackingResponse.LayoutType = 0;
                            stackingResponse.BoxBoard = 0;
                            stackingResponse.BoardResult = 0;
                            stackingResponse.StackType = 0;
                            stackingResponse.CacheRegion = 0;
                            stackingResponse.StowStartResult.Done = 64;

                            logger.LogInformation($"箱子条码[{barcode}]无信息");
                            continue;
                        }

                        if (box.Flowline == null)
                        {
                            logger.LogError($"流线数据异常: {nameof(box.Flowline)} {box.Flowline == null}");
                            continue;
                        }

                        if (box.Pallet == null)
                        {
                            if (box.Flowline.Pallets == null || box.Flowline.Pallets.Count != 1)
                            {
                                stackingResponse.StowStartResult.BoxType = box.ProductCategory.PLCCode;
                                stackingResponse.StowStartResult.PalletIndex = (ushort)(box.Flowline.Index + 1);
                                stackingResponse.StowStartResult.BoxLayer = 0;
                                stackingResponse.StowStartResult.BoxRow = 0;
                                stackingResponse.StowStartResult.BoxCol = 0;
                                stackingResponse.StowStartResult.PalletRemove = 0;
                                stackingResponse.StowStartResult.PalletException = 0;
                                stackingResponse.StowStartResult.BoxHeight = 0;
                                stackingResponse.StowStartResult.BoxWidth = 0;
                                stackingResponse.StowStartResult.BoxLength = 0;
                                stackingResponse.PalletType = 0;
                                stackingResponse.LayoutType = 0;
                                stackingResponse.BoxBoard = 0;
                                stackingResponse.BoardResult = 0;
                                stackingResponse.StackType = 0;
                                stackingResponse.CacheRegion = 0;
                                stackingResponse.StowStartResult.Done = 32; // bit0
                               
                                logger.LogInformation($"流线栈板数据异常: {nameof(box.Flowline.Pallets)} {box.Flowline.Pallets == null}, {box.Flowline.Pallets.Count != 1}");
                                continue;
                            }

                            var flowlinePallet = box.Flowline.Pallets.Single();
                            box.Pallet = flowlinePallet;

                            if (string.IsNullOrEmpty(flowlinePallet.Code))
                            {
                                // 新栈板
                                flowlinePallet.Code = box.PalletNo;
                            }
                            else
                            {
                                if (!box.PalletNo.Equals(flowlinePallet.Code))
                                {
                                    // 不是同一个栈板，需要拖走换新栈板
                                    stackingResponse.StowStartResult.BoxType = box.ProductCategory.PLCCode;
                                    stackingResponse.StowStartResult.PalletIndex = (ushort)(flowlinePallet.Index + 1);
                                    stackingResponse.StowStartResult.BoxLayer = 0;
                                    stackingResponse.StowStartResult.BoxRow = 0;
                                    stackingResponse.StowStartResult.BoxCol = 0;
                                    stackingResponse.StowStartResult.PalletRemove = (ushort)ByteUtil.SetBitAt(stackingResponse.StowStartResult.PalletRemove, flowlinePallet.Index, true);
                                    stackingResponse.StowStartResult.PalletException = (ushort)ByteUtil.SetBitAt(stackingResponse.StowStartResult.PalletException, flowlinePallet.Index, true);
                                    stackingResponse.StowStartResult.BoxHeight = (ushort)box.ProductCategory.BoxHeight;
                                    stackingResponse.StowStartResult.BoxWidth = (ushort)box.ProductCategory.BoxWidth;
                                    stackingResponse.StowStartResult.BoxLength = (ushort)box.ProductCategory.BoxLength;
                                    stackingResponse.PalletType = (ushort)box.ProductCategory.PalletType;
                                    stackingResponse.LayoutType = (ushort)box.ProductCategory.LayoutType;
                                    stackingResponse.BoxBoard = (ushort)box.ProductCategory.BoxBoard;
                                    stackingResponse.BoardResult = 0;
                                    stackingResponse.StackType = 0;
                                    stackingResponse.CacheRegion = (ushort)box.ProductCategory.CacheRegion;
                                    stackingResponse.StowStartResult.Done = 16; // bit0
                                    continue;
                                }
                            }
                        }

                        if (!box.PalletNo.Equals(box.Pallet.Code))
                        {
                            logger.LogError($"栈板编号异常: {box.PalletNo} {box.Pallet.Code}");
                            continue;
                        }

                        if (box.ProductCategory.StackType == 1)
                        {
                            box.Status = BoxStatus.Stacking;
                        }


                        // var isFull = box.PalletIsFull || box.Pallet.Boxes.Count + 1 == box.BoxCount;
                        // var isFull = box.Pallet.Boxes.Count + 1 == box.BoxCount;
                        var isFull = box.Pallet.Boxes.Count + 1 == box.ProductCategory.PalletBoxCount;

                        if (isFull)
                        {
                            if (box.Pallet.Boxes.Any(x => !x.BoxIsFull))
                            {
                                // 箱子产品是否放满
                                logger.LogError($"栈板异常：存在没有放满的箱子: {box.Code}");
                                box.Pallet.Status = PalletStatus.Stacked_NG;
                            }
                            else if (box.Pallet.Boxes.Count + 1 != box.BoxCount)
                            {
                                // 箱子数量是否一致
                                logger.LogError($"栈板异常：栈板箱子数与MES箱子数不一致: {box.Code}");
                                box.Pallet.Status = PalletStatus.Stacked_NG;
                            }
                            else
                            {
                                box.Pallet.Status = PalletStatus.Stacked_OK;
                            }
                        }

                      

                        // 回复信息 4 row 2 col  3  0 1 0
                        // 3 = 4 row 2 col 
                        var boxTotalCount = box.Pallet.Boxes.Count;
                        var layerBoxCount = box.ProductCategory.BoxRow * box.ProductCategory.BoxCol;
                        var pos = (boxTotalCount - 1) % layerBoxCount;  // 2

                        if (box.ProductCategory.StackType == 1)//正常
                        {
                            stackingResponse.StackType = 3;
                        }
                        else if (box.ProductCategory.StackType == 2)//缓存（偶数列）
                        {
                            var CacheResult = ((ushort)(pos / box.ProductCategory.BoxCol) + 1) % 2;
                            if (CacheResult == 0)
                            {
                                stackingResponse.StackType = 2;
                                box.Status = BoxStatus.StackCached; //进缓存码垛
                            }
                            else 
                            {
                                stackingResponse.StackType = 1;
                                box.Status = BoxStatus.NoStackCached;//进缓存不码垛
                            }
                        }
                        else if (box.ProductCategory.StackType == 3)//缓存（奇数列）
                        {
                            var CacheResult = ((ushort)(pos / box.ProductCategory.BoxCol) + 1) % 2;
                            var ColBoxCount = (ushort)(pos / box.ProductCategory.BoxCol) + 1;
                            if (ColBoxCount % box.ProductCategory.BoxCol != 0)
                            {

                                if (CacheResult == 0)
                                {
                                    stackingResponse.StackType = 2;
                                    box.Status = BoxStatus.StackCached; //进缓存码垛
                                }
                                else
                                {
                                    stackingResponse.StackType = 1;
                                    box.Status = BoxStatus.NoStackCached;//进缓存不码垛
                                }
                            }
                            else 
                            {
                                stackingResponse.StackType = 3;
                                box.Status = BoxStatus.Stacking;
                            }
                        }

                        stackingResponse.StowStartResult.BoxType = box.ProductCategory.PLCCode;
                        stackingResponse.StowStartResult.PalletIndex = (ushort)(box.Pallet.Index +1);
                        stackingResponse.StowStartResult.BoxLayer = (ushort)((boxTotalCount - 1) / layerBoxCount);
                        stackingResponse.StowStartResult.BoxRow = (ushort)(pos / box.ProductCategory.BoxCol); // 2 
                        stackingResponse.StowStartResult.BoxCol = (ushort)(pos % box.ProductCategory.BoxCol); // 4
                        stackingResponse.StowStartResult.PalletRemove = (ushort)ByteUtil.SetBitAt(stackingResponse.StowStartResult.PalletRemove, box.Pallet.Index, isFull ? true : false);
                        stackingResponse.StowStartResult.PalletException = (ushort)ByteUtil.SetBitAt(stackingResponse.StowStartResult.PalletException, box.Pallet.Index, box.Pallet.Status == PalletStatus.Stacked_NG ? true : false);
                        stackingResponse.StowStartResult.BoxHeight = (ushort)box.ProductCategory.BoxHeight;
                        stackingResponse.StowStartResult.BoxWidth = (ushort)box.ProductCategory.BoxWidth;
                        stackingResponse.StowStartResult.BoxLength = (ushort)box.ProductCategory.BoxLength;
                        stackingResponse.PalletType = (ushort)box.ProductCategory.PalletType;
                        stackingResponse.LayoutType = (ushort)box.ProductCategory.LayoutType;
                        stackingResponse.BoxBoard = (ushort)box.ProductCategory.BoxBoard;
                        stackingResponse.BoardResult = 0;
                        //stackingResponse.StackType = 0;
                        stackingResponse.CacheRegion = (ushort)box.ProductCategory.CacheRegion;
                        stackingResponse.StowStartResult.Done = 1;

                        // 保存数据
                        if (!await unitOfWork.SaveChangesAsync(async entry =>
                        {
                            entry.Reload();
                            return await Task.FromResult(false);
                        }))
                        {
                            logger.LogError($"保存堆栈箱子数据失败: {box.Code}");
                            continue;
                        }
                    }
                    else if (stackingRequest.StowStartRequest == 1 && stackingResponse.StowStartResult.Done != 0)
                    {
                        // 等待复位
                    }
                    else if (stackingRequest.StowStartRequest == 0 && stackingResponse.StowStartResult.Done != 0)
                    {
                        if (stackingResponse.StowStartResult.Done == 1)
                        {
                            if (stackingResponse.StackType == 3)
                            {
                                // 自复位 
                                var box = await boxRepository.TrackingQuery()
                                    .Include(x => x.Pallet)
                                    .ThenInclude(x => x.Flowline)
                                    .ThenInclude(x => x.Boxes)
                                    .Include(x => x.ProductCategory)
                                    .SingleOrDefaultAsync(x => x.Status == BoxStatus.Stacking);

                                if (box == null)
                                {
                                    logger.LogError($"不存在正在码垛的箱子");
                                    continue;
                                }
                                box.Status = BoxStatus.Stacked;

                                // var isFull = box.PalletIsFull || box.Pallet.Boxes.Count == box.BoxCount;
                                // var isFull = box.Pallet.Boxes.Count == box.BoxCount;
                                var isFull = box.Pallet.Boxes.Count == box.ProductCategory.PalletBoxCount;

                                if (isFull)
                                {
                                    foreach (var palletBox in box.Pallet.Boxes)
                                    {
                                        palletBox.SoftDeleted = true;
                                    }
                                    box.Pallet.SoftDeleted = true;
                                    // box.Pallet.Flowline = null;
                                }

                                if (!await unitOfWork.SaveChangesAsync(async entry =>
                                {
                                    entry.Reload();
                                    return await Task.FromResult(false);
                                }))
                                {
                                    logger.LogError($"更新码垛箱子异常: {box.Code}");
                                    continue;
                                }

                                // PQM上报
                                var ec = new EventContext
                                {
                                    EventType = EventType.ReportPQM,
                                };
                                ec.Setter(box);
                                this.eventAggregator.GetEvent<EventHub>().Publish(ec);
                            }
                            else if (stackingResponse.StackType == 2)
                            {
                                var boxes = await boxRepository.TrackingQuery()
                                    .Include(x => x.Pallet)
                                    .ThenInclude(x => x.Flowline)
                                    .ThenInclude(x => x.Boxes)
                                    .Include(x => x.ProductCategory).Where(x => x.ProductCategory.CacheRegion == stackingResponse.CacheRegion).ToListAsync();
                                var count = boxes.Count;
                                if (count == 2)
                                {
                                    foreach (var box in boxes)
                                    {
                                        box.Status = BoxStatus.Stacked;
                                    }
                                }
                                else 
                                {
                                    logger.LogError($"缓存区码垛箱数异常，请查看");
                                    continue;
                                }
                               
                            }
                            else if (stackingResponse.StackType == 1) 
                            {

                            }

                           

                           
                        }
                        else if (stackingResponse.StowStartResult.Done == 16)
                        {
                            var pallet = palletRepository.TrackingQuery()
                                .Include(x=>x.Flowline)
                                .Where(x => x.Status == PalletStatus.Stacking && x.Index == stackingResponse.StowStartResult.PalletIndex - 1)
                                .SingleOrDefault();

                            if (pallet == null)
                            {
                                logger.LogError($"不存在正在码垛的栈板");
                                continue;
                            }

                            foreach (var palletBox in pallet.Boxes)
                            {
                                palletBox.SoftDeleted = true;
                            }
                            pallet.SoftDeleted = true;
                            // pallet.Flowline = null;

                            if (!await unitOfWork.SaveChangesAsync(async entry =>
                            {
                                entry.Reload();
                                return await Task.FromResult(false);
                            }))
                            {
                                logger.LogError($"更新栈板数据异常: {pallet.Code}");
                                continue;
                            }
                        }

                        stackingResponse.StowStartResult.BoxType = 0;
                        stackingResponse.StowStartResult.BoxLayer = 0;
                        stackingResponse.StowStartResult.BoxRow = 0;
                        stackingResponse.StowStartResult.BoxCol = 0;
                        stackingResponse.StowStartResult.PalletRemove = 0;
                        stackingResponse.StowStartResult.PalletException = 0;
                        stackingResponse.StowStartResult.BoxHeight = 0;
                        stackingResponse.StowStartResult.BoxWidth = 0;
                        stackingResponse.StowStartResult.BoxLength = 0;
                        stackingResponse.StowStartResult.PalletIndex = 0;
                        stackingResponse.PalletType = 0;
                        stackingResponse.LayoutType = 0;
                        stackingResponse.BoxBoard = 0;
                        stackingResponse.BoardResult = 0;
                        stackingResponse.StackType = 0;
                        stackingResponse.CacheRegion = 0;
                        stackingResponse.StowStartResult.Done = 0;
                    }
                    else if (stackingRequest.StowStartRequest == 0 && stackingResponse.StowStartResult.Done == 0)
                    {
                        // 空闲
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{nameof(StackingWorker)}");
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
    }
}
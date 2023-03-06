using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Demo.Flowline
{
    public class Box
    {
        public string Name { get; set; }
        public double ProductRate { get; set; }
        public int BoxProductCount { get; set; }
        public int StackBoxCount { get; set; }
    }

    public class Stack
    {
        public string BoxName { get; set; }
        public int CurrentBoxCount { get; set; }
        public long CurrentClock { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var appConfig = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .Build();

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss_ffff");

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(appConfig)
                .WriteTo.File(path: $"Logs\\log{timestamp}.txt", outputTemplate: "{Message}{NewLine}", shared: true)
                .WriteTo.Console(outputTemplate: "{Message}{NewLine}")
                .CreateLogger();

            #region 初始化
            var stackLeaveInterval = 5 * 60;
            var robotMoveInterval = 21;

            var stackLeaveIntervalConfig = appConfig.GetSection("Box")["StackLeaveInterval"];
            if (!string.IsNullOrEmpty(stackLeaveIntervalConfig))
            {
                if (!int.TryParse(stackLeaveIntervalConfig, out stackLeaveInterval))
                {
                    stackLeaveInterval = 5 * 60;
                }
            }

            var robotMoveIntervalConfig = appConfig.GetSection("Box")["RobotMoveInterval"];
            if (!string.IsNullOrEmpty(robotMoveIntervalConfig))
            {
                if (!int.TryParse(robotMoveIntervalConfig, out robotMoveInterval))
                {
                    robotMoveInterval = 21;
                }
            }

            var baseClock = 0L;
            var totalBox = 0L;
            var maxBoxCount = 0;

            var boxCategories = new List<Box>()
            {
                new Box
                {
                    Name = "N21",
                    ProductRate = 8,
                    BoxProductCount = 32,
                    StackBoxCount = 30,
                },
                new Box
                {
                    Name = "N22",
                    ProductRate = 5,
                    BoxProductCount = 40,
                    StackBoxCount = 24,
                },
                new Box
                {
                    Name = "N23",
                    ProductRate = 6,
                    BoxProductCount = 64,
                    StackBoxCount = 24,
                },
                new Box
                {
                    Name = "N24",
                    ProductRate = 7,
                    BoxProductCount = 48,
                    StackBoxCount = 24,
                },
                new Box
                {
                    Name = "N25",
                    ProductRate = 9,
                    BoxProductCount = 32,
                    StackBoxCount = 24,
                },
                new Box
                {
                    Name = "N26",
                    ProductRate = 4.5,
                    BoxProductCount = 54,
                    StackBoxCount = 36,
                },
                new Box
                {
                    Name = "N27",
                    ProductRate = 4.5,
                    BoxProductCount = 64,
                    StackBoxCount = 24,
                },
                new Box
                {
                    Name = "N28",
                    ProductRate = 5,
                    BoxProductCount = 24,
                    StackBoxCount = 24,
                },
                new Box
                {
                    Name = "S9",
                    ProductRate = 6,
                    BoxProductCount = 54,
                    StackBoxCount = 30,
                },
                new Box
                {
                    Name = "S7",
                    ProductRate = 6,
                    BoxProductCount = 64,
                    StackBoxCount = 36,
                },
                new Box
                {
                    Name = "S5",
                    ProductRate = 6,
                    BoxProductCount = 64,
                    StackBoxCount = 36,
                },
                new Box
                {
                    Name = "S3",
                    ProductRate = 6,
                    BoxProductCount = 64,
                    StackBoxCount = 36,
                },
            };
            var stacks = new List<Stack> { new Stack(), new Stack(), new Stack(), new Stack(), new Stack(), new Stack(), new Stack(), new Stack() };
            var flowBoxes = new List<Box>();
            #endregion

            while (true)
            {
                //Task.Delay(0).Wait();
                baseClock++;

                #region 箱子进入流线
                foreach (var category in boxCategories)
                {
                    if (baseClock % (int)(category.ProductRate * category.BoxProductCount) == 0)
                    {
                        flowBoxes.Add(new Box
                        {
                            Name = category.Name,
                            ProductRate = category.ProductRate,
                            BoxProductCount = category.BoxProductCount,
                            StackBoxCount = category.StackBoxCount,
                        });
                    }
                }
                #endregion

                #region 箱子放入栈板
                foreach (var stack in stacks)
                {
                    if (baseClock % robotMoveInterval == 0)
                    {
                        if (!string.IsNullOrEmpty(stack.BoxName))
                        {
                            if (stack.CurrentBoxCount == boxCategories.Single(x => x.Name == stack.BoxName).StackBoxCount)
                            {
                                continue;
                            }

                            var box = flowBoxes.FirstOrDefault(x => x.Name == stack.BoxName);
                            if (box != null)
                            {
                                flowBoxes.Remove(box);
                                stack.CurrentBoxCount++;
                                if (stack.CurrentBoxCount == boxCategories.Single(x => x.Name == stack.BoxName).StackBoxCount)
                                {
                                    stack.CurrentClock = baseClock;
                                }
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region 栈板分配及拿走
                foreach (var stack in stacks)
                {
                    var productName = "";
                    if (string.IsNullOrEmpty(stack.BoxName))
                    {
                        //var stackBoxNames = stacks.Where(x => !string.IsNullOrEmpty(x.ProductName)).Select(x => x.ProductName).ToList();

                        //productName = flowline.Where(x => !stackBoxNames.Contains(x.Name))
                        //    .GroupBy(x => x.Name)
                        //    .Select(x => new { Key = x.Key, Count = x.Count() })
                        //    .OrderByDescending(x => x.Count).FirstOrDefault()?.Key;

                        //if (string.IsNullOrEmpty(productName))
                        //{

                        var boxes = flowBoxes.GroupBy(x => x.Name)
                            .Select(x => new { Key = x.Key, Count = x.Count() })
                            .OrderByDescending(x => x.Count).ToList();

                        foreach (var box in boxes)
                        {
                            var category = boxCategories.Single(x => x.Name == box.Key);
                            var existStacks = stacks.Where(x => x.BoxName == box.Key);
                            if (((int)Math.Ceiling(box.Count * 1.0 / category.StackBoxCount)) > existStacks.Count())
                            {
                                productName = box.Key;
                                break;
                            }
                        }

                        //}

                        if (string.IsNullOrEmpty(productName))
                        {
                            // 流线上没有箱子了
                            break;
                        }

                        stack.BoxName = productName;
                    }
                    else
                    {
                        if (stack.CurrentBoxCount == boxCategories.Single(x => x.Name == stack.BoxName).StackBoxCount)
                        {
                            if (baseClock - stack.CurrentClock >= stackLeaveInterval)
                            {
                                stack.BoxName = "";
                                stack.CurrentClock = 0;
                                stack.CurrentBoxCount = 0;
                            }
                            continue;
                        }
                    }
                }
                #endregion

                #region 打印信息
                var sortedFlowBoxes = flowBoxes.GroupBy(x => x.Name).Select(x => new { Key = x.Key, Count = x.Count() });

                if (flowBoxes.Count() > maxBoxCount)
                {
                    maxBoxCount = flowBoxes.Count();
                }

                totalBox += flowBoxes.Count();

                var msg = $"[CLK {baseClock},";
                msg += $"ROT {robotMoveInterval},";
                msg += $"STK {stackLeaveInterval}]";
                msg += $"\tMAX {maxBoxCount},";
                msg += $"AVG {totalBox / baseClock},";
                msg += $"CUR {flowBoxes.Count()}";

                msg += $"\tSTACK=> {string.Join(",", stacks.Select(x => $"{x.BoxName} {x.CurrentBoxCount}"))}";
                msg += $"\tLINES=> {string.Join(",", sortedFlowBoxes.Select(x => $"{x.Key} {x.Count}"))}";

                Log.Information(msg);

                #endregion
            }
        }
    }
}

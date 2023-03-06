using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Common.Toolkits.Extensions;
using Common.Toolkits.Workers;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Prism.DryIoc;
using Prism.Ioc;
using StackManager.Context;
using StackManager.Exceptions;
using StackManager.Extensions;
using StackManager.Repositories;
using StackManager.ViewModels;
using StackManager.Views;
using StackManager.Workers;

namespace StackManager
{
    public partial class App : PrismApplication
    {
        private Mutex instanceMutex = null;
        private bool instanceCreated;
        private CancellationTokenSource stopCts;
        private WindowExceptionHandler handler;
        private IConfiguration configuration;

        protected override IContainerExtension CreateContainerExtension()
        {
            // 单例运行
            instanceMutex = new Mutex(true, @"Global\Client.StackManager", out instanceCreated);
            if (!instanceCreated)
            {
                Environment.Exit(1);
                return null;
            }

            handler = new WindowExceptionHandler();
            stopCts = new CancellationTokenSource();
            
            var serviceCollection = new ServiceCollection();

            // 配置文件
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
                .Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);

            // 日志
            serviceCollection.AddLogging(cfg =>
            {
                cfg.ClearProviders();
                cfg.SetMinimumLevel(LogLevel.Information);
                cfg.AddNLog();
            });

            // AutoMapper
            serviceCollection.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new AutoMapperExtension());
            });

            // MySQL
            serviceCollection.AddDbContext<AppDbContext>(opt =>
            {
                var defaultConnection = configuration.GetSection("Database").GetConnectionString("DefaultConnection");
                opt.UseMySql(defaultConnection, ServerVersion.AutoDetect(defaultConnection), opt =>
                {
                    opt.CommandTimeout(60 * 10);
                    opt.MaxBatchSize(1024 * 2);
                    opt.EnableRetryOnFailure();
                });
            }, ServiceLifetime.Transient, ServiceLifetime.Singleton);

            // UnitOfWork
            serviceCollection.AddTransient<IUnitOfWork, UnitOfWork<AppDbContext>>();

            // Workers
            //serviceCollection.AddHostedWorker<TimerUpdateWorker>();

            return new DryIocContainerExtension(new Container(CreateContainerRules())
                .WithDependencyInjectionAdapter(serviceCollection));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册任务
            containerRegistry.RegisterSingleton<IHostedWorker, TimerUpdateWorker>();
            containerRegistry.RegisterSingleton<IHostedWorker, BarcodeScannerWorker>();
#if !MOCK_ENABLE
            containerRegistry.RegisterSingleton<IHostedWorker, FlowlineCommunicationWorker>();
            containerRegistry.RegisterSingleton<IHostedWorker, StackingCommunicationWorker>();
#else
            containerRegistry.RegisterSingleton<IHostedWorker, MockPLCRequestWorker>();
#endif
            containerRegistry.RegisterSingleton<IHostedWorker, DeviceStatusWorker>();
            containerRegistry.RegisterSingleton<IHostedWorker, FlowlineWorker>();
            containerRegistry.RegisterSingleton<IHostedWorker, StackingWorker>();

            containerRegistry.RegisterSingleton<IHostedWorker, UpdateViewWorker>();

            // 注册对话框
            containerRegistry.RegisterForNavigation<MessageOkCancelView, MessageOkCancelViewModel>();
            containerRegistry.RegisterForNavigation<ProfileEditView, ProfileEditViewModel>();
            containerRegistry.RegisterForNavigation<ProductEditView, ProductEditViewModel>();
            containerRegistry.RegisterForNavigation<FlowlineEditView, FlowlineEditViewModel>();
            containerRegistry.RegisterForNavigation<DeviceEditView, DeviceEditViewModel>();
            containerRegistry.RegisterForNavigation<SalveDeviceView, SlaveDeviceEditModel>();
            containerRegistry.RegisterForNavigation<CacheEditView, CacheEditViewModel>();
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
            containerRegistry.RegisterForNavigation<ChangePasswordView, ChangePasswordViewModel>();
            // 注册导航
        }

        protected override Window CreateShell()
        {
            CreateDbIfNotExist();
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Container.Resolve<KeyValuePair<object, IHostedWorker>[]>().ForEach(kv =>
            {
                if (kv.Value is IHostedWorker worker)
                {
                    Task.Run(async() =>
                    {
                        await worker.StartAsync(stopCts.Token);
                    });
                }
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // 释放单例
            if (instanceCreated)
            {
                instanceMutex.ReleaseMutex();
            }
            instanceMutex?.Close();

            stopCts.Cancel();
            Container.Resolve<KeyValuePair<object, IHostedWorker>[]>().ForEach(async kv =>
            {
                if (kv.Value is IHostedWorker worker)
                {
                    await worker.StopAsync(stopCts.Token);
                }
            });

            base.OnExit(e);
        }

        private void CreateDbIfNotExist()
        {
            try
            {
                var context = Container.Resolve<AppDbContext>();
                var config = Container.Resolve<IConfiguration>();

                if (config.GetSection("Database")["EnsureDeletedBeforeCreatedWithSeedData"] == "true")
                {
                    context.Database.EnsureDeleted();
                }

                if (!context.Database.CanConnect())
                {
                    context.Database.EnsureCreated();
                    AppDataBuilder.SeedData(context);
                }
            }
            catch (Exception ex)
            {
                var logger = Container.Resolve<ILogger<App>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }
    }
}

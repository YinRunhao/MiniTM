using Microsoft.Extensions.DependencyInjection;
using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MiniTM.Redis;

namespace MiniTM.Demo
{
    internal class RedisExample : NormalExample
    {
        public async Task Run()
        {
            ServiceCollection services = new ServiceCollection();
            // 配置依赖注入
            // 业务逻辑类型
            services.AddTransient<HelloJobBo>();
            services.AddTransient<HiJobBo>();
            // 业务逻辑工厂
            services.AddSingleton<IJobBoFactory, DIJobFactory>();
            // 任务管理器
            services.AddSingleton<ITaskManager>(sp =>
            {
                var factory = sp.GetService<IJobBoFactory>();
                // 普通执行器
                var ret = TaskManagerFactory.CreateDefaultManager()
                    .UseRedis((cfg) =>
                    {
                        cfg.ConnectionString = "127.0.0.1:6379,defaultDatabase=0";
                        cfg.CancelChannel = "MiniTM:cancel";
                        cfg.ProgressKeepTime = TimeSpan.FromMinutes(10);
                        cfg.Prefix = "MiniTM";
                    })
                    .UseJobBoFactory(factory);

                return ret;
            });

            var provider = services.BuildServiceProvider();

            // 开始测试
            var mng = provider.GetService<ITaskManager>();

            string input = string.Empty;
            TaskProgressDto progress = default;
            // 一个任务单个工作项
            //var taskIdArr = await AddSingleJobTask(mng);
            // 一个任务多个工作项
            var taskIdArr = await AddMultiJobTask(mng);
            Console.WriteLine("Add Task Success Press Enter to Search Progress, Enter 'exit' to close");
            while (true)
            {
                input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }

                foreach (var taskId in taskIdArr)
                {
                    progress = await mng.GetProgressAsync(taskId);
                    PrintProgress(taskId, progress);
                }
            }
            provider.Dispose();
        }
    }
}

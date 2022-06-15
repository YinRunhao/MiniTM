using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Demo
{
    /// <summary>
    /// 使用简单示例工厂的例子
    /// </summary>
    public class SimpleNormalExample : NormalExample
    {
        public async Task Run()
        {
            SimpleJobFactory factory = new SimpleJobFactory();
            // 普通执行器
            //var mng = TaskManagerFactory.CreateDefaultManager().UseJobBoFactory(factory);

            // 线程池执行器
            var mng = TaskManagerFactory.CreateDefaultManager()
                .UseThreadPoolExecutor((cfg) =>
                {
                    cfg.MaxThreads = 3;         // 最大线程数
                    cfg.TaskQueueLength = 1024; // 队列长度
                })
                .UseJobBoFactory(factory);

            string input = string.Empty;
            TaskProgressDto progress = default;
            // 一个任务单个工作项
            var taskIdArr = await AddSingleJobTask(mng);
            // 一个任务多个工作项
            //var taskIdArr = await AddMultiJobTask(mng);
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
            mng.Dispose();
        }
    }
}

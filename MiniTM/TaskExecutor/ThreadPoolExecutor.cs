using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniTM.TaskExecutor
{
    /// <summary>
    /// 线程池执行者
    /// </summary>
    public class ThreadPoolExecutor : ITaskExecutor
    {
        /// <summary>
        /// 工作线程池
        /// </summary>
        private WorkingThreadPool m_ThreadPool;

        public ThreadPoolExecutor(ThreadPoolConfig config)
        {
            WorkingThreadPool.Config(config.MaxThreads, config.TaskQueueLength);
            m_ThreadPool = WorkingThreadPool.GetInstance();
        }

        public void Dispose()
        {
            m_ThreadPool?.Dispose();
            m_ThreadPool = null;
        }

        public async Task ExecuteAsync(TaskItem task)
        {
            byte counter = 0;       // 入队失败次数
            int idx = 0;            // 下标
            JobItem job = default;
            for (idx = 0; idx < task.JobList.Count; idx++)
            {
                job = task.JobList[idx];
                try
                {
                    // 入队
                    m_ThreadPool.JobEnqueue(job);
                    counter = 0;
                }
                catch (SemaphoreFullException)
                {
                    // 任务队列满载
                    counter++;
                    // 暂停0.2 sec 并重试3次
                    if (counter >= 3)
                    {
                        break;
                    }
                    await Task.Delay(200);
                    idx--;
                }
                catch (Exception ex)
                {
                    task.Progress.AddNgRecord(ex.Message);
                }
            }

            if (counter >= 3)
            {
                for(;idx < task.JobList.Count; idx++)
                {
                    task.Progress.AddNgRecord("Queue of thread pool is full");
                }
            }
        }
    }
}

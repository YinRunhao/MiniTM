using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.TaskExecutor
{
    /// <summary>
    /// 线程池配置
    /// </summary>
    public class ThreadPoolConfig
    {
        /// <summary>
        /// 最大线程数
        /// </summary>
        public byte MaxThreads { get; set; }

        /// <summary>
        /// 任务队列长度
        /// </summary>
        public ushort TaskQueueLength { get; set; }

        /// <summary>
        /// 默认线程池配置为1个线程1024最大队列长度
        /// </summary>
        public ThreadPoolConfig()
        {
            MaxThreads = 1;
            TaskQueueLength = 1024;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Core
{
    /// <summary>
    /// 任务执行者
    /// </summary>
    public interface ITaskExecutor : IDisposable
    {
        /// <summary>
        /// 任务开始执行
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task ExecuteAsync(TaskItem task);
    }
}

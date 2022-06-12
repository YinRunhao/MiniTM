using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Core
{
    /// <summary>
    /// 任务取消通知接口
    /// </summary>
    public interface ITaskCancelNotifier : IDisposable
    {
        /// <summary>
        /// 发送任务取消通知
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task TaskCancelNotifyAsync(string taskId);

        /// <summary>
        /// 设置接收到任务取消后的回调
        /// </summary>
        /// <param name="callback"></param>
        void SetHandleCallback(Action<string> callback);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MiniTM.Core
{
    /// <summary>
    /// 任务项
    /// </summary>
    /// <remarks>一个任务项可以包含多个工作项</remarks>
    public class TaskItem
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// 任务进度
        /// </summary>
        public TaskProgress Progress { get; set; }

        /// <summary>
        /// 工作项集合
        /// </summary>
        public List<JobItem> JobList { get; set; }

        /// <summary>
        /// 取消标记
        /// </summary>
        public CancellationTokenSource CancelToken { get; private set; }

        /// <summary>
        /// 工作项开始前回调
        /// </summary>
        public Action<JobItem> OnJobBegining { get; set; }

        /// <summary>
        /// 工作项完成后回调
        /// </summary>
        public Action<JobItem, ExecResult> OnJobFinished { get; set; }

        public TaskItem(CancellationTokenSource cancelToken)
        {
            CancelToken = cancelToken;
        }
    }
}

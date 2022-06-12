using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Core
{
    /// <summary>
    /// 工作项
    /// </summary>
    public class JobItem
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// 执行Bo
        /// </summary>
        public IJobBo ExecBo { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public JobParams Parameters { get; set; }

        /// <summary>
        /// 来源任务项
        /// </summary>
        public TaskItem SrcTask { get; private set; }

        public JobItem(TaskItem srcTask)
        {
            SrcTask = srcTask;
        }
    }
}

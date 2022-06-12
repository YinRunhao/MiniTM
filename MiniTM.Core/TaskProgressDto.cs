using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Core
{
    /// <summary>
    /// 任务进度传输对象
    /// </summary>
    public class TaskProgressDto
    {
        /// <summary>
        /// 工作项总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 成功数
        /// </summary>
        public int Ok { get; set; }

        /// <summary>
        /// 失败数
        /// </summary>
        public int Ng { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public List<string> ErrMsg { get; set; }
    }
}

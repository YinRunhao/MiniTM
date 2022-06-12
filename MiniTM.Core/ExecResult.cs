using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Core
{
    /// <summary>
    /// 执行结果
    /// </summary>
    public class ExecResult
    {
        /// <summary>
        /// 成功标识
        /// </summary>
        public bool Ok { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string Msg { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Core
{
    /// <summary>
    /// 工作项
    /// </summary>
    public interface IJobBo
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="param">工作项参数</param>
        /// <returns>执行结果</returns>
        Task<ExecResult> ExecuteAsync(JobParams param);
    }
}

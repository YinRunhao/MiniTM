using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Core
{
    /// <summary>
    /// 不允许重复的工作项
    /// </summary>
    public interface IDistinctJobBo : IJobBo
    {
        /// <summary>
        /// 获取同种工作项的唯一标识
        /// </summary>
        /// <remarks>用于工作项去重时记录重复的工作项</remarks>
        /// <param name="param">工作项参数</param>
        /// <returns>同种工作项的唯一标识</returns>
        string GetDistinctString(JobParams param);
    }
}

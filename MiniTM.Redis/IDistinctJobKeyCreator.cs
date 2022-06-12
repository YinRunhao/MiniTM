using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Redis
{
    /// <summary>
    /// 不可重复的工作项集合键值生成器
    /// </summary>
    /// <remarks>每个不可重复的工作类型需要在Redis中维持一个List用于区分</remarks>
    public interface IDistinctJobKeyCreator
    {
        /// <summary>
        /// 获取不可重复的工作项集合键值
        /// </summary>
        /// <param name="prefix">Redis键值前缀</param>
        /// <param name="jobTp">工作项类型</param>
        /// <returns></returns>
        public string GetKey(string prefix, Type jobTp);
    }
}

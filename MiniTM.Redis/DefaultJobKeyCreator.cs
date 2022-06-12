using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Redis
{
    /// <summary>
    /// 默认的工作项集合键值生成器
    /// </summary>
    internal class DefaultJobKeyCreator : IDistinctJobKeyCreator
    {
        public string GetKey(string prefix, Type jobTp)
        {
            return prefix + ":job:" + jobTp.FullName;
        }
    }
}

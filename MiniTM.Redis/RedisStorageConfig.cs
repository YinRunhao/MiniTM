using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Redis
{
    /// <summary>
    /// Redis数据存储配置
    /// </summary>
    /// <remarks>进度保存时间默认1h</remarks>
    public class RedisStorageConfig
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 默认库
        /// </summary>
        public int DefDb { get; set; }

        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 进度保存时间
        /// </summary>
        public TimeSpan ProgressKeepTime { get; set; }

        /// <summary>
        /// 不可重复的工作项集合键值生成器
        /// </summary>
        /// <remarks>默认使用类全名拼接前缀作为Key值</remarks>
        public IDistinctJobKeyCreator JobKeyCreator { get; set; }

        public RedisStorageConfig()
        {
            ProgressKeepTime = TimeSpan.FromHours(1);
            JobKeyCreator = new DefaultJobKeyCreator();
        }
    }
}

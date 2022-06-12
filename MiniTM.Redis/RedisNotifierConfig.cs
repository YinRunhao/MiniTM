using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Redis
{
    /// <summary>
    /// Redis通知器配置
    /// </summary>
    public class RedisNotifierConfig
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 任务取消监听通道
        /// </summary>
        public string CancelChannel { get; set; }
    }
}

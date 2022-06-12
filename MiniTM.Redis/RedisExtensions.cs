using System;
using System.Collections.Generic;
using System.Text;
using MiniTM.Core;

namespace MiniTM.Redis
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class RedisExtensions
    {
        /// <summary>
        /// 使用Redis作为数据存储和任务取消通知器
        /// </summary>
        /// <param name="mng"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static TaskManager UseRedis(this TaskManager mng, Action<RedisConfig> act)
        {
            RedisConfig cfg = new RedisConfig();
            act(cfg);

            RedisConnection conn = new RedisConnection(cfg.ConnectionString);
            RedisStorageConfig storageConfig = new RedisStorageConfig
            {
                ConnectionString = cfg.ConnectionString,
                DefDb = cfg.DefDb,
                Prefix = cfg.Prefix,
                ProgressKeepTime = cfg.ProgressKeepTime,
                JobKeyCreator = cfg.JobKeyCreator
            };
            // 使用同一个Redis连接
            RedisStorage storage = new RedisStorage(conn, storageConfig);
            RedisNotifier notifier = new RedisNotifier(conn, cfg.CancelChannel);

            mng.UseDataStorage(storage).UseCancelNotifier(notifier);
            return mng;
        }

        /// <summary>
        /// 使用Redis作为数据存储
        /// </summary>
        /// <param name="mng"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static TaskManager UseRedisStorage(this TaskManager mng, Action<RedisStorageConfig> act)
        {
            RedisStorageConfig cfg = new RedisStorageConfig();
            act(cfg);

            RedisStorage storage = new RedisStorage(cfg);
            mng.UseDataStorage(storage);
            return mng;
        }

        /// <summary>
        /// 使用Redis的发布订阅功能作为任务取消通知器
        /// </summary>
        /// <param name="mng"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static TaskManager UseRedisNotifier(this TaskManager mng, Action<RedisNotifierConfig> act)
        {
            RedisNotifierConfig cfg = new RedisNotifierConfig();
            act(cfg);

            RedisNotifier notifier = new RedisNotifier(cfg);
            mng.UseCancelNotifier(notifier);
            return mng;
        }
    }
}

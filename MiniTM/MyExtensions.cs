using MiniTM.Core;
using MiniTM.DataStorage;
using MiniTM.TaskExecutor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM
{
    public static class MyExtensions
    {
        /// <summary>
        /// 使用线程池作为执行器
        /// </summary>
        /// <param name="mng"></param>
        /// <param name="act">配置</param>
        /// <returns></returns>
        public static TaskManager UseThreadPoolExecutor(this TaskManager mng, Action<ThreadPoolConfig> act)
        {
            ThreadPoolConfig cfg = new ThreadPoolConfig();
            act(cfg);
            var executor = new ThreadPoolExecutor(cfg);
            mng.UseExecutor(executor);
            return mng;
        }

        /// <summary>
        /// 使用普通执行器
        /// </summary>
        /// <param name="mng"></param>
        /// <returns></returns>
        public static TaskManager UseNormalExecutor(this TaskManager mng)
        {
            var executor = new NormalExecutor();
            mng.UseExecutor(executor);
            return mng;
        }

        /// <summary>
        /// 使用本地内存作为数据存储
        /// </summary>
        /// <param name="mng"></param>
        /// <param name="cleanInterval">进度清理间隔</param>
        /// <returns></returns>
        public static TaskManager UseLocalStorage(this TaskManager mng, TimeSpan cleanInterval)
        {
            var storage = new LocalStorage(cleanInterval);
            mng.UseDataStorage(storage);
            return mng;
        }

        /// <summary>
        /// 使用本地内存作为数据存储，默认清理间隔1小时
        /// </summary>
        /// <param name="mng"></param>
        /// <returns></returns>
        public static TaskManager UseLocalStorage(this TaskManager mng)
        {
            var storage = new LocalStorage();
            mng.UseDataStorage(storage);
            return mng;
        }
    }
}

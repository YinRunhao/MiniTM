using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM
{
    /// <summary>
    /// 提供创建任务管理器方法的静态类
    /// </summary>
    public static class TaskManagerFactory
    {
        /// <summary>
        /// 创建默认配置的任务管理器
        /// </summary>
        /// <remarks>使用普通执行器，本地内存作为数据存储</remarks>
        /// <returns></returns>
        public static TaskManager CreateDefaultManager()
        {
            TaskManager mng = new TaskManager();
            mng.UseNormalExecutor()
                .UseLocalStorage();
            return mng;
        }
    }
}

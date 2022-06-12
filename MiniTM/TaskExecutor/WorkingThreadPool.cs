using MiniTM.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniTM.TaskExecutor
{
    /// <summary>
    /// 工作线程池
    /// </summary>
    internal class WorkingThreadPool : IDisposable
    {
        #region Static Members
        /// <summary>
        /// 唯一实例
        /// </summary>
        private static WorkingThreadPool s_Instance;

        /// <summary>
        /// 最大线程数
        /// </summary>
        private static byte s_MaxThreads = 0;

        /// <summary>
        /// 最大任务数
        /// </summary>
        private static ushort s_MaxTaskCnt = 0;

        /// <summary>
        /// 锁对象
        /// </summary>
        private static object s_Lock = new object();

        /// <summary>
        /// 默认最大任务数
        /// </summary>
        private static readonly ushort s_DefTaskCnt = 1024;

        /// <summary>
        /// 配置线程池最大线程数
        /// </summary>
        /// <remarks>应用程序生命周期内只能配置一次, 默认最大任务数为1024</remarks>
        /// <exception cref="ArgumentOutOfRangeException">最大线程数不能为0</exception>
        /// <exception cref="TypeLoadException">线程池已经被初始化</exception>
        /// <param name="maxThreads">最大线程数</param>
        public static void Config(byte maxThreads)
        {
            Config(maxThreads, s_DefTaskCnt);
        }

        /// <summary>
        /// 配置线程池最大线程数
        /// </summary>
        /// <remarks>应用程序生命周期内只能配置一次</remarks>
        /// <exception cref="ArgumentOutOfRangeException">最大线程数不能为0</exception>
        /// <exception cref="TypeLoadException">线程池已经被初始化</exception>
        /// <param name="maxThreads">最大线程数</param>
        /// <param name="maxTaskCount">最大任务数</param>
        public static void Config(byte maxThreads, ushort maxTaskCount)
        {
            if (maxThreads == 0)
            {
                throw new ArgumentOutOfRangeException("不能指定最大线程数为0");
            }

            if (s_MaxThreads == 0)
            {
                s_MaxThreads = maxThreads;
                s_MaxTaskCnt = maxTaskCount;
            }
            else
            {
                throw new TypeLoadException("该线程池已经被初始化");
            }
        }

        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <exception cref="TypeLoadException">尚未指定最大线程数</exception>
        /// <returns></returns>
        public static WorkingThreadPool GetInstance()
        {
            if (s_Instance == null)
            {
                lock (s_Lock)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new WorkingThreadPool();
                    }
                }
            }
            return s_Instance;
        }
        #endregion

        #region Member Fields
        /// <summary>
        /// 任务队列
        /// </summary>
        private ConcurrentQueue<JobItem> m_TaskQueue;

        /// <summary>
        /// 信号量
        /// </summary>
        private Semaphore m_Sem;

        /// <summary>
        /// 是否已释放
        /// </summary>
        private bool m_Disposed;

        /// <summary>
        /// 已释放线程数
        /// </summary>
        private byte m_DisposedCnt;
        #endregion

        #region Constructors
        private WorkingThreadPool()
        {
            if (s_MaxThreads == 0)
            {
                throw new TypeLoadException("该线程池尚未指定最大线程数");
            }
            m_Sem = new Semaphore(0, s_MaxTaskCnt);
            m_TaskQueue = new ConcurrentQueue<JobItem>();
            m_Disposed = false;
            m_DisposedCnt = 0;

            CreateWorkingThreads();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 将工作项加入任务队列等待执行
        /// </summary>
        /// <param name="item">工作项</param>
        /// <exception cref="SemaphoreFullException">任务队列已满</exception>
        public void JobEnqueue(JobItem item)
        {
            m_Sem.Release();
            m_TaskQueue.Enqueue(item);
        }

        /// <summary>
        /// 开始关闭线程池内的线程
        /// </summary>
        public void Dispose()
        {
            m_Disposed = true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 创建工作线程
        /// </summary>
        private void CreateWorkingThreads()
        {
            for (byte i = 0; i < s_MaxThreads; i++)
            {
                Task.Factory.StartNew(DoWork, null, TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// 工作线程运行方法
        /// </summary>
        private async void DoWork(object state)
        {
            // 工作循环
            while (true)
            {
                if (m_Sem.WaitOne(1000))
                {
                    if (m_TaskQueue.TryDequeue(out JobItem job))
                    {
                        var taskItem = job.SrcTask;
                        taskItem.OnJobBegining?.Invoke(job);
                        ExecResult result = new ExecResult();
                        //if (taskItem.CancelToken != null && job.SrcTask.CancelToken.IsCancellationRequested)
                        if (taskItem.CancelToken.IsCancellationRequested)
                        {
                            // 任务取消
                            result.Ok = false;
                            result.Msg = "Task canceled";
                        }
                        else
                        {
                            // 执行任务
                            try
                            {
                                result = await job.ExecBo.ExecuteAsync(job.Parameters);
                            }
                            catch (Exception ex)
                            {
                                result.Ok = false;
                                result.Msg = ex.Message;
                            }
                        }

                        // 回写进度
                        if (result.Ok)
                        {
                            taskItem.Progress.AddOkRecord();
                        }
                        else
                        {
                            taskItem.Progress.AddNgRecord(result.Msg);
                        }
                        //job.OnFinished?.Invoke(job.InnerJob, result);
                        job.SrcTask.OnJobFinished?.Invoke(job, result);
                        // 任务已完成
                        if (taskItem.Progress != null && taskItem.Progress.Finish)
                        {
                            taskItem.CancelToken?.Dispose();
                        }
                    }
                }
                else
                {
                    if (m_Disposed)
                    {
                        break;
                    }
                }
            }

            // 释放信号量
            lock (s_Lock)
            {
                m_DisposedCnt++;
                if (m_DisposedCnt == s_MaxThreads)
                {
                    m_Sem.Dispose();
                    m_Sem = null;
                }
            }
        }
        #endregion
    }
}

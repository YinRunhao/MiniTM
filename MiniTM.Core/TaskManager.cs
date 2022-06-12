using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiniTM.Core
{
    /// <summary>
    /// 任务管理器
    /// </summary>
    public class TaskManager : ITaskManager
    {
        #region Member Fields
        /// <summary>
        /// 任务执行者
        /// </summary>
        protected ITaskExecutor m_Executor;

        /// <summary>
        /// 执行逻辑工厂
        /// </summary>
        protected IJobBoFactory m_Factory;

        /// <summary>
        /// 数据存储
        /// </summary>
        protected IDataStorage m_Storage;

        /// <summary>
        /// 任务取消通知
        /// </summary>
        protected ITaskCancelNotifier m_Notifier;

        /// <summary>
        /// 本地任务字典
        /// </summary>
        protected Dictionary<string, TaskItem> m_TaskDic;

        /// <summary>
        /// 进度清理定时器
        /// </summary>
        private Timer m_ProgressCleaner;

        /// <summary>
        /// 即将被删除的任务进度ID
        /// </summary>
        private readonly List<string> m_RemoveIds = new List<string>();
        #endregion

        #region Constructor
        public TaskManager()
        {
            m_TaskDic = new Dictionary<string, TaskItem>();
            m_ProgressCleaner = new Timer(AutoClean, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }
        #endregion

        #region Public Methods
        public async Task<string> AddTaskAsync<T>(JobParams param) where T : IJobBo
        {
            if (m_Factory == null)
            {
                throw new ArgumentNullException("IJobBoFactory", "未配置执行逻辑工厂");
            }
            var execBo = m_Factory.GetProduct<T>();
            List<JobParams> paramLst = new List<JobParams> { param };
            var taskId = await RunTasksAsync(execBo, paramLst);
            return taskId;
        }

        public async Task<string> AddTasksAsync<T>(IEnumerable<JobParams> param) where T : IJobBo
        {
            if (m_Factory == null)
            {
                throw new ArgumentNullException("IJobBoFactory", "未配置执行逻辑工厂");
            }
            var execBo = m_Factory.GetProduct<T>();
            var taskId = await RunTasksAsync(execBo, param);
            return taskId;
        }

        public async Task<string> AddTaskAsync(Type taskTp, JobParams param)
        {
            if (m_Factory == null)
            {
                throw new ArgumentNullException("IJobBoFactory", "未配置执行逻辑工厂");
            }
            if (CheckJobType(taskTp))
            {
                var execBo = m_Factory.GetProduct(taskTp);
                List<JobParams> paramLst = new List<JobParams> { param };
                var taskId = await RunTasksAsync(execBo, paramLst);
                return taskId;
            }
            else
            {
                throw new ArgumentException($"类型[{taskTp.Name}]未实现IJobBo接口");
            }
        }

        public async Task<string> AddTasksAsync(Type taskTp, IEnumerable<JobParams> param)
        {
            if (m_Factory == null)
            {
                throw new ArgumentNullException("IJobBoFactory", "未配置执行逻辑工厂");
            }
            if (CheckJobType(taskTp))
            {
                var execBo = m_Factory.GetProduct(taskTp);
                return await RunTasksAsync(execBo, param);
            }
            else
            {
                throw new ArgumentException($"类型[{taskTp.Name}]未实现IJobBo接口");
            }
        }

        public virtual async Task<TaskProgressDto> GetProgressAsync(string taskId)
        {
            // 先在本地找任务进度
            if (m_TaskDic.TryGetValue(taskId, out TaskItem task))
            {
                return task.Progress.GetProgressDto();
            }
            // 找不到再向数据存储查找(可能是分布式部署或本地已清理)
            return await m_Storage.GetTaskProgressAsync(taskId);
        }

        public virtual async Task<string> TaskCancelAsync(string taskId)
        {
            if (m_TaskDic.TryGetValue(taskId, out TaskItem task))
            {
                if (!task.Progress.Finish)
                {
                    if (!task.CancelToken.IsCancellationRequested)
                    {
                        task.CancelToken.Cancel();
                    }
                }
            }
            else
            {
                await m_Notifier?.TaskCancelNotifyAsync(taskId);
            }
            return taskId;
        }

        public virtual void Dispose()
        {
            m_ProgressCleaner?.Change(0, -1);
            m_ProgressCleaner?.Dispose();
            m_Executor?.Dispose();
            m_Storage?.Dispose();
            m_Notifier?.Dispose();
        }

        /// <summary>
        /// 配置任务执行者
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public TaskManager UseExecutor(Func<ITaskExecutor> config)
        {
            ITaskExecutor executor = config();
            this.m_Executor?.Dispose();
            this.m_Executor = executor;
            return this;
        }

        /// <summary>
        /// 配置任务执行者
        /// </summary>
        /// <param name="executor"></param>
        /// <returns></returns>
        public TaskManager UseExecutor(ITaskExecutor executor)
        {
            this.m_Executor?.Dispose();
            this.m_Executor = executor;
            return this;
        }

        /// <summary>
        /// 配置任务项工厂
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public TaskManager UseJobBoFactory(Func<IJobBoFactory> config)
        {
            IJobBoFactory factory = config();
            this.m_Factory = factory;
            return this;
        }

        /// <summary>
        /// 配置任务项工厂
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public TaskManager UseJobBoFactory(IJobBoFactory factory)
        {
            this.m_Factory = factory;
            return this;
        }

        /// <summary>
        /// 配置数据存储
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public TaskManager UseDataStorage(Func<IDataStorage> config)
        {
            IDataStorage storage = config();
            this.m_Storage?.Dispose();
            this.m_Storage = storage;
            return this;
        }

        /// <summary>
        /// 配置数据存储
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        public TaskManager UseDataStorage(IDataStorage storage)
        {
            this.m_Storage?.Dispose();
            this.m_Storage = storage;
            return this;
        }

        /// <summary>
        /// 配置任务取消通知者
        /// </summary>
        /// <remarks>不需要通知的可以不配置</remarks>
        /// <param name="config"></param>
        /// <returns></returns>
        public TaskManager UseCancelNotifier(Func<ITaskCancelNotifier> config)
        {
            ITaskCancelNotifier notifier = config();
            this.m_Notifier?.Dispose();
            this.m_Notifier = notifier;
            m_Notifier.SetHandleCallback(CancelTaskCallback);
            return this;
        }

        /// <summary>
        /// 配置任务取消通知者
        /// </summary>
        /// <param name="notifier"></param>
        /// <returns></returns>
        public TaskManager UseCancelNotifier(ITaskCancelNotifier notifier)
        {
            this.m_Notifier?.Dispose();
            this.m_Notifier = notifier;
            return this;
        }
        #endregion

        #region Protecred Methods
        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="execBo"></param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        protected virtual async Task<string> RunTasksAsync(IJobBo execBo, IEnumerable<JobParams> param)
        {
            List<JobParams> jobRun = new List<JobParams>(param);
            IEnumerable<string> repeatId = new List<string>();
            // 不允许重复的工作项
            if (execBo is IDistinctJobBo dstBo)
            {
                Dictionary<string, JobParams> jobDic = new Dictionary<string, JobParams>();
                List<string> jobIds = new List<string>();
                string jobId = string.Empty;
                // 获取名称后根据名称去重
                foreach (var p in param)
                {
                    jobId = dstBo.GetDistinctString(p);
                    jobIds.Add(jobId);

                    jobDic.TryAdd(jobId, p);
                }
                repeatId = await m_Storage.AddJobsIfNotExistsAsync(dstBo.GetType(), jobIds);
                foreach (var id in repeatId)
                {
                    jobDic.Remove(id);
                }
                jobRun = jobDic.Values.ToList();
            }
            var task = CreateTask(execBo, jobRun);
            foreach (var id in repeatId)
            {
                task.Progress.AddNgRecord($"{id} is exist");
            }
            await m_Storage.UpdateTaskProgressAsync(task.TaskId, task.Progress.GetProgressDto());

            if (task.JobList.Count > 0)
            {
                m_TaskDic.Add(task.TaskId, task);
                // 开始执行任务，但不等待完成，直接返回任务ID
                m_Executor.ExecuteAsync(task);
            }

            return task.TaskId;
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="execBo"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual TaskItem CreateTask(IJobBo execBo, IEnumerable<JobParams> param)
        {
            string taskId = Guid.NewGuid().ToString("N");
            CancellationTokenSource source = new CancellationTokenSource();
            TaskItem task = new TaskItem(source);
            task.Progress = new TaskProgress(param.Count());
            task.TaskId = taskId;
            task.JobList = new List<JobItem>();
            task.OnJobFinished = JobFinishCallback;
            JobItem jobItem = default;
            foreach (var p in param)
            {
                jobItem = new JobItem(task)
                {
                    ExecBo = execBo,
                    Parameters = p,
                    TaskId = taskId
                };
                task.JobList.Add(jobItem);
            }

            return task;
        }

        /// <summary>
        /// 任务取消回调
        /// </summary>
        /// <param name="taskId"></param>
        protected virtual void CancelTaskCallback(string taskId)
        {
            if (m_TaskDic.TryGetValue(taskId, out TaskItem task))
            {
                if (!task.Progress.Finish)
                {
                    if (!task.CancelToken.IsCancellationRequested)
                    {
                        task.CancelToken.Cancel();
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 工作项完成回调
        /// </summary>
        /// <param name="job"></param>
        /// <param name="result"></param>
        private void JobFinishCallback(JobItem job, ExecResult result)
        {
            if (job.ExecBo is IDistinctJobBo dstBo)
            {
                string jobId = dstBo.GetDistinctString(job.Parameters);
                m_Storage.RemoveJobAsync(dstBo.GetType(), jobId);
            }
            if (m_TaskDic.TryGetValue(job.TaskId, out TaskItem task))
            {
                var dto = task.Progress.GetProgressDto();
                m_Storage.UpdateTaskProgressAsync(job.TaskId, dto);
            }
        }

        /// <summary>
        /// 检查工作项类型
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        private bool CheckJobType(Type tp)
        {
            var tpIf = typeof(IJobBo);
            return tpIf.IsAssignableFrom(tp);
        }

        /// <summary>
        /// 自动清理
        /// </summary>
        /// <param name="state"></param>
        private void AutoClean(object state)
        {
            // 清理要清理的任务
            foreach (string id in m_RemoveIds)
            {
                m_TaskDic.Remove(id);
            }
            m_RemoveIds.Clear();

            // 查找要清理的任务，留到下次清理时清理
            foreach (var item in m_TaskDic)
            {
                if (item.Value.Progress.Finish)
                {
                    m_RemoveIds.Add(item.Key);
                }
            }
        }
        #endregion
    }
}

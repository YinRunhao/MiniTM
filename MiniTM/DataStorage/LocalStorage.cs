using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniTM.DataStorage
{
    /// <summary>
    /// 基于本地内存的数据存储
    /// </summary>
    /// <remarks>已完成的任务进度默认保留1小时</remarks>
    public partial class LocalStorage : IDataStorage
    {
        /// <summary>
        /// 清理间隔
        /// </summary>
        private TimeSpan m_CleanInterval;

        /// <summary>
        /// 进度字典
        /// </summary>
        private Dictionary<string, TaskProgressDto> m_ProgressDic;

        /// <summary>
        /// 工作项ID集合
        /// </summary>
        private List<JobList> m_JobList;

        /// <summary>
        /// 定时清理器
        /// </summary>
        private Timer m_Cleaner;

        /// <summary>
        /// 即将被删除的任务进度ID
        /// </summary>
        private readonly List<string> m_RemoveIds = new List<string>();

        /// <summary>
        /// 使用默认1小时的清理间隔创建
        /// </summary>
        public LocalStorage() : this(TimeSpan.FromHours(1))
        {
        }

        /// <summary>
        /// 指定清理间隔创建
        /// </summary>
        /// <param name="cleanInterval"></param>
        public LocalStorage(TimeSpan cleanInterval)
        {
            m_CleanInterval = cleanInterval;
            m_ProgressDic = new Dictionary<string, TaskProgressDto>();
            m_JobList = new List<JobList>();
            m_Cleaner = new Timer(AutoClean, null, TimeSpan.Zero, m_CleanInterval);
        }

        public Task<IEnumerable<string>> AddJobsIfNotExistsAsync(Type jobType, IEnumerable<string> jobIds)
        {
            List<string> ret = new List<string>();
            string key = jobType.FullName;
            lock (m_JobList)
            {
                var jobLst = m_JobList.Find(s => s.JobType == key);
                if (jobLst != null)
                {
                    foreach (var newId in jobIds)
                    {
                        // 返回重复的ID
                        if (jobLst.Contains(newId))
                        {
                            ret.Add(newId);
                        }
                        else
                        {
                            // 没有重复的则加入
                            jobLst.Add(newId);
                        }
                    }
                }
                else
                {
                    JobList newLst = new JobList(key);
                    newLst.JobIds.AddRange(jobIds);
                    m_JobList.Add(newLst);
                }
            }
            return Task.FromResult<IEnumerable<string>>(ret);
        }

        public Task RemoveJobAsync(Type jobType, string jobId)
        {
            string key = jobType.FullName;
            lock (m_JobList)
            {
                var jobLst = m_JobList.Find(s => s.JobType == key);
                if (jobLst != null)
                {
                    jobLst.Remove(jobId);
                }
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            m_Cleaner?.Change(0, -1);
            m_Cleaner?.Dispose();
        }

        public Task<TaskProgressDto> GetTaskProgressAsync(string taskId)
        {
            TaskProgressDto ret = default;
            lock (m_ProgressDic)
            {
                if (m_ProgressDic.ContainsKey(taskId))
                {
                    ret = m_ProgressDic[taskId];
                }
            }
            return Task.FromResult(ret);
        }

        public Task UpdateTaskProgressAsync(string taskId, TaskProgressDto data)
        {
            lock (m_ProgressDic)
            {
                if (m_ProgressDic.ContainsKey(taskId))
                {
                    m_ProgressDic[taskId] = data;
                }
                else
                {
                    m_ProgressDic.Add(taskId, data);
                }
            }
            return Task.CompletedTask;
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
                m_ProgressDic.Remove(id);
            }
            m_RemoveIds.Clear();

            // 查找要清理的任务，留到下次清理时清理
            foreach (var item in m_ProgressDic)
            {
                if (item.Value.Total == item.Value.Ng + item.Value.Ok)
                {
                    m_RemoveIds.Add(item.Key);
                }
            }
        }
    }
}

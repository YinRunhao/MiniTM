using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Core
{
    /// <summary>
    /// 数据存储接口
    /// </summary>
    public interface IDataStorage : IDisposable
    {
        /// <summary>
        /// 如果不存在工作项则加入工作项
        /// </summary>
        /// <param name="jobIds">工作项ID</param>
        /// <param name="jobType">工作项类型</param>
        /// <returns>重复的工作项ID</returns>
        Task<IEnumerable<string>> AddJobsIfNotExistsAsync(Type jobType, IEnumerable<string> jobIds);

        /// <summary>
        /// 移除工作项
        /// </summary>
        /// <param name="jobId">工作项ID</param>
        /// <param name="jobType">工作项类型</param>
        /// <returns></returns>
        Task RemoveJobAsync(Type jobType, string jobId);

        /// <summary>
        /// 获取任务进度
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns></returns>
        Task<TaskProgressDto> GetTaskProgressAsync(string taskId);

        /// <summary>
        /// 更新或新增任务进度
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task UpdateTaskProgressAsync(string taskId, TaskProgressDto data);
    }
}

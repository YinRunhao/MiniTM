using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Core
{
    /// <summary>
    /// 任务管理器接口
    /// </summary>
    public interface ITaskManager : IDisposable
    {
        /// <summary>
        /// 获取进度
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>任务进度</returns>
        Task<TaskProgressDto> GetProgressAsync(string taskId);

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <typeparam name="T">任务类型</typeparam>
        /// <param name="param">参数</param>
        /// <returns>任务ID</returns>
        Task<string> AddTaskAsync<T>(JobParams param) where T : IJobBo;

        /// <summary>
        /// 添加任务集合
        /// </summary>
        /// <typeparam name="T">任务类型</typeparam>
        /// <param name="param">参数</param>
        /// <returns>任务ID</returns>
        Task<string> AddTasksAsync<T>(IEnumerable<JobParams> param) where T : IJobBo;

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="taskTp">任务类型</param>
        /// <param name="param">参数</param>
        /// <returns>任务ID</returns>
        Task<string> AddTaskAsync(Type taskTp, JobParams param);

        /// <summary>
        /// 添加任务集合
        /// </summary>
        /// <param name="taskTp">任务类型</param>
        /// <param name="param">参数</param>
        /// <returns>任务ID</returns>
        Task<string> AddTasksAsync(Type taskTp, IEnumerable<JobParams> param);

        /// <summary>
        /// 取消任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns>任务ID，返回空字符串表示找不到任务</returns>
        Task<string> TaskCancelAsync(string taskId);
    }
}

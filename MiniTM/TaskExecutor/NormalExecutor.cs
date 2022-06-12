using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.TaskExecutor
{
    /// <summary>
    /// 普通执行器
    /// </summary>
    public class NormalExecutor : ITaskExecutor
    {
        public void Dispose()
        {
        }

        public async Task ExecuteAsync(TaskItem task)
        {
            var jobs = task.JobList;
            foreach(var item in jobs)
            {
                task.OnJobBegining?.Invoke(item);
                ExecResult result = new ExecResult();
                if (task.CancelToken.IsCancellationRequested)
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
                        result = await item.ExecBo.ExecuteAsync(item.Parameters);
                    }
                    catch(Exception ex)
                    {
                        result.Ok = false;
                        result.Msg = ex.Message;
                    }
                }

                // 回写进度
                if (result.Ok)
                {
                    task.Progress?.AddOkRecord();
                }
                else
                {
                    task.Progress.AddNgRecord(result.Msg);
                }
                task.OnJobFinished?.Invoke(item, result);
            }

            // 任务执行完就释放CancelToken
            task.CancelToken?.Dispose();
        }
    }
}

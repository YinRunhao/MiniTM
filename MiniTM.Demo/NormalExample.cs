using MiniTM.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Demo
{
    /// <summary>
    /// 普通用例
    /// </summary>
    public class NormalExample
    {
        /// <summary>
        /// 单工作项任务
        /// </summary>
        /// <param name="mng"></param>
        /// <returns></returns>
        protected async Task<List<string>> AddSingleJobTask(ITaskManager mng)
        {
            // 任务ID
            List<string> ret = new List<string>();

            JobParams p1 = new JobParams();
            p1.AddParam("name", "Mike");

            JobParams p2 = new JobParams();
            p2.AddParam("name", "Leo");

            JobParams p3 = new JobParams();
            p3.AddParam("name", "Don");

            JobParams p4 = new JobParams();
            p4.AddParam("name", "Raph");

            ret.Add(await mng.AddTaskAsync<HiJobBo>(p1));
            ret.Add(await mng.AddTaskAsync(typeof(HiJobBo), p2));

            ret.Add(await mng.AddTaskAsync<HelloJobBo>(p3));
            ret.Add(await mng.AddTaskAsync(typeof(HelloJobBo), p4));

            return ret;
        }

        /// <summary>
        /// 多工作项任务
        /// </summary>
        /// <param name="mng"></param>
        /// <returns></returns>
        protected async Task<List<string>> AddMultiJobTask(ITaskManager mng)
        {
            List<JobParams> jobs = new List<JobParams>();
            // 任务ID
            List<string> ret = new List<string>();

            JobParams p1 = new JobParams();
            p1.AddParam("name", "Mike");
            jobs.Add(p1);

            JobParams p2 = new JobParams();
            p2.AddParam("name", "Leo");
            jobs.Add(p2);

            JobParams p3 = new JobParams();
            p3.AddParam("name", "Don");
            jobs.Add(p3);

            List<JobParams> jobs1 = new List<JobParams>();
            JobParams p4 = new JobParams();
            p4.AddParam("name", "Raph");
            jobs1.Add(p3);

            ret.Add(await mng.AddTasksAsync<HiJobBo>(jobs));

            ret.Add(await mng.AddTasksAsync(typeof(HelloJobBo), jobs1));

            return ret;
        }

        protected void PrintProgress(string taskId, TaskProgressDto progress)
        {
            string json = JsonConvert.SerializeObject(progress);
            Console.WriteLine($"{taskId}: {json}");
        }
    }
}

using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Demo
{
    public class DistinctJobExample : NormalExample
    {
        public async Task Run()
        {
            SimpleJobFactory factory = new SimpleJobFactory();
            // 普通执行器
            var mng = TaskManagerFactory.CreateDefaultManager().UseJobBoFactory(factory);

            string input = string.Empty;
            TaskProgressDto progress = default;
            var taskIdArr = await AddDistinctJobTask(mng);
            Console.WriteLine("Add Task Success Press Enter to Search Progress, Enter 'exit' to close");
            while (true)
            {
                input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }

                foreach (var taskId in taskIdArr)
                {
                    progress = await mng.GetProgressAsync(taskId);
                    PrintProgress(taskId, progress);
                }
            }
            mng.Dispose();
        }

        private async Task<List<string>> AddDistinctJobTask(ITaskManager mng)
        {
            List<JobParams> jobs = new List<JobParams>();

            JobParams p1 = new JobParams();
            p1.AddParam("name", "Leo");
            p1.AddParam("idCard", "441900111111111111");
            jobs.Add(p1);

            JobParams p2 = new JobParams();
            p2.AddParam("name", "LEO");
            p2.AddParam("idCard", "441900111111111111");
            jobs.Add(p2);

            JobParams p3 = new JobParams();
            p3.AddParam("name", "Don");
            p3.AddParam("idCard", "441900222222222222");
            jobs.Add(p3);

            JobParams p4 = new JobParams();
            p4.AddParam("name", "DON");
            p4.AddParam("idCard", "441900222222222222");
            jobs.Add(p4);

            // 4个工作项实际上只有2个有效
            string taskId = await mng.AddTasksAsync<AddUserBo>(jobs);
            return new List<string>() { taskId };
        }
    }
}

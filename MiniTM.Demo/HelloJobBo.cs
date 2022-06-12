using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Demo
{
    public class HelloJobBo : IJobBo
    {
        public async Task<ExecResult> ExecuteAsync(JobParams param)
        {
            // 假装工作了2秒
            await Task.Delay(2000);
            ExecResult result = new ExecResult();
            string name = param.GetParam<string>("name");
            Console.WriteLine($"{name} say hello");
            result.Ok = true;
            result.Msg = "OK";
            return result;
        }
    }
}

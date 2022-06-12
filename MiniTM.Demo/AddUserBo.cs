using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Demo
{
    /// <summary>
    /// 不允许重复工作项示例，待执行队列中不允许重复
    /// </summary>
    /// <remark>参数将传用户名和身份证号，以身份证号作为唯一ID</remark>
    public class AddUserBo : IDistinctJobBo
    {
        public async Task<ExecResult> ExecuteAsync(JobParams param)
        {
            // 假装工作了2秒
            await Task.Delay(2000);
            ExecResult result = new ExecResult();
            string name = param.GetParam<string>("name");
            string idCard = param.GetParam<string>("idCard");
            Console.WriteLine($"Add user [{name}] idCard:{idCard}");
            result.Ok = true;
            result.Msg = "OK";
            return result;
        }

        public string GetDistinctString(JobParams param)
        {
            // 返回唯一标识
            string id = param.GetParam<string>("idCard");
            return id;
        }
    }
}

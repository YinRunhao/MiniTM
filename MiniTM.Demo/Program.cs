using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiniTM.Core;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace MiniTM.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 简单的示例
            var example = new SimpleNormalExample();

            // 使用依赖注入创建业务逻辑对象
            //var example = new DINormalExample();

            // 不允许重复的工作项示例
            //var example = new DistinctJobExample();

            await example.Run();
        }
    }
}

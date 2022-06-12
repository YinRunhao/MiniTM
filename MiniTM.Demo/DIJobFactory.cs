using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace MiniTM.Demo
{
    /// <summary>
    /// 使用依赖注入的业务逻辑工厂
    /// </summary>
    public class DIJobFactory : IJobBoFactory
    {
        private IServiceProvider m_Service;

        public DIJobFactory(IServiceProvider sp)
        {
            m_Service = sp;
        }

        public T GetProduct<T>() where T : IJobBo
        {
            var ret = m_Service.GetService<T>();
            return ret;
        }

        public IJobBo GetProduct(Type type)
        {
            var ret = m_Service.GetService(type);
            return (IJobBo)ret;
        }
    }
}

using MiniTM.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Demo
{
    /// <summary>
    /// 简单的业务逻辑工厂
    /// </summary>
    public class SimpleJobFactory : IJobBoFactory
    {
        public T GetProduct<T>() where T : IJobBo
        {
            IJobBo ret = default;
            var type = typeof(T);
            // 此处可使用特性等方法确定要创建的业务逻辑类型
            if (type.Name == "HiJobBo")
            {
                ret = new HiJobBo();
            }
            else if(type.Name == "HelloJobBo")
            {
                ret = new HelloJobBo();
            }
            else if(type.Name == "AddUserBo")
            {
                ret = new AddUserBo();
            }

            return (T)ret;
        }

        public IJobBo GetProduct(Type type)
        {
            IJobBo ret = default;
            // 此处可使用特性等方法确定要创建的业务逻辑类型
            if (type.Name == "HiJobBo")
            {
                ret = new HiJobBo();
            }
            else if (type.Name == "HelloJobBo")
            {
                ret = new HelloJobBo();
            }
            else if (type.Name == "AddUserBo")
            {
                ret = new AddUserBo();
            }

            return ret;
        }
    }
}

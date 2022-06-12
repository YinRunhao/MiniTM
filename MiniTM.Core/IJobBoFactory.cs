using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Core
{
    /// <summary>
    /// 工作项工厂
    /// </summary>
    public interface IJobBoFactory
    {
        /// <summary>
        /// 获取工作项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetProduct<T>() where T : IJobBo;

        /// <summary>
        /// 获取工作项
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IJobBo GetProduct(Type type);
    }
}

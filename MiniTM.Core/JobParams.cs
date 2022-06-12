using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.Core
{
    /// <summary>
    /// 工作项参数集合
    /// </summary>
    /// <remarks>一个工作项参数集合表示一次执行</remarks>
    public class JobParams
    {
        private Dictionary<string, object> m_Dic;

        public object this[string nm]
        {
            get
            {
                return m_Dic[nm];
            }
            set
            {
                m_Dic[nm] = value;
            }
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddParam(string name, object value)
        {
            m_Dic.TryAdd(name, value);
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetParam<T>(string name)
        {
            if (m_Dic.TryGetValue(name, out object val))
            {
                if (val is T)
                {
                    return (T)val;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }

        public Dictionary<string, object> GetSrcDictionary()
        {
            return m_Dic;
        }

        public JobParams()
        {
            m_Dic = new Dictionary<string, object>();
        }

        public JobParams(Dictionary<string, object> dic)
        {
            m_Dic = new Dictionary<string, object>();
            foreach (var item in dic)
            {
                m_Dic.Add(item.Key, item.Value);
            }
        }
    }
}

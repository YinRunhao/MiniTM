using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MiniTM.DataStorage
{
    public partial class LocalStorage
    {
        /// <summary>
        /// 工作项ID集合
        /// </summary>
        private class JobList : IList<string>
        {
            /// <summary>
            /// 工作项类型全名
            /// </summary>
            public string JobType { get; set; }

            /// <summary>
            /// 工作项唯一ID
            /// </summary>
            public List<string> JobIds { get; set; }

            public JobList(string jobTpNm)
            {
                JobType = jobTpNm;
                JobIds = new List<string>();
            }

            public string this[int index] { get => ((IList<string>)JobIds)[index]; set => ((IList<string>)JobIds)[index] = value; }

            public int Count => ((ICollection<string>)JobIds).Count;

            public bool IsReadOnly => ((ICollection<string>)JobIds).IsReadOnly;

            public void Add(string item)
            {
                ((ICollection<string>)JobIds).Add(item);
            }

            public void Clear()
            {
                ((ICollection<string>)JobIds).Clear();
            }

            public bool Contains(string item)
            {
                return ((ICollection<string>)JobIds).Contains(item);
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                ((ICollection<string>)JobIds).CopyTo(array, arrayIndex);
            }

            public IEnumerator<string> GetEnumerator()
            {
                return ((IEnumerable<string>)JobIds).GetEnumerator();
            }

            public int IndexOf(string item)
            {
                return ((IList<string>)JobIds).IndexOf(item);
            }

            public void Insert(int index, string item)
            {
                ((IList<string>)JobIds).Insert(index, item);
            }

            public bool Remove(string item)
            {
                return ((ICollection<string>)JobIds).Remove(item);
            }

            public void RemoveAt(int index)
            {
                ((IList<string>)JobIds).RemoveAt(index);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)JobIds).GetEnumerator();
            }
        }
    }
}

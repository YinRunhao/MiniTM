using System;
using System.Collections.Generic;
using System.Threading;

namespace MiniTM.Core
{
    /// <summary>
    /// 任务进度
    /// </summary>
    public class TaskProgress
    {
        /// <summary>
        /// 工作项总数
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// 成功数
        /// </summary>
        public int Ok
        {
            get
            {
                return m_Ok;
            }
        }

        /// <summary>
        /// 失败数
        /// </summary>
        public int Ng
        {
            get
            {
                return m_Ng;
            }
        }

        private int m_Ok;

        private int m_Ng;

        public bool Finish
        {
            get
            {
                return m_Ok + m_Ng == Total;
            }
        }

        private List<string> ErrMsg;

        public TaskProgress(int total)
        {
            this.Total = total;
            ErrMsg = new List<string>();
        }

        /// <summary>
        /// 增加失败记录
        /// </summary>
        /// <param name="msg"></param>
        public void AddNgRecord(string msg)
        {
            Interlocked.Increment(ref m_Ng);
            lock (this)
            {
                ErrMsg.Add(msg);
            }
        }

        /// <summary>
        /// 增加成功记录
        /// </summary>
        public void AddOkRecord()
        {
            Interlocked.Increment(ref m_Ok);
        }

        /// <summary>
        /// 获取当前进度的快照
        /// </summary>
        /// <returns>进度传输对象</returns>
        public TaskProgressDto GetProgressDto()
        {
            List<string> err = new List<string>();
            lock (this)
            {
                foreach (var item in ErrMsg)
                {
                    err.Add(item);
                }
            }
            TaskProgressDto ret = new TaskProgressDto
            {
                Ok = m_Ok,
                Ng = m_Ng,
                Total = Total,
                ErrMsg = err
            };
            return ret;
        }
    }
}

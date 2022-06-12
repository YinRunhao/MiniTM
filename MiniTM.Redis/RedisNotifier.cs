using MiniTM.Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Redis
{
    /// <summary>
    /// 使用Redis的发布订阅作为任务取消消息通知器
    /// </summary>
    public class RedisNotifier : ITaskCancelNotifier
    {
        private RedisConnection m_Connection;

        private string m_CancelChannel;

        private ISubscriber m_Subscriber;

        private Action<string> m_Callback;

        public RedisNotifier(RedisNotifierConfig config)
        {
            m_CancelChannel = config.CancelChannel;
            m_Connection = new RedisConnection(config.ConnectionString);
        }

        internal RedisNotifier(RedisConnection conn, string cancelChannel)
        {
            m_Connection = conn;
            m_CancelChannel = cancelChannel;
        }

        public void Dispose()
        {
            m_Subscriber?.UnsubscribeAll();
            m_Connection?.Dispose();
        }

        public void SetHandleCallback(Action<string> callback)
        {
            if(m_Subscriber == null)
            {
                m_Subscriber = m_Connection.GetSubscriber();
            }
            m_Callback = callback;
            m_Subscriber.Subscribe(m_CancelChannel, CancelCallback);
        }

        public async Task TaskCancelNotifyAsync(string taskId)
        {
            if (m_Subscriber == null)
            {
                m_Subscriber = await m_Connection.GetSubscriberAsync();
            }
            await m_Subscriber.PublishAsync(m_CancelChannel, taskId);
        }

        private void CancelCallback(RedisChannel cnl, RedisValue val)
        {
            m_Callback?.Invoke(val);
        }
    }
}

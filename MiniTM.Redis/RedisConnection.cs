using StackExchange.Redis;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MiniTM.Redis
{
    /// <summary>
    ///  Redis连接
    /// </summary>
    internal class RedisConnection : IDisposable
    {
        private readonly string m_ConnStr;

        private readonly int m_DefDb;

        private ConnectionMultiplexer m_Connection;

        public RedisConnection(string connStr, int defDb = 0)
        {
            m_ConnStr = connStr;
            m_DefDb = defDb;
        }

        /// <summary>
        /// 使用Redis分布式锁执行某些操作
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="act">操作</param>
        /// <param name="expiry">锁过期时间，若超出时间自动解锁 单位：sec</param>
        /// <param name="retry">获取锁的重复次数</param>
        /// <param name="tryDelay">获取锁的重试间隔  单位：ms</param>
        public void LockAction(string lockName, Action act, int expiry = 10, int retry = 3, int tryDelay = 200)
        {
            var db = GetDatabase();
            db.LockAction(lockName, act, expiry, retry, tryDelay);
        }

        /// <summary>
        /// 使用Redis分布式锁执行某些异步操作
        /// </summary>
        /// <param name="lockName">锁名</param>
        /// <param name="act">操作</param>
        /// <param name="expiry">锁过期时间，若超出时间自动解锁 单位：sec</param>
        /// <param name="retry">获取锁的重复次数</param>
        /// <param name="tryDelay">获取锁的重试间隔  单位：ms</param>
        public async Task LockActionAsync(string lockName, Func<Task> act, int expiry = 10, int retry = 3, int tryDelay = 200)
        {
            var db = await GetDatabaseAsync();
            await db.LockActionAsync(lockName, act, expiry, retry, tryDelay);
        }

        public async Task<IDatabase> GetDatabaseAsync(int db)
        {
            await ConnectAsync();
            return m_Connection.GetDatabase(db);
        }

        public IDatabase GetDatabase(int db)
        {
            Connect();
            return m_Connection.GetDatabase(db);
        }

        public async Task<IDatabase> GetDatabaseAsync()
        {
            return await GetDatabaseAsync(m_DefDb);
        }

        public IDatabase GetDatabase()
        {
            return GetDatabase(m_DefDb);
        }

        public async Task<ISubscriber> GetSubscriberAsync()
        {
            await ConnectAsync();
            return m_Connection.GetSubscriber();
        }

        public ISubscriber GetSubscriber()
        {
            Connect();
            return m_Connection.GetSubscriber();
        }

        public IConnectionMultiplexer GetMultiplexer()
        {
            Connect();
            return m_Connection;
        }

        public async Task<IConnectionMultiplexer> GetMultiplexerAsync()
        {
            await ConnectAsync();
            return m_Connection;
        }

        public void Dispose()
        {
            if (m_Connection != null)
            {
                m_Connection?.Dispose();
                m_Connection = null;
            }
        }

        private async Task ConnectAsync()
        {
            if (m_Connection == null)
            {
                m_Connection = await ConnectionMultiplexer.ConnectAsync(m_ConnStr);
            }
        }

        private void Connect()
        {
            if (m_Connection == null)
            {
                m_Connection = ConnectionMultiplexer.Connect(m_ConnStr);
            }
        }
    }
}

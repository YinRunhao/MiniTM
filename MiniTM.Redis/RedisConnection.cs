using StackExchange.Redis;
using System;
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

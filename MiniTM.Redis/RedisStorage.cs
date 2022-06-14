using MiniTM.Core;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTM.Redis
{
    /// <summary>
    /// 使用Redis作为数据存储器
    /// </summary>
    public class RedisStorage : IDataStorage
    {
        #region Member Fields
        /// <summary>
        /// 配置项
        /// </summary>
        private RedisStorageConfig m_Config;

        /// <summary>
        /// Redis连接
        /// </summary>
        private RedisConnection m_Conn;
        #endregion

        #region Constructor
        public RedisStorage(RedisStorageConfig config)
        {
            m_Config = config;
            m_Conn = new RedisConnection(config.ConnectionString, config.DefDb);
        }

        internal RedisStorage(RedisConnection conn, RedisStorageConfig config)
        {
            m_Config = config;
            m_Conn = conn;
        }
        #endregion

        #region Public Methods
        public async Task<IEnumerable<string>> AddJobsIfNotExistsAsync(Type jobType, IEnumerable<string> jobIds)
        {
            List<string> rptLst = new List<string>();
            List<RedisValue> addLst = new List<RedisValue>();

            string key = GetDistinctJobKey(jobType);
            var db = await m_Conn.GetDatabaseAsync(m_Config.DefDb);

            await db.LockActionAsync(key, async () =>
            {
                // LOCK
                var existsJob = await db.ListRangeAsync(key);
                var existsLst = existsJob.ToList();
                foreach (var job in jobIds)
                {
                    if (existsLst.IndexOf(job) >= 0)
                    {
                        rptLst.Add(job);
                    }
                    else
                    {
                        addLst.Add(job);
                    }
                }
                if (addLst.Any())
                {

                    await db.ListLeftPushAsync(key, addLst.ToArray());
                }
            });

            return rptLst;
        }

        public async Task RemoveJobAsync(Type jobType, string jobId)
        {
            string key = GetDistinctJobKey(jobType);
            var db = await m_Conn.GetDatabaseAsync(m_Config.DefDb);
            await db.ListRemoveAsync(key, jobId);
        }

        public void Dispose()
        {
            m_Conn?.Dispose();
        }

        public async Task<TaskProgressDto> GetTaskProgressAsync(string taskId)
        {
            string key = GetRedisKey(taskId);
            var db = await m_Conn.GetDatabaseAsync(m_Config.DefDb);
            string json = await db.StringGetAsync(key);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            var ret = JsonConvert.DeserializeObject<TaskProgressDto>(json);
            return ret;
        }

        public async Task UpdateTaskProgressAsync(string taskId, TaskProgressDto data)
        {
            string key = GetRedisKey(taskId);
            string val = JsonConvert.SerializeObject(data);
            var db = await m_Conn.GetDatabaseAsync(m_Config.DefDb);
            await db.StringSetAsync(key, val, m_Config.ProgressKeepTime);
        }
        #endregion

        #region Private Methods
        private string GetRedisKey(string key)
        {
            return m_Config.Prefix + ":task:" + key;
        }

        /// <summary>
        /// 获取唯一工作项的列表Key
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        private string GetDistinctJobKey(Type jobType)
        {
            return m_Config.JobKeyCreator.GetKey(m_Config.Prefix, jobType);
        }
        #endregion
    }
}

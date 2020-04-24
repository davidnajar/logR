using logR.Server.Services.Interfaces;
using logR.Shared;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace logR.Server.Services
{
    public class LogStorageService : ILogStorage
    {
        private readonly IMongoClient _mongoClient;

        public LogStorageService(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }
        public async Task<IEnumerable<LogEvent>> SearchLogs(Expression<Func<LogEvent,bool>> query)
        {
            var result = await _mongoClient.GetDatabase("log").GetCollection<LogEvent>("logs").FindAsync(query);
            return result.ToEnumerable();
            
        }

        public async Task StoreLog(LogEvent logEvent)
        {
            await _mongoClient.GetDatabase("log").GetCollection<LogEvent>("logs").InsertOneAsync(logEvent);
        }
    }
}

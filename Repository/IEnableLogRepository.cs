using DataAccess.DAO;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class EnableLogRepository : IEnableLogRepository
    {
        private readonly EnableLogDAO _enableLogDAO;

        public EnableLogRepository(EnableLogDAO enableLogDAO)
        {
            _enableLogDAO = enableLogDAO;
        }
        public async Task<List<EnableLog>> GetAllLogsAsync()
        {
            return await _enableLogDAO.GetAllLogsAsync();
        }

        public async Task<EnableLog> GetLogByIdAsync(int logId)
        {
            return await _enableLogDAO.GetLogByIdAsync(logId);
        }

        public async Task AddLogAsync(EnableLog log)
        {
            await _enableLogDAO.AddLogAsync(log);
        }

        public async Task UpdateLogAsync(EnableLog log)
        {
            await _enableLogDAO.UpdateLogAsync(log);
        }

        public async Task DeleteLogAsync(int logId)
        {
            await _enableLogDAO.DeleteLogAsync(logId);
        }
    }
}

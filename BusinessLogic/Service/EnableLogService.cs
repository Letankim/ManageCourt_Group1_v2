using BusinessLogic.Interface;

using Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Interface;

namespace BusinessLogic.Service
{
    public class EnableLogService : IEnableLogService
    {
        private readonly IEnableLogRepository enableLogRepository;

        public EnableLogService(IEnableLogRepository enableLogRepository)
        {
            this.enableLogRepository = enableLogRepository;
        }

        public async Task AddLogAsync(EnableLog log)
        {
            await enableLogRepository.AddLogAsync(log);
        }

        public async Task DeleteLogAsync(int id)
        {
            await enableLogRepository.DeleteLogAsync(id);
        }

        public async Task<EnableLog> GetLogByIdAsync(int id)
        {
            return await enableLogRepository.GetLogByIdAsync(id);
        }

        public async Task<IEnumerable<EnableLog>> GetListAllLogsAsync()
        {
            return await enableLogRepository.GetAllLogsAsync();
        }

        public async Task UpdateLogAsync(EnableLog log)
        {
            await enableLogRepository.UpdateLogAsync(log);
        }
    }
}

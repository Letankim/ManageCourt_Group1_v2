using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IEnableLogRepository
    {
        Task<List<EnableLog>> GetAllLogsAsync();
        Task<EnableLog> GetLogByIdAsync(int logId);
        Task AddLogAsync(EnableLog log);
        Task UpdateLogAsync(EnableLog log);
        Task DeleteLogAsync(int logId);
    }
}

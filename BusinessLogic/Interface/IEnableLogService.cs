using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IEnableLogService
    {
        Task<IEnumerable<EnableLog>> GetListAllLogsAsync();
        Task<EnableLog> GetLogByIdAsync(int id);
        Task AddLogAsync(EnableLog item);
        Task UpdateLogAsync(EnableLog item);
        Task DeleteLogAsync(int id);
    }
}

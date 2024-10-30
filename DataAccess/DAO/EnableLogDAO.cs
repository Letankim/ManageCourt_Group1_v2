using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class EnableLogDAO : SingletonBase<EnableLog>
    {
        public async Task<List<EnableLog>> GetAllLogsAsync()
        {
            return await _context.EnableLogs.ToListAsync();
        }

        public async Task<EnableLog> GetLogByIdAsync(int logId)
        {
            return await _context.EnableLogs.FindAsync(logId);
        }

        public async Task AddLogAsync(EnableLog log)
        {
            await _context.EnableLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLogAsync(EnableLog log)
        {
            _context.EnableLogs.Update(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLogAsync(int logId)
        {
            var log = await _context.EnableLogs.FindAsync(logId);
            if (log != null)
            {
                _context.EnableLogs.Remove(log);
                await _context.SaveChangesAsync();
            }
        }
    }
}

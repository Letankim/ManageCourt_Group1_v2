using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class BadmintonCourtDAO : SingletonBase<BadmintonCourt>
    {
        public async Task<List<BadmintonCourt>> GetAllCourtsAsync()
        {
            return await _context.BadmintonCourts.ToListAsync();
        }

        public async Task<BadmintonCourt> GetCourtByIdAsync(int courtId)
        {
            return await _context.BadmintonCourts.FindAsync(courtId);
        }

        public async Task AddCourtAsync(BadmintonCourt court)
        {
            await _context.BadmintonCourts.AddAsync(court);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourtAsync(BadmintonCourt court)
        {
            _context.BadmintonCourts.Update(court);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourtAsync(int courtId)
        {
            var court = await _context.BadmintonCourts.FindAsync(courtId);
            if (court != null)
            {
                _context.BadmintonCourts.Remove(court);
                await _context.SaveChangesAsync();
            }
        }
    }
}

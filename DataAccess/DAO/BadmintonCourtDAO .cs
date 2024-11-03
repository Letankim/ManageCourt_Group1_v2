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
            return await _context.BadmintonCourts
                .Include(c => c.CourtImages)
                .Include(c => c.Owner)
                .Include(c => c.CourtSchedules)
                .Include(c => c.Bookings) 
                .FirstOrDefaultAsync(c => c.CourtId == courtId);
        }

        public async Task AddCourtAsync(BadmintonCourt court)
        {
            await _context.BadmintonCourts.AddAsync(court);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourtAsync(BadmintonCourt court)
        {
            if (court == null)
            {
                throw new ArgumentNullException(nameof(court), "Court data is missing.");
            }

            var existingCourt = await _context.BadmintonCourts.FirstOrDefaultAsync(c => c.CourtId == court.CourtId);
            if (existingCourt != null)
            {
                _context.Entry(existingCourt).CurrentValues.SetValues(court);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("The court could not be found.");
            }
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

        public async Task<List<BadmintonCourt>> GetAllEnabledCourtsAsync(int page, int pageSize)
        {
            return await _context.BadmintonCourts
                                 .Where(court => court.IsEnabled == true) 
                                 .Skip((page - 1) * pageSize)
                                 .Include((p)=> p.CourtImages)
                                 .Include(p => p.Owner)
                                 .Take(pageSize)
                                 .ToListAsync();
        }


        public async Task<int> GetCourtsCountAsync()
        {
            return await _context.BadmintonCourts
                 .Where(court => court.IsEnabled == true)
                 .CountAsync();
        }

        public async Task<(List<BadmintonCourt>, int)> GetFilteredCourtsAsync(int page, int pageSize, decimal? minPrice, decimal? maxPrice, TimeOnly? openTime, TimeOnly? closeTime, string search)
        {
            var query = _context.BadmintonCourts
                                .Include(c => c.CourtImages)
                                .Include(c => c.Owner)
                                .Where(c => c.IsEnabled == true)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.CourtName.Contains(search) || c.Location.Contains(search));

            if (minPrice.HasValue)
                query = query.Where(c => c.PricePerHour >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(c => c.PricePerHour <= maxPrice.Value);

            if (openTime.HasValue)
                query = query.Where(c => c.StartTime <= openTime.Value);

            if (closeTime.HasValue)
                query = query.Where(c => c.EndTime >= closeTime.Value);

            int totalCourts = await query.CountAsync();
            var courts = await query
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            return (courts, totalCourts);
        }


        public async Task<BadmintonCourt> GetCourtByIdActiveAsync(int courtId)
        {
            return await _context.BadmintonCourts
                                 .Where(court => court.CourtId == courtId && court.IsEnabled == true) 
                                 .Include(court => court.CourtImages)  
                                 .Include(court => court.Owner)        
                                 .FirstOrDefaultAsync();      
        }

        public async Task<List<BadmintonCourt>> GetCourtsByOwnerIdAsync(int ownerId)
        {
            return await _context.BadmintonCourts
                                 .Where(court => court.OwnerId == ownerId && court.IsEnabled == true)
                                 .Include(court => court.CourtImages) 
                                 .Include(court => court.Owner)   
                                 .ToListAsync();
        }


    }
}

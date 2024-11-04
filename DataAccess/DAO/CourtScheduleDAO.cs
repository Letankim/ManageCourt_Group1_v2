using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CourtScheduleDAO : SingletonBase<CourtSchedule>
    {
        public async Task<List<CourtSchedule>> GetAllSchedulesAsync()
        {
            return await _context.CourtSchedules.Include(s => s.Court).ToListAsync();
        }
        
        public async Task<List<CourtSchedule>> GetAllSchedulesAllCourtNameAsync()
        {
            var allCourtName = await _context.CourtSchedules.Include(s => s.Court).GroupBy(s => s.CourtId).Select(s => s.First()).ToListAsync();
            return allCourtName.OrderBy(s => s.CourtId).ToList();
        }

        public async Task<CourtSchedule> GetScheduleByIdAsync(int scheduleId)
        {
            return await _context.CourtSchedules.FindAsync(scheduleId);
        }
        
        public async Task<List<CourtSchedule>> GetScheduleByCourtIdAsync(int courtId)
        {
            return await _context.CourtSchedules.Where(s => s.CourtId == courtId).ToListAsync();
        }

        public async Task AddScheduleAsync(CourtSchedule schedule)
        {
            await _context.CourtSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateScheduleAsync(CourtSchedule schedule)
        {
            _context.CourtSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteScheduleAsync(int scheduleId)
        {
            var schedule = await _context.CourtSchedules.FindAsync(scheduleId);
            if (schedule != null)
            {
                _context.CourtSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkScheduleAsUnavailableAsync(int scheduleId)
        {
            var schedule = await _context.CourtSchedules.FindAsync(scheduleId);
            if (schedule != null)
            {
                schedule.IsAvailable = false;
                _context.CourtSchedules.Update(schedule);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CourtSchedule>> GetSchedulesByCourtIdAsync(int courtId, DateOnly date)
        {
            return await _context.CourtSchedules
                .Where(schedule => schedule.CourtId == courtId && schedule.Date == date)
                .ToListAsync();
        }

        public async Task<List<CourtSchedule>> GetAvailableSchedulesAsync(int courtId, DateOnly date)
        {
            return await _context.CourtSchedules
                .Where(schedule => schedule.CourtId == courtId &&
                                   schedule.Date == date &&
                                   schedule.IsAvailable == true)
                .ToListAsync();
        }

        public async Task<(int availableCount, int bookedCount)> GetAvailabilityStatisticsAsync(DateOnly startDate, DateOnly endDate)
        {
            var schedules = await _context.CourtSchedules
                .Where(schedule => schedule.Date >= startDate && schedule.Date <= endDate)
                .ToListAsync();

            int availableCount = schedules.Count(s => s.IsAvailable == true);
            int bookedCount = schedules.Count(s => s.IsAvailable == false);

            return (availableCount, bookedCount);
        }



    }
}

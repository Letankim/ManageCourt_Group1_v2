using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class CourtScheduleRepository : ICourtScheduleRepository
    {
        private readonly CourtScheduleDAO _courtScheduleDAO;

        public CourtScheduleRepository(CourtScheduleDAO courtScheduleDAO)
        {
            _courtScheduleDAO = courtScheduleDAO;
        }

        public async Task<List<CourtSchedule>> GetAllSchedulesAsync()
        {
            return await _courtScheduleDAO.GetAllSchedulesAsync();
        }

        public async Task<CourtSchedule> GetScheduleByIdAsync(int scheduleId)
        {
            return await _courtScheduleDAO.GetScheduleByIdAsync(scheduleId);
        }

        public async Task AddScheduleAsync(CourtSchedule schedule)
        {
            await _courtScheduleDAO.AddScheduleAsync(schedule);
        }

        public async Task UpdateScheduleAsync(CourtSchedule schedule)
        {
            await _courtScheduleDAO.UpdateScheduleAsync(schedule);
        }

        public async Task DeleteScheduleAsync(int scheduleId)
        {
            var schedule = await _courtScheduleDAO.GetScheduleByIdAsync(scheduleId);
            if (schedule != null)
            {
                await _courtScheduleDAO.DeleteScheduleAsync(scheduleId);
            }
        }

        public async Task<List<CourtSchedule>> GetSchedulesByCourtIdAsync(int courtId, DateOnly date)
        {
            return await _courtScheduleDAO.GetSchedulesByCourtIdAsync(courtId, date);
        }

        public async Task<List<CourtSchedule>> GetAvailableSchedulesAsync(int courtId, DateOnly date)
        {
            return await _courtScheduleDAO.GetAvailableSchedulesAsync(courtId, date);
        }

        public async Task MarkScheduleAsUnavailableAsync(int scheduleId)
        {
            await _courtScheduleDAO.MarkScheduleAsUnavailableAsync(scheduleId);
        }

    }
}

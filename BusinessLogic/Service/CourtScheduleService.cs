using BusinessLogic.Interface;
using DataAccess.DAO;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Service
{
    public class CourtScheduleService : ICourtScheduleService
    {
        private readonly ICourtScheduleRepository _courtScheduleRepository;

        public CourtScheduleService(ICourtScheduleRepository courtScheduleRepository)
        {
            _courtScheduleRepository = courtScheduleRepository;
        }

        public async Task<IEnumerable<CourtSchedule>> GetListAllSchedulesAsync()
        {
            return await _courtScheduleRepository.GetAllSchedulesAsync();
        }
        
        public async Task<IEnumerable<CourtSchedule>> GetAllSchedulesAllCourtNameAsync()
        {
            return await _courtScheduleRepository.GetAllSchedulesAllCourtNameAsync();
        }

        public async Task<CourtSchedule> GetScheduleByIdAsync(int scheduleId)
        {
            return await _courtScheduleRepository.GetScheduleByIdAsync(scheduleId);
        }
        
        public async Task<List<CourtSchedule>> GetScheduleByCourtIdAsync(int courtId)
        {
            return await _courtScheduleRepository.GetScheduleByCourtIdAsync(courtId);
        }

        public async Task AddScheduleAsync(CourtSchedule schedule)
        {
            await _courtScheduleRepository.AddScheduleAsync(schedule);
        }

        public async Task UpdateScheduleAsync(CourtSchedule schedule)
        {
            await _courtScheduleRepository.UpdateScheduleAsync(schedule);
        }

        public async Task DeleteScheduleAsync(int scheduleId)
        {
            await _courtScheduleRepository.DeleteScheduleAsync(scheduleId);
        }

        public async Task<List<CourtSchedule>> GetSchedulesByCourtIdAsync(int courtId, DateOnly date)
        {
            return await _courtScheduleRepository.GetSchedulesByCourtIdAsync(courtId, date);
        }

        public async Task<List<CourtSchedule>> GetAvailableSchedulesAsync(int courtId, DateOnly date)
        {
            return await _courtScheduleRepository.GetAvailableSchedulesAsync(courtId, date);
        }

        public async Task MarkScheduleAsUnavailableAsync(int scheduleId)
        {
            var schedule = await _courtScheduleRepository.GetScheduleByIdAsync(scheduleId);
            if (schedule != null && schedule.IsAvailable == true)
            {
                await _courtScheduleRepository.MarkScheduleAsUnavailableAsync(scheduleId);
            }
        }

        public async Task<(int availableCount, int bookedCount)> GetAvailabilityStatisticsAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _courtScheduleRepository.GetAvailabilityStatisticsAsync(startDate, endDate);
        }


    }
}

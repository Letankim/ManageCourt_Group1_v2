using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface ICourtScheduleRepository
    {
        Task<List<CourtSchedule>> GetAllSchedulesAsync();
        Task<CourtSchedule> GetScheduleByIdAsync(int scheduleId);
        Task AddScheduleAsync(CourtSchedule schedule);
        Task UpdateScheduleAsync(CourtSchedule schedule);
        Task DeleteScheduleAsync(int scheduleId);
    }
}

using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface ICourtScheduleService
    {
        Task<IEnumerable<CourtSchedule>> GetListAllSchedulesAsync();
        Task<CourtSchedule> GetScheduleByIdAsync(int id);
        Task AddScheduleAsync(CourtSchedule item);
        Task UpdateScheduleAsync(CourtSchedule item);
        Task DeleteScheduleAsync(int id);
    }
}

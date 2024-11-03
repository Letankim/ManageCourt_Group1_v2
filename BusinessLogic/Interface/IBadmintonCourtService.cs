using DataAccess.DAO;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IBadmintonCourtService
    {
        Task<List<BadmintonCourt>> GetListAllCourtsAsync();
        Task<BadmintonCourt> GetCourtByIdAsync(int id);
        Task AddCourtAsync(BadmintonCourt item);
        Task UpdateCourtAsync(BadmintonCourt item);
        Task DeleteCourtAsync(int id);
        Task<List<BadmintonCourt>> GetAllEnabledCourtsAsync(int page, int pageSize);
        Task<int> GetCourtsCountAsync();

        Task<(List<BadmintonCourt>, int)> GetFilteredCourtsAsync(
            int page, int pageSize, decimal? minPrice, decimal? maxPrice, TimeOnly? openTime, TimeOnly? closeTime, string search);

        Task<BadmintonCourt> GetCourtByIdActiveAsync(int courtId);
        Task<List<BadmintonCourt>> GetCourtsByOwnerIdAsync(int ownerId);

    }
}

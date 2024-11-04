using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IBadmintonCourtRepository
    {
        Task<List<BadmintonCourt>> GetAllCourtsAsync();
        Task<BadmintonCourt> GetCourtByIdAsync(int courtId);
        Task AddCourtAsync(BadmintonCourt court);
        Task UpdateCourtAsync(BadmintonCourt court);
        Task DeleteCourtAsync(int courtId);
        Task<List<BadmintonCourt>> GetAllEnabledCourtsAsync(int page, int pageSize);
        Task<int> GetCourtsCountAsync();

        Task<(List<BadmintonCourt>, int)> GetFilteredCourtsAsync(
            int page, int pageSize, decimal? minPrice, decimal? maxPrice, TimeOnly? openTime, TimeOnly? closeTime, string search);
        Task<BadmintonCourt> GetCourtByIdActiveAsync(int courtId);

        Task<List<BadmintonCourt>> GetCourtsByOwnerIdAsync(int ownerId);
     }
}

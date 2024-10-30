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
    }
}

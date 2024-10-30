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
    }
}

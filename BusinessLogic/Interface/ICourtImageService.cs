using DataAccess.DAO;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface ICourtImageService
    {
        Task<IEnumerable<CourtImage>> GetListAllImagesAsync();
        Task<CourtImage> GetImageByIdAsync(int id);
        Task AddImageAsync(CourtImage item);
        Task UpdateImageAsync(CourtImage item);
        Task DeleteImageAsync(int id);

        Task<List<CourtImage>> GetImagesByCourtIdAsync(int courtId);

        Task DeleteImagesByCourtIdAsync(int courtId);
    }
}

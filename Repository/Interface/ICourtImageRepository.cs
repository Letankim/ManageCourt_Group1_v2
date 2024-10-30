using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface ICourtImageRepository
    {
        Task<List<CourtImage>> GetAllImagesAsync();
        Task<CourtImage> GetImageByIdAsync(int imageId);
        Task AddImageAsync(CourtImage image);
        Task UpdateImageAsync(CourtImage image);
        Task DeleteImageAsync(int imageId);
    }
}

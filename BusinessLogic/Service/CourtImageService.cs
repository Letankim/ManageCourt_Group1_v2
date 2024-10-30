using BusinessLogic.Interface;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BusinessLogic.Service
{
    public class CourtImageService : ICourtImageService
    {
        private readonly ICourtImageRepository _courtImageRepository;

        public CourtImageService(ICourtImageRepository courtImageRepository)
        {
            _courtImageRepository = courtImageRepository;
        }

        public async Task<IEnumerable<CourtImage>> GetListAllImagesAsync()
        {
            return await _courtImageRepository.GetAllImagesAsync();
        }
        public async Task<CourtImage> GetImageByIdAsync(int id)
        {
            return await _courtImageRepository.GetImageByIdAsync(id);
        }

        public async Task AddImageAsync(CourtImage item)
        {
            await _courtImageRepository.AddImageAsync(item);
        }

        public async Task UpdateImageAsync(CourtImage item)
        {
            await _courtImageRepository.UpdateImageAsync(item);
        }

        public async Task DeleteImageAsync(int id)
        {
            await _courtImageRepository.DeleteImageAsync(id);
        }
    }
}

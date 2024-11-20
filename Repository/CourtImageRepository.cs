using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class CourtImageRepository : ICourtImageRepository
    {
        private readonly CourtImageDAO _courtImageDAO;

        public CourtImageRepository(CourtImageDAO courtImageDAO)
        {
            _courtImageDAO = courtImageDAO;
        }

        public async Task<List<CourtImage>> GetAllImagesAsync()
        {
            return await _courtImageDAO.GetAllImagesAsync();
        }

        public async Task<CourtImage> GetImageByIdAsync(int imageId)
        {
            return await _courtImageDAO.GetImageByIdAsync(imageId);
        }

        public async Task AddImageAsync(CourtImage image)
        {
            await _courtImageDAO.AddImageAsync(image);
        }

        public async Task UpdateImageAsync(CourtImage image)
        {
            await _courtImageDAO.UpdateImageAsync(image);
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _courtImageDAO.GetImageByIdAsync(imageId);
            if (image != null)
            {
                await _courtImageDAO.DeleteImageAsync(imageId);
            }
        }

        public async Task<List<CourtImage>> GetImagesByCourtIdAsync(int courtId)
        {
            return await _courtImageDAO.GetImagesByCourtIdAsync(courtId);
        }

        public async Task DeleteImagesByCourtIdAsync(int courtId)
        {
            await _courtImageDAO.DeleteImagesByCourtIdAsync(courtId);
        }


    }
}

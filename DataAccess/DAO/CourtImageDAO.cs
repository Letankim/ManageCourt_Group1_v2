using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CourtImageDAO : SingletonBase<CourtImage>
    {
        public async Task<List<CourtImage>> GetAllImagesAsync()
        {
            return await _context.CourtImages.ToListAsync();
        }

        public async Task<CourtImage> GetImageByIdAsync(int imageId)
        {
            return await _context.CourtImages.FindAsync(imageId);
        }

        public async Task AddImageAsync(CourtImage image)
        {
            await _context.CourtImages.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateImageAsync(CourtImage image)
        {
            _context.CourtImages.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _context.CourtImages.FindAsync(imageId);
            if (image != null)
            {
                _context.CourtImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}

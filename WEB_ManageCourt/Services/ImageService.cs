using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WEB_ManageCourt.Services
{
    public class ImageService
    {
        public async Task<string> SaveImageAsync(IFormFile image)
        {
            var directoryPath = Path.Combine("wwwroot/images/courtImages");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var filePath = Path.Combine(directoryPath, image.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return "/images/courtImages/" + image.FileName;
        }
    }
}

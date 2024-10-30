using Microsoft.EntityFrameworkCore;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class AccessoryDAO : SingletonBase<Accessory>
    {
        public async Task<List<Accessory>> GetAllAccessoriesAsync()
        {
            return await _context.Accessories.ToListAsync();
        }

        public async Task<Accessory> GetAccessoryByIdAsync(int accessoryId)
        {
            return await _context.Accessories.FindAsync(accessoryId);
        }

        public async Task AddAccessoryAsync(Accessory accessory)
        {
            await _context.Accessories.AddAsync(accessory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccessoryAsync(Accessory accessory)
        {
            _context.Accessories.Update(accessory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccessoryAsync(int accessoryId)
        {
            var accessory = await _context.Accessories.FindAsync(accessoryId);
            if (accessory != null)
            {
                _context.Accessories.Remove(accessory);
                await _context.SaveChangesAsync();
            }
        }
    }
}

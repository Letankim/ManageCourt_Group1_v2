using DataAccess.DAO;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class AccessoryRepository : IAccessoryRepository
    {
        private readonly AccessoryDAO _accessoryDAO;

        public AccessoryRepository(AccessoryDAO accessoryDAO)
        {
            _accessoryDAO = accessoryDAO;
        }

        public async Task<List<Accessory>> GetAllAccessoriesAsync()
        {
            return await _accessoryDAO.GetAllAccessoriesAsync();
        }

        public async Task<Accessory> GetAccessoryByIdAsync(int accessoryId)
        {
            return await _accessoryDAO.GetAccessoryByIdAsync(accessoryId);
        }

        public async Task AddAccessoryAsync(Accessory accessory)
        {
            await _accessoryDAO.AddAccessoryAsync(accessory);
        }

        public async Task UpdateAccessoryAsync(Accessory accessory)
        {
            await _accessoryDAO.UpdateAccessoryAsync(accessory);
        }

        public async Task DeleteAccessoryAsync(int accessoryId)
        {
            var accessory = await _accessoryDAO.GetAccessoryByIdAsync(accessoryId);
            if (accessory != null)
            {
                await _accessoryDAO.DeleteAccessoryAsync(accessoryId);
            }
        }
    }
}

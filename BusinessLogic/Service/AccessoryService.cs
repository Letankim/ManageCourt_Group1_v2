using BusinessLogic.Interface;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Service
{
    public class AccessoryService : IAccessoryService
    {
        private readonly IAccessoryRepository _accessoryRepository;

        public AccessoryService(IAccessoryRepository accessoryRepository)
        {
            _accessoryRepository = accessoryRepository;
        }

        public async Task<IEnumerable<Accessory>> GetListAllAccessoriesAsync()
        {
            return await _accessoryRepository.GetAllAccessoriesAsync();
        }

        public async Task<Accessory> GetAccessoryByIdAsync(int accessoryId)
        {
            return await _accessoryRepository.GetAccessoryByIdAsync(accessoryId);
        }

        public async Task AddAccessoryAsync(Accessory accessory)
        {
            await _accessoryRepository.AddAccessoryAsync(accessory);
        }

        public async Task UpdateAccessoryAsync(Accessory accessory)
        {
            await _accessoryRepository.UpdateAccessoryAsync(accessory);
        }

        public async Task DeleteAccessoryAsync(int accessoryId)
        {
            await _accessoryRepository.DeleteAccessoryAsync(accessoryId);
        }
    }
}

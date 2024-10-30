using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IAccessoryRepository
    {
        Task<List<Accessory>> GetAllAccessoriesAsync();
        Task<Accessory> GetAccessoryByIdAsync(int accessoryId);
        Task AddAccessoryAsync(Accessory accessory);
        Task UpdateAccessoryAsync(Accessory accessory);
        Task DeleteAccessoryAsync(int accessoryId);
    }
}

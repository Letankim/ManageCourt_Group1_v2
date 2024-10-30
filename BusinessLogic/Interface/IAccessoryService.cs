using Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IAccessoryService
    {
        Task<IEnumerable<Accessory>> GetListAllAccessoriesAsync();
        Task<Accessory> GetAccessoryByIdAsync(int id);
        Task AddAccessoryAsync(Accessory item);
        Task UpdateAccessoryAsync(Accessory item);
        Task DeleteAccessoryAsync(int id);
    }
}

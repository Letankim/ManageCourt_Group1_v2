using BusinessLogic.Interface;
using DataAccess.DAO;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Service
{
    public class BadmintonCourtService : IBadmintonCourtService
    {
        private readonly IBadmintonCourtRepository _badmintonCourtRepository;

        public BadmintonCourtService(IBadmintonCourtRepository badmintonCourtRepository)
        {
            _badmintonCourtRepository = badmintonCourtRepository;
        }

        public async Task<BadmintonCourt> GetCourtByIdAsync(int courtId)
        {
            return await _badmintonCourtRepository.GetCourtByIdAsync(courtId);
        }

        public async Task AddCourtAsync(BadmintonCourt court)
        {
            await _badmintonCourtRepository.AddCourtAsync(court);
        }

        public async Task UpdateCourtAsync(BadmintonCourt court)
        {
            await _badmintonCourtRepository.UpdateCourtAsync(court);
        }

        public async Task DeleteCourtAsync(int courtId)
        {
            await _badmintonCourtRepository.DeleteCourtAsync(courtId);
        }

        public async Task<List<BadmintonCourt>> GetListAllCourtsAsync()
        {
            return await _badmintonCourtRepository.GetAllCourtsAsync();
        }

        public async Task<List<BadmintonCourt>> GetAllEnabledCourtsAsync(int page, int pageSize)
        {
            return await _badmintonCourtRepository.GetAllEnabledCourtsAsync(page, pageSize);
        }

        public async Task<int> GetCourtsCountAsync()
        {
            return await _badmintonCourtRepository.GetCourtsCountAsync();
        }

        public async Task<(List<BadmintonCourt>, int)> GetFilteredCourtsAsync(
    int page, int pageSize, decimal? minPrice, decimal? maxPrice, TimeOnly? openTime, TimeOnly? closeTime, string search)
        {
            return await _badmintonCourtRepository.GetFilteredCourtsAsync(page, pageSize, minPrice, maxPrice, openTime, closeTime, search);
        }
        public async Task<BadmintonCourt> GetCourtByIdActiveAsync(int courtId)
        {
            return await _badmintonCourtRepository.GetCourtByIdAsync(courtId);
        }

        public async Task<List<BadmintonCourt>> GetCourtsByOwnerIdAsync(int ownerId)
        {
            return await _badmintonCourtRepository.GetCourtsByOwnerIdAsync((int)ownerId);
        }
    }
}

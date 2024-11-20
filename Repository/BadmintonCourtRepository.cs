using DataAccess.DAO;
using Model;
using Repositories.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class BadmintonCourtRepository : IBadmintonCourtRepository
    {
        private readonly BadmintonCourtDAO _badmintonCourtDAO;

        public BadmintonCourtRepository(BadmintonCourtDAO badmintonCourtDAO)
        {
            _badmintonCourtDAO = badmintonCourtDAO;
        }

        public async Task<List<BadmintonCourt>> GetAllCourtsAsync()
        {
            return await _badmintonCourtDAO.GetAllCourtsAsync();
        }

        public async Task<BadmintonCourt> GetCourtByIdAsync(int courtId)
        {
            return await _badmintonCourtDAO.GetCourtByIdAsync(courtId);
        }

        public async Task AddCourtAsync(BadmintonCourt court)
        {
            await _badmintonCourtDAO.AddCourtAsync(court);
        }

        public async Task UpdateCourtAsync(BadmintonCourt court)
        {
            await _badmintonCourtDAO.UpdateCourtAsync(court);
        }

        public async Task DeleteCourtAsync(int courtId)
        {
            var court = await _badmintonCourtDAO.GetCourtByIdAsync(courtId);
            if (court != null)
            {
                await _badmintonCourtDAO.DeleteCourtAsync(courtId);
            }
        }

        public async Task<List<BadmintonCourt>> GetAllEnabledCourtsAsync(int page, int pageSize)
        {
            return await _badmintonCourtDAO.GetAllEnabledCourtsAsync(page, pageSize);
        }

        public async Task<int> GetCourtsCountAsync()
        {
            return await _badmintonCourtDAO.GetCourtsCountAsync();
        }

        public async Task<(List<BadmintonCourt>, int)> GetFilteredCourtsAsync(
            int page, int pageSize, decimal? minPrice, decimal? maxPrice, TimeOnly? openTime, TimeOnly? closeTime, string search)
        {
            return await _badmintonCourtDAO.GetFilteredCourtsAsync(page, pageSize, minPrice, maxPrice, openTime, closeTime, search);
        }
        public async Task<BadmintonCourt> GetCourtByIdActiveAsync(int courtId)
        {
            return await _badmintonCourtDAO.GetCourtByIdAsync(courtId);
        }

        public async Task<List<BadmintonCourt>> GetCourtsByOwnerIdAsync(int ownerId)
        {
            return await _badmintonCourtDAO.GetCourtsByOwnerIdAsync((int)ownerId);
        }
    }
}

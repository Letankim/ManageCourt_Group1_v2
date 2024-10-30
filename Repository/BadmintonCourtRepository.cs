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
    }
}

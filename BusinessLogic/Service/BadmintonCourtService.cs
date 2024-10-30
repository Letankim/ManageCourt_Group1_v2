using BusinessLogic.Interface;
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
    }
}

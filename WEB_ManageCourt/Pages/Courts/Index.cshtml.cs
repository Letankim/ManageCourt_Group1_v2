using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Interface;

namespace WEB_ManageCourt.Pages.Courts
{
    public class IndexModel : PageModel
    {
        private readonly IBadmintonCourtService _badmintonCourtService;

        public IndexModel(IBadmintonCourtService badmintonCourtService)
        {
            _badmintonCourtService = badmintonCourtService;
        }

        public List<BadmintonCourt> Courts { get; set; } = new List<BadmintonCourt>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public TimeOnly? OpenTime { get; set; }

        [BindProperty(SupportsGet = true)]
        public TimeOnly? CloseTime { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        public async Task OnGetAsync(int page = 1)
        {
            int pageSize = 6;
            CurrentPage = page;

            (Courts, int totalCourts) = await _badmintonCourtService.GetFilteredCourtsAsync(page, pageSize, MinPrice, MaxPrice, OpenTime, CloseTime, Search);
            TotalPages = (int)Math.Ceiling((double)totalCourts / pageSize);
        }

    }
}

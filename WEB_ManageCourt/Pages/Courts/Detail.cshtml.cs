using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model;
using BusinessLogic.Interface;
using Microsoft.IdentityModel.Tokens;

namespace WEB_ManageCourt.Pages.Courts
{
    public class DetailModel : PageModel
    {
        private readonly IBadmintonCourtService _courtService;

        public DetailModel(IBadmintonCourtService courtService)
        {
            _courtService = courtService;
        }

        [BindProperty(SupportsGet = true)]
        public int CourtId { get; set; }

        public string CourtName { get; set; }
        public string Location { get; set; }
        public decimal PricePerHour { get; set; }
        public string Status { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Description { get; set; }
        public string EmbedMapsLink { get; set; }
        public string StandardMapsLink { get; set; }
        public List<CourtImage> CourtImages { get; set; }
        public bool IsClosed { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var court = await _courtService.GetCourtByIdActiveAsync(id);

            if (court == null)
            {
                return NotFound();
            }
            CourtId = court.CourtId;
            CourtName = court.CourtName;
            Location = court.Location;
            PricePerHour = court.PricePerHour;
            StartTime = court.StartTime;
            EndTime = court.EndTime;
            Description = court.Description;
            string baseMapsLink = court.MapsLink != null && !string.IsNullOrEmpty(court.MapsLink)
               ? court.MapsLink
               : "https://maps.google.com";
            EmbedMapsLink = baseMapsLink.Contains("embed") ? baseMapsLink : baseMapsLink.Replace("/maps", "/maps/embed");
            StandardMapsLink = baseMapsLink.Contains("embed") ? baseMapsLink.Replace("/maps/embed", "/maps/dir/") : baseMapsLink;
            CourtImages = court.CourtImages != null ? new List<CourtImage>(court.CourtImages) : new List<CourtImage>();
            Status = (court.IsEnabled ?? false) ? "Đang hoạt động" : "Tạm đóng";
            IsClosed = DateTime.Now.TimeOfDay < StartTime.ToTimeSpan() || DateTime.Now.TimeOfDay > EndTime.ToTimeSpan();
            return Page();
        }
    }
}

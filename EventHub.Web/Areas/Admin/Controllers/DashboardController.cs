using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Web.Models.Admin;
using EventHub.Web.Models.Event;

namespace EventHub.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;
        private readonly IVenueService _venueService;
        private readonly ITicketService _ticketService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            IEventService eventService,
            ICategoryService categoryService,
            IVenueService venueService,
            ITicketService ticketService,
            UserManager<ApplicationUser> userManager)
        {
            _eventService = eventService;
            _categoryService = categoryService;
            _venueService = venueService;
            _ticketService = ticketService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // TODO: this could be optimized with a single stored procedure or a stats view
            var events = await _eventService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var venues = await _venueService.GetAllAsync();
            var totalTickets = await _ticketService.GetTotalTicketsSoldAsync();
            var totalUsers = _userManager.Users.Count();

            var model = new DashboardViewModel
            {
                TotalEvents = events.Count(),
                TotalUsers = totalUsers,
                TotalTicketsSold = totalTickets,
                TotalCategories = categories.Count(),
                TotalVenues = venues.Count(),
                TotalRevenue = events.Sum(e => e.TicketPrice * (e.Tickets?.Count ?? 0)),
                RecentEvents = events.Take(10).Select(e => new RecentEventViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    StartDate = e.StartDate,
                    CategoryName = e.Category.Name,
                    AvailableTickets = e.AvailableTickets,
                    IsActive = e.IsActive
                })
            };

            return View(model);
        }

        // manage events from admin panel
        public async Task<IActionResult> Events()
        {
            var events = await _eventService.GetAllAsync();

            var model = events.Select(e => new EventItemViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description.Length > 100
                    ? e.Description.Substring(0, 100) + "..."
                    : e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                TicketPrice = e.TicketPrice,
                AvailableTickets = e.AvailableTickets,
                CategoryName = e.Category.Name,
                VenueName = e.Venue.Name,
                City = e.Venue.City
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEvent(int id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            // flip the active status
            ev.IsActive = !ev.IsActive;
            await _eventService.UpdateAsync(ev);

            TempData["Success"] = ev.IsActive
                ? "Event activated."
                : "Event deactivated.";

            return RedirectToAction(nameof(Events));
        }
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EventHub.Core.Interfaces;
using EventHub.Web.Models;
using EventHub.Web.Models.Home;

namespace EventHub.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;
        private readonly IVenueService _venueService;

        public HomeController(
            ILogger<HomeController> logger,
            IEventService eventService,
            ICategoryService categoryService,
            IVenueService venueService)
        {
            _logger = logger;
            _eventService = eventService;
            _categoryService = categoryService;
            _venueService = venueService;
        }

        public async Task<IActionResult> Index()
        {
            // TODO: consider caching the homepage data since it doesn't change that often
            var upcomingEvents = await _eventService.GetUpcomingEventsAsync(6);
            var categories = await _categoryService.GetAllAsync();
            var venues = await _venueService.GetAllAsync();

            var model = new HomeViewModel
            {
                TotalEvents = await _eventService.GetTotalCountAsync(),
                TotalCategories = categories.Count(),
                TotalVenues = venues.Count(),
                UpcomingEvents = upcomingEvents.Select(e => new EventCardViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    ImageUrl = e.ImageUrl,
                    StartDate = e.StartDate,
                    TicketPrice = e.TicketPrice,
                    CategoryName = e.Category.Name,
                    VenueName = e.Venue.Name,
                    City = e.Venue.City
                })
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

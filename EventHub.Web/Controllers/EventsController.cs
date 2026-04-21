using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Web.Models.Event;

namespace EventHub.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICategoryService _categoryService;
        private readonly IVenueService _venueService;
        private readonly IReviewService _reviewService;
        private readonly IFavoriteService _favoriteService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        private const int PageSize = 6;

        public EventsController(
            IEventService eventService,
            ICategoryService categoryService,
            IVenueService venueService,
            IReviewService reviewService,
            IFavoriteService favoriteService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _eventService = eventService;
            _categoryService = categoryService;
            _venueService = venueService;
            _reviewService = reviewService;
            _favoriteService = favoriteService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int page = 1)
        {
            var (events, totalCount) = await _eventService.GetPagedAsync(searchTerm, categoryId, page, PageSize);
            var categories = await _categoryService.GetAllAsync();

            // AutoMapper handles field projection; IsFavorited is set below for logged-in users
            var mappedEvents = _mapper.Map<IEnumerable<EventItemViewModel>>(events).ToList();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User)!;
                var favoriteIds = (await _favoriteService.GetFavoriteEventIdsAsync(userId)).ToHashSet();
                foreach (var ev in mappedEvents)
                    ev.IsFavorited = favoriteIds.Contains(ev.Id);
            }

            var model = new EventListViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                CurrentPage = page,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
                Categories = categories.Select(c => new CategoryOptionViewModel { Id = c.Id, Name = c.Name }),
                Events = mappedEvents
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ev = await _eventService.GetByIdWithDetailsAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            var avgRating = await _reviewService.GetAverageRatingForEventAsync(id);

            var model = _mapper.Map<EventDetailsViewModel>(ev);
            model.AverageRating = avgRating;
            model.Reviews = _mapper.Map<IEnumerable<ReviewItemViewModel>>(ev.Reviews);

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var model = new EventCreateViewModel();
            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            // basic date validation
            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
                await PopulateDropdowns(model);
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            var eventEntity = new Event
            {
                Title = model.Title,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                TicketPrice = model.TicketPrice,
                AvailableTickets = model.AvailableTickets,
                CategoryId = model.CategoryId,
                VenueId = model.VenueId,
                OrganizerId = userId!
            };

            await _eventService.CreateAsync(eventEntity);
            TempData["Success"] = "Event created successfully!";

            return RedirectToAction(nameof(Details), new { id = eventEntity.Id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            // only the organizer or an admin can edit
            var userId = _userManager.GetUserId(User);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var model = new EventEditViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                ImageUrl = ev.ImageUrl,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                TicketPrice = ev.TicketPrice,
                AvailableTickets = ev.AvailableTickets,
                CategoryId = ev.CategoryId,
                VenueId = ev.VenueId
            };

            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var ev = await _eventService.GetByIdAsync(model.Id);
            if (ev == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // TODO: add a check to prevent editing events that already have sold tickets
            ev.Title = model.Title;
            ev.Description = model.Description;
            ev.ImageUrl = model.ImageUrl;
            ev.StartDate = model.StartDate;
            ev.EndDate = model.EndDate;
            ev.TicketPrice = model.TicketPrice;
            ev.AvailableTickets = model.AvailableTickets;
            ev.CategoryId = model.CategoryId;
            ev.VenueId = model.VenueId;

            await _eventService.UpdateAsync(ev);
            TempData["Success"] = "Event updated successfully!";

            return RedirectToAction(nameof(Details), new { id = ev.Id });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _eventService.GetByIdWithDetailsAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var model = _mapper.Map<EventItemViewModel>(ev);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            await _eventService.DeleteAsync(id);
            TempData["Success"] = "Event deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // pulls categories and venues for the create/edit dropdowns
        private async Task PopulateDropdowns(EventCreateViewModel model)
        {
            var categories = await _categoryService.GetAllAsync();
            var venues = await _venueService.GetAllAsync();

            model.Categories = categories.Select(c => new CategoryOptionViewModel
            {
                Id = c.Id,
                Name = c.Name
            });

            model.Venues = venues.Select(v => new VenueOptionViewModel
            {
                Id = v.Id,
                Name = v.Name
            });
        }

        /// <summary>Toggle favorite status for the current user (AJAX-friendly).</summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            await _favoriteService.ToggleFavoriteAsync(id, userId);
            var isFav = await _favoriteService.IsFavoriteAsync(id, userId);
            return Json(new { isFavorited = isFav });
        }

        [Authorize]
        public async Task<IActionResult> Favorites()
        {
            var userId = _userManager.GetUserId(User)!;
            var favoriteIds = (await _favoriteService.GetFavoriteEventIdsAsync(userId)).ToHashSet();

            var allEvents = await _eventService.GetAllAsync();
            var favoriteEvents = allEvents.Where(e => favoriteIds.Contains(e.Id));

            var model = _mapper.Map<IEnumerable<EventItemViewModel>>(favoriteEvents).ToList();

            // every event on this page is already favorited
            foreach (var ev in model)
                ev.IsFavorited = true;

            return View(model);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Web.Models.Review;

namespace EventHub.Web.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IEventService _eventService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(
            IReviewService reviewService,
            IEventService eventService,
            UserManager<ApplicationUser> userManager)
        {
            _reviewService = reviewService;
            _eventService = eventService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create(int eventId)
        {
            var ev = await _eventService.GetByIdAsync(eventId);
            if (ev == null)
            {
                return NotFound();
            }

            var model = new CreateReviewViewModel
            {
                EventId = ev.Id,
                EventTitle = ev.Title
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var ev = await _eventService.GetByIdAsync(model.EventId);
                model.EventTitle = ev?.Title ?? "Unknown Event";
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;

            var review = new Core.Entities.Review
            {
                EventId = model.EventId,
                UserId = userId,
                Content = model.Content,
                Rating = model.Rating
            };

            await _reviewService.CreateAsync(review);
            TempData["Success"] = "Review submitted!";

            return RedirectToAction("Details", "Events", new { id = model.EventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            var userId = _userManager.GetUserId(User)!;

            try
            {
                await _reviewService.DeleteAsync(id, userId);
                TempData["Success"] = "Review deleted.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", "Events", new { id = eventId });
        }
    }
}

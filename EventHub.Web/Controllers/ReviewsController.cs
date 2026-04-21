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

        /// <summary>AJAX endpoint — returns JSON so the Details page can prepend the new review without a full reload.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax([FromBody] CreateReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, errors });
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

            // load user display name for the returned card
            var user = await _userManager.FindByIdAsync(userId);
            var userName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown";

            return Ok(new
            {
                success = true,
                review = new
                {
                    id = review.Id,
                    content = review.Content,
                    rating = review.Rating,
                    createdOn = review.CreatedOn.ToString("MMM dd, yyyy"),
                    userName,
                    userId
                }
            });
        }
    }
}

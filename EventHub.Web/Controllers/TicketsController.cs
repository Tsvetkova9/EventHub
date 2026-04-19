using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Web.Models.Ticket;

namespace EventHub.Web.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IEventService _eventService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketsController(
            ITicketService ticketService,
            IEventService eventService,
            UserManager<ApplicationUser> userManager)
        {
            _ticketService = ticketService;
            _eventService = eventService;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyTickets()
        {
            var userId = _userManager.GetUserId(User)!;
            var tickets = await _ticketService.GetTicketsByUserAsync(userId);

            var model = new TicketListViewModel
            {
                Tickets = tickets.Select(t => new TicketItemViewModel
                {
                    Id = t.Id,
                    EventTitle = t.Event.Title,
                    VenueName = t.Event.Venue.Name,
                    EventDate = t.Event.StartDate,
                    Quantity = t.Quantity,
                    TotalPrice = t.TotalPrice,
                    Status = t.Status.ToString(),
                    PurchasedOn = t.PurchasedOn
                })
            };

            return View(model);
        }

        public async Task<IActionResult> Purchase(int eventId)
        {
            var ev = await _eventService.GetByIdAsync(eventId);
            if (ev == null)
            {
                return NotFound();
            }

            var model = new PurchaseTicketViewModel
            {
                EventId = ev.Id,
                EventTitle = ev.Title,
                TicketPrice = ev.TicketPrice,
                AvailableTickets = ev.AvailableTickets
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(PurchaseTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // refetch event info since it's not posted back
                var ev = await _eventService.GetByIdAsync(model.EventId);
                if (ev != null)
                {
                    model.EventTitle = ev.Title;
                    model.TicketPrice = ev.TicketPrice;
                    model.AvailableTickets = ev.AvailableTickets;
                }
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;

            try
            {
                await _ticketService.PurchaseTicketAsync(model.EventId, userId, model.Quantity);
                TempData["Success"] = "Ticket purchased successfully!";
                return RedirectToAction(nameof(MyTickets));
            }
            catch (InvalidOperationException ex)
            {
                // the service handles all validation, we just display the error
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", "Events", new { id = model.EventId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User)!;

            try
            {
                // TODO: maybe send a cancellation confirmation email here
                await _ticketService.CancelTicketAsync(id, userId);
                TempData["Success"] = "Ticket cancelled successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(MyTickets));
        }
    }
}

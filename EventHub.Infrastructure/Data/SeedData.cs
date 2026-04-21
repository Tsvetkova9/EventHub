using EventHub.Core.Entities;
using EventHub.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventHub.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await SeedRolesAsync(roleManager);
            var (adminUser, regularUser) = await SeedUsersAsync(userManager);
            await SeedCategoriesAsync(context);
            await SeedVenuesAsync(context);
            await SeedEventsAsync(context, adminUser.Id);
            await SeedTicketsAsync(context, regularUser.Id);
            await SeedReviewsAsync(context, regularUser.Id);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task<(ApplicationUser Admin, ApplicationUser User)> SeedUsersAsync(
            UserManager<ApplicationUser> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@eventhub.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@eventhub.com",
                    Email = "admin@eventhub.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    CreatedOn = DateTime.UtcNow
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var regularUser = await userManager.FindByEmailAsync("user@eventhub.com");
            if (regularUser == null)
            {
                regularUser = new ApplicationUser
                {
                    UserName = "user@eventhub.com",
                    Email = "user@eventhub.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true,
                    CreatedOn = DateTime.UtcNow
                };
                await userManager.CreateAsync(regularUser, "User@123");
                await userManager.AddToRoleAsync(regularUser, "User");
            }

            return (adminUser, regularUser);
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync())
                return;

            var categories = new List<Category>
            {
                new() { Name = "Music", Description = "Live concerts, festivals, and music performances", ImageUrl = "https://picsum.photos/seed/musiccat/600/400" },
                new() { Name = "Technology", Description = "Tech conferences, hackathons, and workshops", ImageUrl = "https://picsum.photos/seed/techcat/600/400" },
                new() { Name = "Sports", Description = "Sporting events, tournaments, and competitions", ImageUrl = "https://picsum.photos/seed/sportscat/600/400" },
                new() { Name = "Arts & Culture", Description = "Art exhibitions, theater, and cultural festivals", ImageUrl = "https://picsum.photos/seed/artscat/600/400" },
                new() { Name = "Food & Drink", Description = "Food festivals, wine tastings, and culinary events", ImageUrl = "https://picsum.photos/seed/foodcat/600/400" },
                new() { Name = "Business", Description = "Networking events, seminars, and business conferences", ImageUrl = "https://picsum.photos/seed/businesscat/600/400" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedVenuesAsync(ApplicationDbContext context)
        {
            if (await context.Venues.AnyAsync())
                return;

            var venues = new List<Venue>
            {
                new() { Name = "Grand Arena", Address = "123 Main Street", City = "Sofia", Country = "Bulgaria", Capacity = 5000, ImageUrl = "/images/venues/arena.jpg" },
                new() { Name = "Tech Hub Conference Center", Address = "456 Innovation Blvd", City = "Plovdiv", Country = "Bulgaria", Capacity = 500, ImageUrl = "/images/venues/techhub.jpg" },
                new() { Name = "Riverside Park", Address = "789 River Road", City = "Varna", Country = "Bulgaria", Capacity = 10000, ImageUrl = "/images/venues/riverside.jpg" },
                new() { Name = "City Gallery", Address = "321 Art Lane", City = "Burgas", Country = "Bulgaria", Capacity = 200, ImageUrl = "/images/venues/gallery.jpg" },
                new() { Name = "Central Stadium", Address = "555 Sports Ave", City = "Sofia", Country = "Bulgaria", Capacity = 30000, ImageUrl = "/images/venues/stadium.jpg" }
            };

            context.Venues.AddRange(venues);
            await context.SaveChangesAsync();
        }

        private static async Task SeedEventsAsync(ApplicationDbContext context, string organizerId)
        {
            if (await context.Events.AnyAsync())
                return;

            var events = new List<Event>
            {
                new()
                {
                    Title = "Summer Music Festival 2026",
                    Description = "A three-day outdoor music festival featuring international and local artists across multiple stages. Enjoy live performances, food stalls, and an unforgettable atmosphere.",
                    StartDate = new DateTime(2026, 7, 15, 18, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 7, 17, 23, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 75.00m,
                    AvailableTickets = 4500,
                    IsActive = true,
                    CategoryId = 1,
                    VenueId = 1,
                    OrganizerId = organizerId,
                    ImageUrl = "https://picsum.photos/seed/musicfestival/600/400"
                },
                new()
                {
                    Title = "DevConf 2026",
                    Description = "The premier software development conference in Bulgaria. Featuring talks on AI, cloud computing, web development, and DevOps from industry leaders.",
                    StartDate = new DateTime(2026, 9, 20, 9, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 9, 21, 18, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 120.00m,
                    AvailableTickets = 400,
                    IsActive = true,
                    CategoryId = 2,
                    VenueId = 2,
                    OrganizerId = organizerId,
                    ImageUrl = "https://picsum.photos/seed/devconf2026/600/400"
                },
                new()
                {
                    Title = "City Marathon 2026",
                    Description = "Annual city marathon with 5K, 10K, half-marathon, and full marathon categories. Open to all fitness levels with professional timing and support.",
                    StartDate = new DateTime(2026, 10, 5, 7, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 10, 5, 15, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 25.00m,
                    AvailableTickets = 8000,
                    IsActive = true,
                    CategoryId = 3,
                    VenueId = 3,
                    OrganizerId = organizerId,
                    ImageUrl = "https://picsum.photos/seed/citymarathon/600/400"
                },
                new()
                {
                    Title = "Contemporary Art Exhibition",
                    Description = "A curated exhibition of contemporary art from emerging Bulgarian artists. Features paintings, sculptures, and digital installations.",
                    StartDate = new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 8, 30, 20, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 15.00m,
                    AvailableTickets = 150,
                    IsActive = true,
                    CategoryId = 4,
                    VenueId = 4,
                    OrganizerId = organizerId,
                    ImageUrl = "https://picsum.photos/seed/artexhibition/600/400"
                },
                new()
                {
                    Title = "Street Food Festival",
                    Description = "Explore cuisines from around the world at the biggest street food festival in the region. Over 50 food vendors, live cooking demos, and family-friendly activities.",
                    StartDate = new DateTime(2026, 6, 12, 11, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 6, 14, 22, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 10.00m,
                    AvailableTickets = 9000,
                    IsActive = true,
                    CategoryId = 5,
                    VenueId = 3,
                    OrganizerId = organizerId,
                    ImageUrl = "https://picsum.photos/seed/streetfood/600/400"
                },
                new()
                {
                    Title = "Startup Networking Night",
                    Description = "An evening of networking for entrepreneurs, investors, and startup enthusiasts. Pitch competitions, panel discussions, and refreshments included.",
                    StartDate = new DateTime(2026, 11, 8, 18, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 11, 8, 22, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 30.00m,
                    AvailableTickets = 200,
                    IsActive = true,
                    CategoryId = 6,
                    VenueId = 2,
                    OrganizerId = organizerId,
                    ImageUrl = "https://picsum.photos/seed/startupnight/600/400"
                }
            };

            context.Events.AddRange(events);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTicketsAsync(ApplicationDbContext context, string userId)
        {
            if (await context.Tickets.AnyAsync())
                return;

            var tickets = new List<Ticket>
            {
                new()
                {
                    EventId = 1,
                    UserId = userId,
                    Quantity = 2,
                    TotalPrice = 150.00m,
                    Status = TicketStatus.Active,
                    PurchasedOn = DateTime.UtcNow
                },
                new()
                {
                    EventId = 2,
                    UserId = userId,
                    Quantity = 1,
                    TotalPrice = 120.00m,
                    Status = TicketStatus.Active,
                    PurchasedOn = DateTime.UtcNow
                }
            };

            context.Tickets.AddRange(tickets);
            await context.SaveChangesAsync();
        }

        private static async Task SeedReviewsAsync(ApplicationDbContext context, string userId)
        {
            if (await context.Reviews.AnyAsync())
                return;

            var reviews = new List<Review>
            {
                new()
                {
                    EventId = 1,
                    UserId = userId,
                    Rating = 5,
                    Content = "Amazing event! The lineup was incredible and the atmosphere was electric. Can't wait for next year!",
                    CreatedOn = DateTime.UtcNow
                },
                new()
                {
                    EventId = 2,
                    UserId = userId,
                    Rating = 4,
                    Content = "Great conference with very informative talks. The networking opportunities were excellent.",
                    CreatedOn = DateTime.UtcNow
                }
            };

            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();
        }
    }
}

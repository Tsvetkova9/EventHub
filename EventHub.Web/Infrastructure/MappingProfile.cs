using AutoMapper;
using EventHub.Core.Entities;
using EventHub.Web.Models.Admin;
using EventHub.Web.Models.Event;
using EventHub.Web.Models.Review;
using EventHub.Web.Models.Ticket;

namespace EventHub.Web.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Event -> EventItemViewModel
            CreateMap<Event, EventItemViewModel>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.Description.Length > 150 ? src.Description.Substring(0, 150) + "..." : src.Description))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => src.Venue.Name))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Venue.City))
                .ForMember(dest => dest.IsFavorited, opt => opt.Ignore()); // set in controller per authenticated user

            // Event -> EventDetailsViewModel
            // AverageRating and Reviews are set manually after mapping
            CreateMap<Event, EventDetailsViewModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => src.Venue.Name))
                .ForMember(dest => dest.VenueAddress, opt => opt.MapFrom(src => src.Venue.Address))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Venue.City))
                .ForMember(dest => dest.OrganizerName,
                    opt => opt.MapFrom(src => src.Organizer.FirstName + " " + src.Organizer.LastName))
                .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore());

            // Review -> ReviewItemViewModel
            CreateMap<Review, ReviewItemViewModel>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            // Ticket -> TicketItemViewModel
            CreateMap<Ticket, TicketItemViewModel>()
                .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title))
                .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => src.Event.Venue.Name))
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.Event.StartDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Event -> RecentEventViewModel (admin dashboard)
            CreateMap<Event, RecentEventViewModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}

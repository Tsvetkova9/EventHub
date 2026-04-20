using EventHub.Core.Entities;
using EventHub.Core.Interfaces;
using EventHub.Core.Services;
using EventHub.Tests.Helpers;
using Moq;

namespace EventHub.Tests
{
    public class EventServiceTests
    {
        private readonly Mock<IRepository<Event>> _eventRepoMock;
        private readonly EventService _service;

        public EventServiceTests()
        {
            _eventRepoMock = new Mock<IRepository<Event>>();
            _service = new EventService(_eventRepoMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_Returns_Correct_Event()
        {
            var ev = new Event { Id = 1, Title = "Music Night" };
            _eventRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ev);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Music Night", result!.Title);
        }

        [Fact]
        public async Task GetByIdAsync_Returns_Null_For_Missing_Event()
        {
            _eventRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Event?)null);

            var result = await _service.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_Sets_CreatedOn_And_Saves()
        {
            Event? captured = null;

            _eventRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .Callback<Event>(e => captured = e)
                .Returns(Task.CompletedTask);

            _eventRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var newEvent = new Event { Title = "Art Expo" };
            await _service.CreateAsync(newEvent);

            Assert.NotNull(captured);
            // CreatedOn should be stamped by the service
            Assert.True(captured!.CreatedOn > DateTime.UtcNow.AddMinutes(-1));
            _eventRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Sets_IsActive_False()
        {
            var ev = new Event { Id = 7, Title = "Old Event", IsActive = true };

            _eventRepoMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(ev);
            _eventRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            await _service.DeleteAsync(7);

            Assert.False(ev.IsActive);
            _eventRepoMock.Verify(r => r.Update(ev), Times.Once);
        }
    }
}

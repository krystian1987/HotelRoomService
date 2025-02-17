using FluentValidation;
using FluentValidation.Results;
using HotelRoomService.Models;
using HotelRoomService.Repositories.Interfaces;
using HotelRoomService.Services;
using HotelRoomService.Services.Interfaces;
using Moq;
using Xunit;

namespace HotelRoomService.Tests.Services
{
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> _mockRoomRepository;
        private readonly Mock<IValidator<Room>> _mockValidator;
        private readonly RoomService _roomService;

        public RoomServiceTests()
        {
            _mockRoomRepository = new Mock<IRoomRepository>();
            _mockValidator = new Mock<IValidator<Room>>();
            _roomService = new RoomService(_mockRoomRepository.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task CreateRoomAsync_ShouldReturnRoom_WhenRoomIsValid()
        {
            // Arrange
            var room = new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
            _mockValidator.Setup(v => v.ValidateAsync(room, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult());
            _mockRoomRepository.Setup(repo => repo.AddRoomAsync(room)).Returns(Task.CompletedTask);

            // Act
            var result = await _roomService.CreateRoomAsync(room);

            // Assert
            Assert.Equal(room, result);
            _mockRoomRepository.Verify(repo => repo.AddRoomAsync(room), Times.Once);
        }

        [Fact]
        public async Task CreateRoomAsync_ShouldThrowValidationException_WhenRoomIsInvalid()
        {
            // Arrange
            var room = new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Name", "Name is required") };
            _mockValidator.Setup(v => v.ValidateAsync(room, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult(validationFailures));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _roomService.CreateRoomAsync(room));
            _mockRoomRepository.Verify(repo => repo.AddRoomAsync(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task GetRoomByIdAsync_ShouldReturnRoom_WhenRoomExists()
        {
            // Arrange
            var room = new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
            _mockRoomRepository.Setup(repo => repo.GetRoomByIdAsync(1)).ReturnsAsync(room);

            // Act
            var result = await _roomService.GetRoomByIdAsync(1);

            // Assert
            Assert.Equal(room, result);
        }

        [Fact]
        public async Task GetRoomByIdAsync_ShouldThrowsKeyNotFoundException_WhenRoomDoesNotExist()
        {
            // Arrange
            _mockRoomRepository.Setup(repo => repo.GetRoomByIdAsync(1)).Throws<KeyNotFoundException>();


			// Act & Assert
			await Assert.ThrowsAsync<KeyNotFoundException>(() => _roomService.GetRoomByIdAsync(1));			
        }

        [Fact]
        public async Task GetRoomsAsync_ShouldReturnRooms_WhenNoFiltersApplied()
        {
            // Arrange
            var rooms = new List<Room>
            {
                new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available },
                new Room { Id = 2, Name = "Room2", Size = 3, Status = RoomStatus.Occupied }
            };
            _mockRoomRepository.Setup(repo => repo.GetAllRoomsAsync(null, null, null)).ReturnsAsync(rooms);

            // Act
            var result = await _roomService.GetRoomsAsync();

            // Assert
            Assert.Equal(rooms, result);
        }

        [Fact]
        public async Task SetRoomStatusAsync_ShouldReturnTrue_WhenRoomExists()
        {
            // Arrange
            var room = new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
            _mockRoomRepository.Setup(repo => repo.GetRoomByIdAsync(1)).ReturnsAsync(room);
            _mockRoomRepository.Setup(repo => repo.UpdateRoomAsync(room)).Returns(Task.CompletedTask);

            // Act
            var result = await _roomService.SetRoomStatusAsync(1, RoomStatus.Unavailable, "Maintenance");

            // Assert
            Assert.True(result);
            Assert.Equal(RoomStatus.Unavailable, room.Status);
            Assert.Equal("Maintenance", room.AdditionalDetails);
            _mockRoomRepository.Verify(repo => repo.UpdateRoomAsync(room), Times.Once);
        }

        [Fact]
        public async Task SetRoomStatusAsync_ShouldReturnFalse_WhenRoomDoesNotExist()
        {
            // Arrange
            _mockRoomRepository.Setup(repo => repo.GetRoomByIdAsync(1)).Throws<KeyNotFoundException>();

			// Act
			var result = await _roomService.SetRoomStatusAsync(1, RoomStatus.Unavailable, "Maintenance");

            // Assert
            Assert.False(result);
            _mockRoomRepository.Verify(repo => repo.UpdateRoomAsync(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task UpdateRoomAsync_ShouldReturnTrue_WhenRoomIsValid()
        {
            // Arrange
            var room = new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
            _mockValidator.Setup(v => v.ValidateAsync(room, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult());
            _mockRoomRepository.Setup(repo => repo.UpdateRoomAsync(room)).Returns(Task.CompletedTask);

            // Act
            var result = await _roomService.UpdateRoomAsync(room);

            // Assert
            Assert.True(result);
            _mockRoomRepository.Verify(repo => repo.UpdateRoomAsync(room), Times.Once);
        }

        [Fact]
        public async Task UpdateRoomAsync_ShouldThrowValidationException_WhenRoomIsInvalid()
        {
            // Arrange
            var room = new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Name", "Name is required") };
            _mockValidator.Setup(v => v.ValidateAsync(room, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult(validationFailures));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _roomService.UpdateRoomAsync(room));
            _mockRoomRepository.Verify(repo => repo.UpdateRoomAsync(It.IsAny<Room>()), Times.Never);
        }
    }
}

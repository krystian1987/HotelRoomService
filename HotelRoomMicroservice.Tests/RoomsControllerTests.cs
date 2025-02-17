using HotelRoomService.Controllers;
using HotelRoomService.Models;
using HotelRoomService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HotelRoomService.Tests.Controllers
{
    public class RoomsControllerTests
    {
        private readonly Mock<IRoomService> _mockRoomService;
        private readonly RoomController _controller;

        public RoomsControllerTests()
        {
            _mockRoomService = new Mock<IRoomService>();
            _controller = new RoomController(_mockRoomService.Object);
        }

        [Fact]
        public async Task GetRooms_ShouldReturnOk_WhenRoomsExist()
        {
            // Arrange
            var rooms = new List<Room>
            {
                new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available },
                new Room { Id = 2, Name = "Room2", Size = 3, Status = RoomStatus.Occupied }
            };
            _mockRoomService.Setup(service => service.GetRoomsAsync(null, null, null)).ReturnsAsync(rooms);

            // Act
            var result = await _controller.GetRooms(null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
			var roomDtoList = Assert.IsType<List<RoomDTO>>(okResult.Value);
			Assert.Equal(rooms.Count(), roomDtoList.Count());
        }

        [Fact]
        public async Task GetRoomById_ShouldReturnOk_WhenRoomExists()
        {
            // Arrange
            var room = new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
            _mockRoomService.Setup(service => service.GetRoomByIdAsync(1)).ReturnsAsync(room);

            // Act
            var result = await _controller.GetRoomById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
			var roomDto = Assert.IsType<RoomDTO>(okResult.Value);
			Assert.Equal(room.Id, roomDto.Id);
        }

        [Fact]
        public async Task GetRoomById_ShouldReturnNotFound_WhenRoomDoesNotExist()
        {
            // Arrange
            _mockRoomService.Setup(service => service.GetRoomByIdAsync(1)).ReturnsAsync((Room)null);

            // Act
            var result = await _controller.GetRoomById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateRoom_ShouldReturnCreatedAtAction_WhenRoomIsCreated()
        {
            // Arrange
            var room = new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
			var createRoomDTO = new CreateRoomDTO {Name = "Room1", Size = 2, Status = RoomStatus.Available };
			_mockRoomService.Setup(service => service.CreateRoomAsync(It.IsAny<Room>())).ReturnsAsync(room);

            // Act
            var result = await _controller.CreateRoom(createRoomDTO);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetRoomById), createdAtActionResult.ActionName);
            Assert.Equal(room.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(room, createdAtActionResult.Value);
        }

        [Fact]
        public async Task UpdateRoom_ShouldReturnNoContent_WhenRoomIsUpdated()
        {
            // Arrange
            var room = new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
			var updateRoomDTO = new UpdateRoomDTO { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
			_mockRoomService.Setup(service => service.UpdateRoomAsync(It.IsAny<Room>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateRoom(1, updateRoomDTO);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateRoom_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            var room = new UpdateRoomDTO { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };

            // Act
            var result = await _controller.UpdateRoom(2, room);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateRoom_ShouldReturnNotFound_WhenRoomDoesNotExist()
        {
            // Arrange
            var room = new Room { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
			var updateRoomDTO = new UpdateRoomDTO { Id = 1, Name = "Room1", Size = 2, Status = RoomStatus.Available };
			_mockRoomService.Setup(service => service.UpdateRoomAsync(It.IsAny<Room>())).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateRoom(1, updateRoomDTO);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SetRoomStatus_ShouldReturnNoContent_WhenStatusIsSet()
        {
            // Arrange
            _mockRoomService.Setup(service => service.SetRoomStatusAsync(1, RoomStatus.Unavailable, "Maintenance")).ReturnsAsync(true);

            // Act
            var result = await _controller.SetRoomStatus(1, RoomStatus.Unavailable, "Maintenance");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task SetRoomStatus_ShouldReturnNotFound_WhenRoomDoesNotExist()
        {
            // Arrange
            _mockRoomService.Setup(service => service.SetRoomStatusAsync(1, RoomStatus.Unavailable, "Maintenance")).ReturnsAsync(false);

            // Act
            var result = await _controller.SetRoomStatus(1, RoomStatus.Unavailable, "Maintenance");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SetRoomStatus_ShouldReturnBadRequest_WhenArgumentExceptionThrown()
        {
            // Arrange
            _mockRoomService.Setup(service => service.SetRoomStatusAsync(1, RoomStatus.Unavailable, null))
                            .ThrowsAsync(new ArgumentException("Details are required for maintenance or locked rooms."));

            // Act
            var result = await _controller.SetRoomStatus(1, RoomStatus.Unavailable, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Details are required for maintenance or locked rooms.", badRequestResult.Value);
        }
    }
}

using HotelRoomService.Attributes;
using HotelRoomService.Models;
using HotelRoomService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HotelRoomService.Controllers
{
	[ApiKeyAuthorize]
	[Route("api/rooms")]
	[ApiController]
	public class RoomController : ControllerBase
	{
		private readonly IRoomService _roomService;

		public RoomController(IRoomService roomService)
		{
			_roomService = roomService;
		}

		/// <summary>
		/// Retrieves a list of rooms based on the provided filters.
		/// </summary>
		/// <param name="name">Filter rooms by name.</param>
		/// <param name="size">Filter rooms by size.</param>
		/// <param name="isAvailable">Filter rooms by availability.</param>
		/// <returns>A list of rooms that match the filters.</returns>
		[HttpGet]
		[SwaggerOperation(Summary = "Retrieves a list of rooms based on the provided filters.")]
		[SwaggerResponse(200, "A list of rooms that match the filters.", typeof(IEnumerable<RoomDTO>))]
		public async Task<IActionResult> GetRooms([FromQuery] string? name, [FromQuery] int? size, [FromQuery] bool? isAvailable)
		{
			var rooms = await _roomService.GetRoomsAsync(name, size, isAvailable);
			return Ok(rooms.Select(x=>x.ToDTO()).ToList());
		}

		/// <summary>
		/// Retrieves the details of a specific room by its ID.
		/// </summary>
		/// <param name="id">The ID of the room to retrieve.</param>
		/// <returns>The room with the specified ID.</returns>
		[HttpGet("{id}")]
		[SwaggerOperation(Summary = "Retrieves the details of a specific room by its ID.")]
		[SwaggerResponse(200, "The room with the specified ID.", typeof(RoomDTO))]
		[SwaggerResponse(404, "Room not found.")]
		public async Task<IActionResult> GetRoomById(int id)
		{
			var room = await _roomService.GetRoomByIdAsync(id);
			return room != null ? Ok(room.ToDTO()) : NotFound();
		}

		/// <summary>
		/// Creates a new room.
		/// </summary>
		/// <param name="room">The room to create.</param>
		/// <returns>The created room.</returns>
		[HttpPost]
		[SwaggerOperation(Summary = "Creates a new room.")]
		[SwaggerResponse(201, "The created room.", typeof(RoomDTO))]
		[SwaggerResponse(400, "Invalid request body.")]
		public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDTO room)
		{
			var createdRoom = await _roomService.CreateRoomAsync(room.ToEntity());
			return CreatedAtAction(nameof(GetRoomById), new { id = createdRoom.Id }, createdRoom);
		}

		/// <summary>
		/// Updates the details of an existing room.
		/// </summary>
		/// <param name="id">The ID of the room to update.</param>
		/// <param name="room">The updated room details.</param>
		/// <returns>No content if the room is successfully updated.</returns>
		[HttpPut("{id}")]
		[SwaggerOperation(Summary = "Updates the details of an existing room.")]
		[SwaggerResponse(204, "No content if the room is successfully updated.")]
		[SwaggerResponse(400, "ID in the URL does not match the ID in the request body.")]
		[SwaggerResponse(404, "Room not found.")]
		public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomDTO room)
		{
			if (id != room.Id) return BadRequest();
			var result = await _roomService.UpdateRoomAsync(room.ToEntity());
			return result ? NoContent() : NotFound();
		}

		/// <summary>
		/// Updates the status of a specific room.
		/// </summary>
		/// <param name="id">The ID of the room to update the status for.</param>
		/// <param name="status">The new status of the room.</param>
		/// <param name="details">Additional details for the status update.</param>
		/// <returns>No content if the status is successfully updated.</returns>
		[HttpPatch("{id}/status")]
		[SwaggerOperation(Summary = "Updates the status of a specific room.")]
		[SwaggerResponse(204, "No content if the status is successfully updated.")]
		[SwaggerResponse(400, "Invalid status update.")]
		[SwaggerResponse(404, "Room not found.")]
		public async Task<IActionResult> SetRoomStatus(int id, [FromBody] RoomStatus status, [FromQuery] string? details)
		{
			try
			{
				var result = await _roomService.SetRoomStatusAsync(id, status, details);
				return result ? NoContent() : NotFound();
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}


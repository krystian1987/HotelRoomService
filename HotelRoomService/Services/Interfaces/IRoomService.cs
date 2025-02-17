using HotelRoomService.Models;

namespace HotelRoomService.Services.Interfaces
{
	public interface IRoomService
	{
		/// <summary>
		/// Creates a new room.
		/// </summary>
		/// <param name="room">The room to create.</param>
		/// <returns>The created room.</returns>
		Task<Room> CreateRoomAsync(Room room);

		/// <summary>
		/// Retrieves a room by its ID.
		/// </summary>
		/// <param name="id">The ID of the room to retrieve.</param>
		/// <returns>The room with the specified ID, or null if not found.</returns>
		Task<Room?> GetRoomByIdAsync(int id);

		/// <summary>
		/// Retrieves all rooms based on the provided filters.
		/// </summary>
		/// <param name="name">Filter rooms by name.</param>
		/// <param name="size">Filter rooms by size.</param>
		/// <param name="isAvailable">Filter rooms by availability.</param>
		/// <returns>A list of rooms that match the filters.</returns>
		Task<IEnumerable<Room>> GetRoomsAsync(string? name, int? size, bool? isAvailable = null);

		/// <summary>
		/// Sets the status of a room.
		/// </summary>
		/// <param name="id">The ID of the room to update.</param>
		/// <param name="status">The new status of the room.</param>
		/// <param name="details">Additional details for the status update.</param>
		/// <returns>True if the status was successfully updated, false otherwise.</returns>
		Task<bool> SetRoomStatusAsync(int id, RoomStatus status, string? details);

		/// <summary>
		/// Updates an existing room.
		/// </summary>
		/// <param name="room">The room to update.</param>
		/// <returns>True if the room was successfully updated, false otherwise.</returns>
		Task<bool> UpdateRoomAsync(Room room);
	}

}
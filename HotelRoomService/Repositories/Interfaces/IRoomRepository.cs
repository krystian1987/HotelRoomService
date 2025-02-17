using HotelRoomService.Models;

namespace HotelRoomService.Repositories.Interfaces
{
	public interface IRoomRepository
	{
		/// <summary>
		/// Retrieves all rooms based on the provided filters.
		/// </summary>
		/// <param name="name">Filter rooms by name.</param>
		/// <param name="size">Filter rooms by size.</param>
		/// <param name="isAvailable">Filter rooms by availability.</param>
		/// <returns>A list of rooms that match the filters.</returns>
		Task<IEnumerable<Room>> GetAllRoomsAsync(string? name = null, int? size = null, bool? isAvailable = null);

		/// <summary>
		/// Retrieves a room by its ID.
		/// </summary>
		/// <param name="id">The ID of the room to retrieve.</param>
		/// <returns>The room with the specified ID.</returns>
		Task<Room> GetRoomByIdAsync(int id);

		/// <summary>
		/// Adds a new room to the repository.
		/// </summary>
		/// <param name="room">The room to add.</param>
		Task AddRoomAsync(Room room);

		/// <summary>
		/// Updates an existing room in the repository.
		/// </summary>
		/// <param name="room">The room to update.</param>
		Task UpdateRoomAsync(Room room);
	}

}

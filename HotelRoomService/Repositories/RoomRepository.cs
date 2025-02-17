using HotelRoomService.Data;
using HotelRoomService.Models;
using HotelRoomService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomService.Repositories
{
	public class RoomRepository : IRoomRepository
	{
		private readonly HotelDbContext _dbContext;

		public RoomRepository(HotelDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<Room>> GetAllRoomsAsync(string? name = null, int? size = null, bool? isAvailable = null)
		{
			IQueryable<Room> query = _dbContext.Rooms;

			if (!string.IsNullOrEmpty(name))
			{
				query = query.Where(r => r.Name.Contains(name));
			}

			if (size.HasValue)
			{
				query = query.Where(r => r.Size == size);
			}

			if (isAvailable.HasValue)
			{
				query = query.Where(r => r.IsAvailable == isAvailable);
			}

			return await query.ToListAsync();
		}

		public async Task<Room> GetRoomByIdAsync(int id)
		{
			var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == id);

			if (room == null)
			{
				throw new KeyNotFoundException($"Room with ID {id} not found.");
			}

			return room;
		}

		public async Task AddRoomAsync(Room room)
		{
			await _dbContext.Rooms.AddAsync(room);
			await _dbContext.SaveChangesAsync();
		}

		public async Task UpdateRoomAsync(Room room)
		{
			_dbContext.Rooms.Update(room);
			await _dbContext.SaveChangesAsync();
		}
	}
}

using FluentValidation;
using HotelRoomService.Models;
using HotelRoomService.Repositories.Interfaces;
using HotelRoomService.Services.Interfaces;
using Serilog;

namespace HotelRoomService.Services
{
	public class RoomService : IRoomService
	{
		private readonly IRoomRepository _roomRepository;
		private readonly IValidator<Room> _validator;

		public RoomService(IRoomRepository repository, IValidator<Room> validator)
		{
			_roomRepository = repository;
			_validator = validator;
		}

		public async Task<IEnumerable<Room>> GetRoomsAsync(string? name = null, int? size = null, bool? isAvailable = null)
		{
			Log.Information("Entering GetRoomsAsync method");
			try
			{
				var rooms = await _roomRepository.GetAllRoomsAsync(name, size, isAvailable);
				Log.Information("Exiting GetRoomsAsync method");
				return rooms;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error fetching rooms from repository");
				throw;
			}
		}

		public async Task<Room?> GetRoomByIdAsync(int id)
		{
			Log.Information("Entering GetRoomByIdAsync method with ID {Id}", id);
			try
			{
				var room = await _roomRepository.GetRoomByIdAsync(id);
				Log.Information("Exiting GetRoomByIdAsync method with ID {Id}", id);
				return room;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error fetching room with ID {Id}", id);
				throw;
			}
		}

		public async Task<Room> CreateRoomAsync(Room room)
		{
			Log.Information("Entering CreateRoomAsync method");
			try
			{
				var validationResult = await _validator.ValidateAsync(room);
				if (!validationResult.IsValid)
				{
					throw new ValidationException(validationResult.Errors);
				}

				await _roomRepository.AddRoomAsync(room);
				Log.Information("Exiting CreateRoomAsync method");
				return room;
			}
			catch (ValidationException ex)
			{
				Log.Warning("Validation failed: {Errors}", ex.Errors);
				throw;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error creating room");
				throw;
			}
		}

		public async Task<bool> UpdateRoomAsync(Room room)
		{
			Log.Information("Entering UpdateRoomAsync method with ID {Id}", room.Id);
			try
			{
				var validationResult = await _validator.ValidateAsync(room);
				if (!validationResult.IsValid)
				{
					throw new ValidationException(validationResult.Errors);
				}

				await _roomRepository.UpdateRoomAsync(room);
				Log.Information("Exiting UpdateRoomAsync method with ID {Id}", room.Id);
				return true;
			}
			catch (ValidationException ex)
			{
				Log.Warning("Validation failed: {Errors}", ex.Errors);
				throw;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error updating room with ID {Id}", room.Id);
				return false;
			}
		}

		public async Task<bool> SetRoomStatusAsync(int id, RoomStatus status, string? details)
		{
			Log.Information("Entering SetRoomStatusAsync method with ID {Id}", id);
			try
			{
				Room room;
				try
				{
					room = await _roomRepository.GetRoomByIdAsync(id);
				}
				catch (KeyNotFoundException)
				{
					Log.Warning("Room with ID {Id} not found", id);
					return false;
				}

				if ((status == RoomStatus.Maintenance || status == RoomStatus.Unavailable) && string.IsNullOrEmpty(details))
				{
					throw new ArgumentException("Details are required for maintenance or locked rooms.");
				}

				room.IsAvailable = status == RoomStatus.Available;
				room.Status = status;
				room.AdditionalDetails = details;

				await _roomRepository.UpdateRoomAsync(room);
				Log.Information("Exiting SetRoomStatusAsync method with ID {Id}", id);
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error setting status for room with ID {Id}", id);
				throw;
			}
		}
	}
}

namespace HotelRoomService.Models
{
	public static class RoomExtension
	{

		public static RoomDTO ToDTO(this Room room)
		{
			return new RoomDTO
			{
				Id = room.Id,
				Name = room.Name,
				Size = room.Size,
				IsAvailable = room.IsAvailable,
				Status = room.Status,
				AdditionalDetails = room.AdditionalDetails
			};
		}
	}
}

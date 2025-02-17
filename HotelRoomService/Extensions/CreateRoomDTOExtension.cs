namespace HotelRoomService.Models
{
	public static class CreateRoomDTOExtension
	{

		public static Room ToEntity(this CreateRoomDTO room)
		{
			return new Room
			{
				Name = room.Name,
				Size = room.Size,
				IsAvailable = room.Status == RoomStatus.Available,
				Status = room.Status,
				AdditionalDetails = room.AdditionalDetails
			};
		}
	}
}

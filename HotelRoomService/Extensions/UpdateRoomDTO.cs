namespace HotelRoomService.Models
{
	public static class UpdateRoomDTOExtension
	{

		public static Room ToEntity(this UpdateRoomDTO room)
		{
			return new Room
			{
				Id = room.Id,
				Name = room.Name,
				Size = room.Size,
				IsAvailable = room.Status == RoomStatus.Available,
				Status = room.Status,
				AdditionalDetails = room.AdditionalDetails
			};
		}
	}
}

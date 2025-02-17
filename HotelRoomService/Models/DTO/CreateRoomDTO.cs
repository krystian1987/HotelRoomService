namespace HotelRoomService.Models
{
	public class CreateRoomDTO
	{
		public string Name { get; set; } = String.Empty;
		public int Size { get; set; }
		public RoomStatus Status { get; set; }
		public string? AdditionalDetails { get; set; }
	}
}

namespace HotelRoomService.Models
{
	public class Room
	{
		public int Id { get; set; }
		public string Name { get; set; } = String.Empty;
		public int Size { get; set; }
		public bool IsAvailable { get; set; }
		public RoomStatus Status { get; set; }
		public string? AdditionalDetails { get; set; }
	}
}

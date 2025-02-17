using System.Text.Json.Serialization;

namespace HotelRoomService.Models
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum RoomStatus
	{
		Available,
		Booked,
		Occupied,
		Maintenance,
		Cleaning,
		Unavailable
	}
}
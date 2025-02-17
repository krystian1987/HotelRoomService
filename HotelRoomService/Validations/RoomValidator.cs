using FluentValidation;
using HotelRoomService.Models;

namespace HotelRoomService.Validations
{
	public class RoomValidator : AbstractValidator<Room>
	{
		public RoomValidator()
		{
			RuleFor(r => r.Name)
				.NotEmpty().WithMessage("Room name is required.")
				.MaximumLength(100).WithMessage("Room name cannot exceed 100 characters.");

			RuleFor(r => r.Size)
				.GreaterThan(0).WithMessage("Room size must be greater than 0.");

			RuleFor(r => r.Status)
				.IsInEnum().WithMessage("Invalid room status.");
		}
	}
}

using FluentValidation;

namespace HotelRoomService.Middleware
{
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;

		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		/// <summary>
		/// Invokes the middleware to handle exceptions.
		/// </summary>
		/// <param name="context">The HTTP context.</param>
		/// <returns>A task that represents the completion of request processing.</returns>
		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (ValidationException ex)
			{
				_logger.LogWarning("Validation error: {Error}", ex.Message);
				context.Response.StatusCode = 400;
				await context.Response.WriteAsync($"Validation Error: {ex.Message}");
			}
			catch (Exception ex)
			{
				_logger.LogError("Unhandled exception: {Error}", ex.Message);
				context.Response.StatusCode = 500;
				await context.Response.WriteAsync("Internal Server Error");
			}
		}
	}
}

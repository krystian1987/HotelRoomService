using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace HotelRoomService.Attributes
{
	[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
	public class ApiKeyAuthorizeAttribute : Attribute, IAuthorizationFilter
	{
		private const string ApiKeyHeaderName = "X-Api-Key";

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
			var apiKey = configuration["Authentication:ApiKey"];

			if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var receivedApiKey))
			{
				Log.Warning("Missing API Key in controller");
				context.Result = new UnauthorizedObjectResult("API Key is missing");
				return;
			}

			if (!string.IsNullOrWhiteSpace(apiKey) && !apiKey.Equals(receivedApiKey))
			{
				Log.Warning("Invalid API Key attempt in controller");
				context.Result = new UnauthorizedObjectResult("Invalid API Key");
				return;
			}
		}
	}
}

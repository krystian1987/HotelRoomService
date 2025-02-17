using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;
using HotelRoomService.Middleware;
using HotelRoomService.Repositories.Interfaces;
using HotelRoomService.Repositories;
using HotelRoomService.Services;
using HotelRoomService.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using HotelRoomService.Validations;
using Microsoft.OpenApi.Models;
using HotelRoomService.Services.Interfaces;
using HotelRoomService.Models;
using Microsoft.OpenApi.Any;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.CreateLogger();

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("HotelDbConnection");

builder.Services.AddDbContext<HotelDbContext>(options =>
	options.UseSqlite(connectionString));

builder.Services.AddValidatorsFromAssemblyContaining<RoomValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();

builder.Services.AddRateLimiter(options =>
{
	options.AddFixedWindowLimiter("global", configure =>
	{
		configure.PermitLimit = 10;
		configure.Window = TimeSpan.FromSeconds(10);
	});
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(x =>
{
	x.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Room API", Version = "v1" });
	x.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Name = "X-API-KEY",
		Type = SecuritySchemeType.ApiKey,
		Description = "API Key Authentication"
	});

	x.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "ApiKey"
				}
			},
			Array.Empty<string>()
		}
	});

	x.EnableAnnotations();

	x.MapType<RoomStatus>(() => new OpenApiSchema
	{
		Type = "string",
		Enum = Enum.GetNames(typeof(RoomStatus))
				.Select(name => (IOpenApiAny)new OpenApiString(name))
				.ToList()
	});
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();

	try
	{
		Log.Information("Applying migrations...");
		dbContext.Database.Migrate();
		Log.Information("Database is ready.");
	}
	catch (Exception ex)
	{
		Log.Error(ex, "An error occurred while migrating the database.");
	}
}

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapMetrics();
});

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
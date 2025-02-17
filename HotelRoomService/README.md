# HotelRoomService

## Description
Hotel Room Microservice is a REST API designed to manage hotel rooms.
It provides functionalities such as:

✅ Fetching all rooms with optional filtering by name, size, and availability  
✅ Fetching a single room by ID  
✅ Creating new hotel rooms  
✅ Updating room details  
✅ Marking rooms as occupied or unavailable when:
- A guest has booked the room
- The room is being cleaned
- The room is under maintenance
- The room is manually locked for special guests  
✅ Validation & logging included with FluentValidation & Serilog  
✅ Global error handling with proper HTTP responses  

## Prerequisites
Ensure you have the following installed:
- [.NET 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [SQLite](https://www.sqlite.org/download.html) (optional, as EF Core handles database creation)
- Git (for cloning the repository)

## Installation & Setup

### 1. Clone the repository
```sh
git clone https://github.com/your-username/hotel-room-service.git
cd hotel-room-service
```
### 2. Configure the database
The service uses SQLite as the database.
Modify `appsettings.json` if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=hotel.db"
  },
  "ApiSettings": {
    "ApiKey": "super-secret-key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 3. Serilog Config

```json
"Serilog": {
      "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Grafana.Loki" ],
      "MinimumLevel": {
        "Default": "Information",
        "Override": {
          "Microsoft": "Error",
          "System": "Error"
        }
      },
      "WriteTo": [
        { "Name": "Console" },
        {
          "Name": "File",
          "Args": {
            "path": "./output/log.txt",
            "rollingInterval": "Day",
            "retainedFileCountLimit": 7
          }
        },
        {
          "Name": "GrafanaLoki",
          "Args": {
            "uri": "http://192.168.1.95:3100",
            "textFormatter": "Serilog.Sinks.Grafana.Loki.LokiJsonTextFormatter, Serilog.Sinks.Grafana.Loki",
            "labels": [
              {
                "key": "app",
                "value": "p2g"
              }
            ]
          }
        }]
}
```

| Field      | Required | Default | Description |
|:-----------|:---------|:--------|:------------|
| Using | no | `null` | A list of sinks you would like use. The valid sinks are listed in the examplea above. |
| MinimumLevel | no | `null` | The minimum level to write. `[Verbose, Debug, Information, Warning, Error, Fatal]` |
| WriteTo | no | `null` | Additional config for various sinks you are writing to. |

More detailed information about configuring Logging can be found on the [Serilog Config Repo](https://github.com/serilog/serilog-settings-configuration#serilogsettingsconfiguration--).

### 4. API Authentication
All API requests require an **API Key** in the request headers:
```
X-API-KEY: super-secret-key
```
Modify `appsettings.json` if needed:
```json
{
  "Authentication": {
    "ApiKey": "my-secret-api-key"
  }
}
```

### Build the Project

Open the solution file `HotelRoomService.sln` in Visual Studio or your preferred IDE and build the solution.

Alternatively, you can build the project using the .NET CLI:


### Run the Application

You can run the application from Visual Studio by pressing `F5` or `Ctrl+F5`.

Alternatively, you can run the application using the .NET CLI:


The application will start and listen on the configured port (e.g., `https://localhost:5001`).

### Running the Tests

The project includes unit tests for the service and controller layers. You can run the tests from Visual Studio's Test Explorer.

Alternatively, you can run the tests using the .NET CLI:


This will execute all the tests in the solution and display the results in the terminal.

## Project Structure

- `HotelRoomService/`: The main project containing the API controllers, services, repositories, and middleware.
- `HotelRoomService.Tests/`: The test project containing unit tests for the service and controller layers.

## Key Files

- `Controllers/RoomsController.cs`: The API controller for managing hotel rooms.
- `Services/RoomService.cs`: The service layer for business logic related to hotel rooms.
- `Repositories/RoomRepository.cs`: The repository layer for data access related to hotel rooms.
- `Middleware/GlobalExceptionMiddleware.cs`: Middleware for handling global exceptions.
- `Tests/Controllers/RoomsControllerTests.cs`: Unit tests for the `RoomsController`.
- `Tests/Services/RoomServiceTests.cs`: Unit tests for the `RoomService`.
- `Tests/Middleware/GlobalExceptionMiddlewareTests.cs`: Unit tests for the `GlobalExceptionMiddleware`.

## REST API Endpoints

### Get All Rooms

- **URL**: `/api/rooms`
- **Method**: `GET`
- **Query Parameters**:
  - `name` (optional): Filter rooms by name.
  - `size` (optional): Filter rooms by size.
  - `status` (optional): Filter rooms by status.
- **Description**: Retrieves a list of rooms based on the provided filters.
- **Response**: `200 OK` with a list of rooms.

### Get Room by ID

- **URL**: `/api/rooms/{id}`
- **Method**: `GET`
- **URL Parameters**:
  - `id`: The ID of the room to retrieve.
- **Description**: Retrieves the details of a specific room by its ID.
- **Response**: 
  - `200 OK` with the room details if found.
  - `404 Not Found` if the room does not exist.

### Create Room

- **URL**: `/api/rooms`
- **Method**: `POST`
- **Request Body**: A JSON object representing the room to create.
- **Description**: Creates a new room.
- **Response**: 
  - `201 Created` with the created room details.
  - `400 Bad Request` if the request body is invalid.

### Update Room

- **URL**: `/api/rooms/{id}`
- **Method**: `PUT`
- **URL Parameters**:
  - `id`: The ID of the room to update.
- **Request Body**: A JSON object representing the updated room details.
- **Description**: Updates the details of an existing room.
- **Response**: 
  - `204 No Content` if the room is successfully updated.
  - `400 Bad Request` if the ID in the URL does not match the ID in the request body.
  - `404 Not Found` if the room does not exist.

### Set Room Status

- **URL**: `/api/rooms/{id}/status`
- **Method**: `PATCH`
- **URL Parameters**:
  - `id`: The ID of the room to update the status for.
- **Query Parameters**:
  - `details` (optional): Additional details for the status update.
- **Request Body**: A JSON object representing the new status.
- **Description**: Updates the status of a specific room.
- **Response**: 
  - `204 No Content` if the status is successfully updated.
  - `400 Bad Request` if the status update is invalid.
  - `404 Not Found` if the room does not exist.

## Logging & Monitoring

### 1. Logging with Serilog
Logs are stored in `logs/log-.txt` and displayed in the console. Errors and request logs are captured automatically.

### 2. Monitoring with Prometheus

The API exposes metrics at `/metrics`
To enable Prometheus, add:

```csharp
app.UseMetricServer();
app.UseHttpMetrics();
```


## Technologies Used
- **ASP.NET Core 7.0**
- **Entity Framework Core** (with SQLite)
- **FluentValidation** for request validation
- **Serilog** for logging
- **Swagger** (OpenAPI) for API documentati

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
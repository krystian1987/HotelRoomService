using System.Net;
using FluentValidation;
using HotelRoomService.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HotelRoomService.Tests.Middleware
{
    public class GlobalExceptionMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly Mock<ILogger<GlobalExceptionMiddleware>> _mockLogger;
        private readonly GlobalExceptionMiddleware _middleware;

        public GlobalExceptionMiddlewareTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
            _middleware = new GlobalExceptionMiddleware(_mockNext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Invoke_ShouldCallNextMiddleware_WhenNoExceptionThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            await _middleware.Invoke(context);

            // Assert
            _mockNext.Verify(next => next(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_ShouldHandleValidationException_WhenValidationExceptionThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var validationException = new ValidationException("Validation error");
            _mockNext.Setup(next => next(context)).ThrowsAsync(validationException);

            // Act
            await _middleware.Invoke(context);

            // Assert
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Validation error")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_ShouldHandleException_WhenUnhandledExceptionThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new Exception("Unhandled exception");
            _mockNext.Setup(next => next(context)).ThrowsAsync(exception);

            // Act
            await _middleware.Invoke(context);

            // Assert
            _mockLogger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unhandled exception")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        }

        private async Task<string> GetResponseBodyAsync(HttpContext context)
        {
            context.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
            using var reader = new System.IO.StreamReader(context.Response.Body);
            return await reader.ReadToEndAsync();
        }
    }
}

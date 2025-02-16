using ImageEventApi.Core.Application.DTO;
using ImageEventApi.Core.Domain.Interfaces;
using ImageEventApi.Core.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ImageEventApi.Tests.Controllers
{
    public class ImagesControllerTests
    {
        private readonly Mock<IImageStorageService> _mockStorage;
        private readonly ImagesController _controller;

        public ImagesControllerTests()
        {
            _mockStorage = new Mock<IImageStorageService>();
            _controller = new ImagesController(_mockStorage.Object);
        }

        [Fact]
        public void PostImage_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Test error");
            var request = new ImageRequest();

            // Act
            var result = _controller.PostImage(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void PostImage_ValidModel_UpdatesStorageAndReturnsOk()
        {
            // Arrange
            var request = new ImageRequest
            {
                Description = "Test Description",
                ImageUrl = "http://test.com/image.jpg"
            };

            ImageEvent capturedEvent = null;
            _mockStorage.Setup(s => s.Update(It.IsAny<ImageEvent>()))
                .Callback<ImageEvent>(e => capturedEvent = e);

            // Act
            var result = _controller.PostImage(request);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockStorage.Verify(s => s.Update(It.IsAny<ImageEvent>()), Times.Once);

            Assert.NotNull(capturedEvent);
            Assert.Equal(request.Description, capturedEvent.Description);
            Assert.Equal(request.ImageUrl, capturedEvent.ImageUrl);
            Assert.True(DateTime.UtcNow.Subtract(capturedEvent.ReceivedAt).TotalSeconds < 5);
        }

        [Fact]
        public void GetImage_ReturnsLatestImageAndCount()
        {
            // Arrange
            var expectedImage = new ImageEvent { ImageUrl = "http://test.com/image.jpg" };
            var expectedCount = 5;

            _mockStorage.SetupGet(s => s.LatestImage).Returns(expectedImage);
            _mockStorage.SetupGet(s => s.LastHourCount).Returns(expectedCount);

            // Act
            var result = _controller.GetImage();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as ImageResponse;

            Assert.NotNull(response);
            Assert.Equal(expectedCount, response.LastHourCount);
            Assert.Equal(expectedImage.ImageUrl, response.Image?.ImageUrl);
        }

        [Fact]
        public void GetImage_WhenNoImage_ReturnsNullImage()
        {
            // Arrange
            _mockStorage.SetupGet(s => s.LatestImage).Returns((ImageEvent)null);
            _mockStorage.SetupGet(s => s.LastHourCount).Returns(0);

            // Act
            var result = _controller.GetImage();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as ImageResponse;

            Assert.NotNull(response);
            Assert.Null(response.Image);
            Assert.Equal(0, response.LastHourCount);
        }
    }
}

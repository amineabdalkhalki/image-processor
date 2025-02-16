using System.Reflection;
using System.Text;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;
using ImageEventApi.Core.Domain.Models;
using ImageEventApi.Infrastructure.AWS.EventConsumers;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace ImageEventApi.Tests.Infrastructure.AWS.EventConsumers
{
    public class KinesisEventConsumerTests
    {
        private readonly Mock<ILambdaContext> _mockContext;
        private readonly Mock<ILambdaLogger> _mockLogger;

        public KinesisEventConsumerTests()
        {
            _mockLogger = new Mock<ILambdaLogger>();
            _mockContext = new Mock<ILambdaContext>();
            _mockContext.Setup(c => c.Logger).Returns(_mockLogger.Object);
        }

        [Fact]
        public async Task FunctionHandlerAsync_ValidEvent_SendsPostRequest()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var expectedUrl = "http://test.com/api/images";
            var testEvent = CreateTestKinesisEvent(new ImageEvent { ImageUrl = "test.jpg" });

            Environment.SetEnvironmentVariable("ImageApiUrl", expectedUrl);

            mockHttp.Expect(HttpMethod.Post, expectedUrl)
                .Respond(System.Net.HttpStatusCode.OK);

            var consumer = CreateConsumerWithMockHttpClient(mockHttp);

            // Act
            await consumer.FunctionHandlerAsync(testEvent, _mockContext.Object);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            _mockLogger.Verify(l => l.LogLine($"Processed Kinesis event via API: test.jpg"), Times.Once);
        }


        [Fact]
        public async Task FunctionHandlerAsync_ApiFailure_LogsError()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var testEvent = CreateTestKinesisEvent(new ImageEvent { ImageUrl = "error.jpg" });

            mockHttp.Expect("*")
                .Respond(System.Net.HttpStatusCode.InternalServerError);

            var consumer = CreateConsumerWithMockHttpClient(mockHttp);

            // Act
            await consumer.FunctionHandlerAsync(testEvent, _mockContext.Object);

            // Assert
            _mockLogger.Verify(l => l.LogLine("Failed to process Kinesis event via API. Status code: InternalServerError"), Times.Once);
        }

        [Fact]
        public async Task FunctionHandlerAsync_Exception_LogsError()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var testEvent = CreateTestKinesisEvent(new ImageEvent());

            mockHttp.Expect("*").Throw(new HttpRequestException("Test error"));
            var consumer = CreateConsumerWithMockHttpClient(mockHttp);

            // Act
            await consumer.FunctionHandlerAsync(testEvent, _mockContext.Object);

            // Assert
            _mockLogger.Verify(l => l.LogLine("Error processing record: Test error"), Times.Once);
        }

        private KinesisEventConsumer CreateConsumerWithMockHttpClient(MockHttpMessageHandler handler)
        {
            var consumer = new KinesisEventConsumer();

            // Replace HttpClient with our mock handler using reflection
            var httpClientField = typeof(KinesisEventConsumer).GetField("_httpClient",
                BindingFlags.NonPublic | BindingFlags.Instance);

            httpClientField.SetValue(consumer, new HttpClient(handler));

            return consumer;
        }

        private KinesisEvent CreateTestKinesisEvent(ImageEvent imageEvent)
        {
            var json = JsonSerializer.Serialize(imageEvent);
            var data = Encoding.UTF8.GetBytes(json);

            return new KinesisEvent
            {
                Records = new List<KinesisEvent.KinesisEventRecord>
                {
                    new KinesisEvent.KinesisEventRecord
                    {
                        Kinesis = new KinesisEvent.Record { Data = new MemoryStream(data) }
                    }
                }
            };
        }
    }
}
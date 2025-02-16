using System.Text;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;
using ImageEventApi.Core.Domain.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ImageEventApi.Infrastructure.AWS.EventConsumers
{
    public class KinesisEventConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        /* This implementation just to satisfy the requirements, i know it's not the best
         I have tried first to Inject the service or even an Event Producer
         But once i deploy to AWS i didn't had a 'warm' instance and the consumer had
         Different instance of the Service (checked the Hashcode they were different in the CloudWatch logs)
         I tried to use Redis or DynamoDb but they weren't included in free tier :) */
        public KinesisEventConsumer()
        {
            _httpClient = new HttpClient();

            // Build configuration from appsettings.json and environment variables.
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Read the API URL from configuration.
            _apiUrl = config["ImageApiUrl"];
            if (string.IsNullOrEmpty(_apiUrl))
            {
                // Fallback if not configured.
                _apiUrl = "http://localhost:5000/api/images";
            }
        }

        public KinesisEventConsumer(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task FunctionHandlerAsync(KinesisEvent kinesisEvent, ILambdaContext context)
        {
            foreach (var record in kinesisEvent.Records)
            {
                try
                {
                    var json = Encoding.UTF8.GetString(record.Kinesis.Data.ToArray());
                    var imageEvent = JsonSerializer.Deserialize<ImageEvent>(json);
                    if (imageEvent != null)
                    {
                        // Post the image event to the API endpoint so it updates the shared state.
                        var content = new StringContent(JsonSerializer.Serialize(imageEvent), Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync(_apiUrl, content);
                        if (response.IsSuccessStatusCode)
                        {
                            context.Logger.LogLine($"Processed Kinesis event via API: {imageEvent.ImageUrl}");
                        }
                        else
                        {
                            context.Logger.LogLine($"Failed to process Kinesis event via API. Status code: {response.StatusCode}");
                        }
                    }
                    else
                    {
                        context.Logger.LogLine("Deserialized image event is null.");
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine($"Error processing record: {ex.Message}");
                }
            }
            await Task.CompletedTask;
        }
    }
}

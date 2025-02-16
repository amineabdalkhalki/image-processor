using System.Text;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;
using ImageEventApi.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ImageEventApi.Consumers
{
    public class KinesisEventConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

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

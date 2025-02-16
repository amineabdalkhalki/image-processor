using System.Text;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;
using ImageEventApi.Models;

namespace ImageEventApi;
public class KinesisEventConsumer
{
    private readonly ImageEventProcessor _processor;

    public KinesisEventConsumer()
    {
        _processor = LambdaDIContainer.ServiceProvider.GetRequiredService<ImageEventProcessor>();
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
                    _processor.Process(imageEvent);
                    context.Logger.LogLine($"Processed Kinesis event: {imageEvent.ImageUrl}");
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
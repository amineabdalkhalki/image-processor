using System;
using global::ImageEventApi.Models;
using ImageEventApi.Services;

namespace ImageEventApi;

public class ImageEventProcessor
{
    private readonly IImageStorage _storage;

    public ImageEventProcessor(IImageStorage storage)
    {
        _storage = storage;
    }

    public void Process(ImageEvent imageEvent)
    {
        // Unified processing logic
        _storage.Update(imageEvent);
        Console.WriteLine($"Processed event: {imageEvent.ImageUrl}");
    }
}
using ImageEventApi.DTO;
using ImageEventApi.Models;
using ImageEventApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IImageStorageService _imageStorage;

    public ImagesController(IImageStorageService imageStorage)
    {
        _imageStorage = imageStorage;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult PostImage([FromBody] ImageRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var imageEvent = Map(request);
        _imageStorage.Update(imageEvent);

        return Ok();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetImage()
    {
        var latestImage = _imageStorage.LatestImage;
        var lastHourCount = _imageStorage.LastHourCount;

        var response = new ImageResponse
        {
            Image = latestImage,
            LastHourCount = lastHourCount
        };

        return Ok(response);
    }

    private static ImageEvent Map(ImageRequest request)
    {
        return new ImageEvent
        {
            ImageUrl = request.ImageUrl,
            Description = request.Description,
            ReceivedAt = DateTime.UtcNow
        };
    }
}

using Microsoft.AspNetCore.Mvc;
using ImageEventApi.Models;
using ImageEventApi.Services;
using ImageEventApi.DTO;

namespace ImageEventApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageStorage _imageStorage;

        public ImagesController(IImageStorage imageStorage)
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

            //map to ImageEvent
            ImageEvent imageEvent = Map(request);


            _imageStorage.Update(imageEvent);
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetImage()
        {
            return Ok(new
            {
                Image = _imageStorage.LatestImage,
                LastHourCount = _imageStorage.LastHourCount
            });
        }

        private static ImageEvent Map(ImageRequest request)
        {
            return new ImageEvent()
            {
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                ReceivedAt = DateTime.UtcNow
            };
        }
    }
}

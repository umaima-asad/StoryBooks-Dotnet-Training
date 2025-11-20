using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using StoryBooks.Language;
using System.Globalization;
namespace StoryBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslationsController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;

        public TranslationsController(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        [HttpGet("hello")]
        public IActionResult Get()
        {
            return Ok(new { message = _localizer["Hello"] });
        }
    }
}

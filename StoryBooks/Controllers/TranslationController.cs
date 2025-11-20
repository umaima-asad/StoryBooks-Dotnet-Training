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
            var strings = _localizer.GetAllStrings()
                        .ToDictionary(ls => ls.Name, ls => ls.Value);

            return Ok(new { resources = strings });
        }
    }
}

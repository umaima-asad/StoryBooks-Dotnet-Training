using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryBooks.Application.DTOs;  
using StoryBooks.Application.Services;

namespace StoryBooks.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StoryBookController : ControllerBase
    {
        private readonly IStoryBookServices _service;
        private readonly IValidator<StoryBookDTO> _validator;
        private readonly IValidator<CreateStoryBookDTO> _createValidator;
        private readonly IAuthorizationService _authorizationService;
        private readonly IRedisCacheService _cacheService;
        public StoryBookController(IStoryBookServices service, IValidator<StoryBookDTO> validator, IValidator<CreateStoryBookDTO> createValidator,IAuthorizationService authorizationService, IRedisCacheService cacheService)
        {

            _service = service;
            _validator = validator;
            _createValidator = createValidator;
            _authorizationService = authorizationService;
            _cacheService = cacheService;
        }


        [Authorize (Roles = "Librarian , Student")]
        [HttpGet]
        public async Task<IActionResult> GetStoryBooks(int pageNumber = 1, int pageSize = 10)
        {
            var cacheKey = $"StoryBooks_Page{pageNumber}_Size{pageSize}";
            var storyBooksFromCache = _cacheService.GetData<PagedStoryBooksDTO>(cacheKey);
            if (storyBooksFromCache is not null)
            {
                return Ok(storyBooksFromCache);
            }

            var (storyBooks, totalCount) = await _service.GetStoryBooksAsync(pageNumber, pageSize);

            var response = new PagedStoryBooksDTO
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = storyBooks,
                PageCount = (int)Math.Ceiling((double)totalCount / pageSize)
            };
            _cacheService.SetData(cacheKey, response);
            return Ok(response);
        }


        [Authorize(Roles = "Librarian , Student")]
        [HttpGet("{id}")]
        public async Task<ActionResult<StoryBookDTO?>> GetStoryBook(int id)
        {
            var cacheKey = $"StoryBook_{id}";
            var storyBookFromCache = _cacheService.GetData<StoryBookDTO>(cacheKey);
            if (storyBookFromCache is not null)
            {
                return Ok(storyBookFromCache);
            }
            var storyBook = await _service.GetStoryBookByIdAsync(id);
            _cacheService.SetData(cacheKey, storyBook);
            if (storyBook == null) return NotFound();

            return Ok(storyBook);
        }


        [Authorize(Roles = "Librarian , Student")]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong 🏓");
        }


        [Authorize(Roles = "Librarian")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<StoryBookDTO>> CreateStoryBook(CreateStoryBookDTO storyBookDto)
        {

            bool exists = await _service.StoryBookExistsAsync(storyBookDto);
            if (exists)
               return BadRequest("Book already exists");

            var validationResult = await _createValidator.ValidateAsync(storyBookDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            string? imagePath = null;
            if (storyBookDto.Cover != null && storyBookDto.Cover.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(storyBookDto.Cover.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await storyBookDto.Cover.CopyToAsync(stream);
                }
                imagePath = $"/images/{uniqueFileName}";
            }

            var createdStoryBook = await _service.CreateStoryBookAsync(new StoryBookDTO
            {
                BookName = storyBookDto.BookName,
                Author = storyBookDto.Author,
                Cover = imagePath // <- store path in DB
            });

            var created = await _service.CreateStoryBookAsync(createdStoryBook);
            return CreatedAtAction(nameof(GetStoryBook), new { id = created.BookName }, created);
        }


        [Authorize(Roles = "Librarian")]
        [HttpPut("{id}")]
        public async Task<ActionResult<StoryBookDTO>> UpdateStoryBook(int id, CreateStoryBookDTO storyBookDto)
        {
            var validationResult = await _createValidator.ValidateAsync(storyBookDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            if (!ModelState.IsValid) return BadRequest(ModelState);


            string? imagePath = null;
            if (storyBookDto.Cover != null && storyBookDto.Cover.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(storyBookDto.Cover.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await storyBookDto.Cover.CopyToAsync(stream);
                }
                imagePath = $"/images/{uniqueFileName}";

            }

            var dbStorybook = await _service.GetStoryBookByIdAsync(id);
            if (dbStorybook == null)
                return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, dbStorybook, "CanEditWithoutCover");
            if (!authResult.Succeeded)
            {
                return Forbid(); // or return Unauthorized() if you prefer
            }

            var storyBookToUpdate = new StoryBookDTO
            {
                BookName = storyBookDto.BookName,
                Author = storyBookDto.Author,
                Cover = imagePath // <- store path in DB
            };
            var updatedStoryBook = await _service.UpdateStoryBookAsync(id, storyBookToUpdate);
            if (updatedStoryBook == null) return NotFound();
            
            return Ok(updatedStoryBook);
        }


        [Authorize(Roles = "Librarian")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoryBook(int id)
        {
            var result = await _service.DeleteStoryBookAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }


        [Authorize(Roles = "Librarian, Student")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<StoryBookDTO>>> SearchStoryBooks(string search_word)
        {
            if (string.IsNullOrWhiteSpace(search_word) || search_word.Length > 100)
                return BadRequest("Invalid search term");
            var cacheKey = $"Search_{search_word}";
            var searchResultsFromCache = _cacheService.GetData<IEnumerable<StoryBookDTO>>(cacheKey);
            if (searchResultsFromCache is not null)
            {
                return Ok(searchResultsFromCache);
            }
            var search_results = await _service.SearchStoryBookAsync(search_word);
            _cacheService.SetData(cacheKey, search_results);
            return Ok(search_results);
        }

    }
}

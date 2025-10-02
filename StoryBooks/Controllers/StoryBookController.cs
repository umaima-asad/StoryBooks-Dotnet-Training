using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoryBooks.DTOs;  
using StoryBooks.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace StoryBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryBookController : ControllerBase
    {
        private readonly IStoryBookServices _service;
        private readonly IValidator<StoryBookDTO> _validator;
        private readonly IValidator<CreateStoryBookDTO> _createValidator;
        public StoryBookController(IStoryBookServices service, IValidator<StoryBookDTO> validator, IValidator<CreateStoryBookDTO> createValidator)
        {

            _service = service;
            _validator = validator;
            _createValidator = createValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoryBookDTO>>> GetStoryBooks(int pageSize = 5, int pageNumber=1)
        {
            var storyBooks = await _service.GetStoryBooksAsync(pageSize,pageNumber);
            return Ok(storyBooks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StoryBookDTO>> GetStoryBook(int id)
        {
            var storyBook = await _service.GetStoryBookByIdAsync(id);
            if (storyBook == null) return NotFound();

            return Ok(storyBook);
        }
        [Authorize]
        [HttpPost]
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoryBook(int id)
        {
            var result = await _service.DeleteStoryBookAsync(id);
            if (!result) return NotFound();
            
            return NoContent();
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<StoryBookDTO>>> SearchStoryBooks(string search_word)
        {
            if (string.IsNullOrWhiteSpace(search_word) || search_word.Length > 100)
                return BadRequest("Invalid search term");

            var search_results = await _service.SearchStoryBookAsync(search_word);
            return Ok(search_results);
        }
        //add images as static files

    }
}

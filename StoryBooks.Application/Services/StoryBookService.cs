using Microsoft.EntityFrameworkCore;
using StoryBooks.Application.DTOs;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Domain.Models;
using StoryBooks.Application.MappingProfiles;
using Microsoft.AspNetCore.Http;
using StoryBooks.Application.Interfaces;

namespace StoryBooks.Application.Services
{
    public class StoryBookService : IStoryBookServices
    {
        private readonly IStoryBookRepository _StoryBookcontext;
        private readonly TenantProvider _tenantProvider;
        public StoryBookService(IStoryBookRepository StoryBookcontext, TenantProvider tenantProvider)
        {
            _StoryBookcontext = StoryBookcontext;
            _tenantProvider = tenantProvider;
        }


        public async Task<(IEnumerable<StoryBookDTO> StoryBooks, int TotalCount)> GetStoryBooksAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _StoryBookcontext.GetAllStoryBooksAsync(pageNumber, pageSize);

            var storyBookDtos = items.Select(sb => sb.ToDto());

            return (storyBookDtos, totalCount);
        }


        public async Task<StoryBookDTO?> GetStoryBookByIdAsync(int id)
        {
            var storyBook = await _StoryBookcontext.GetStoryBooksByIdAsync(id);
            if (storyBook == null)
                return null;
            return storyBook.ToDto();
        }
        public async Task<StoryBookDTO> CreateStoryBookAsync(StoryBookDTO storyBookDto)
        {
            var storyBook = storyBookDto.ToEntity();
            storyBook.TenantID = _tenantProvider.GetTenantId();
            var createdStoryBook = await _StoryBookcontext.AddStoryBookAsync(storyBook);
            return storyBookDto;
        }
        public async Task<StoryBook> UpdateStoryBookAsync(int id, StoryBookDTO storyBookDto)
        {
            var storyBook = storyBookDto.ToEntity();
            var updatedStoryBook = await _StoryBookcontext.UpdateStoryBookAsync(id, storyBook); 
            return updatedStoryBook;
        }
        public async Task<bool> DeleteStoryBookAsync(int id)
        {
            var result = await _StoryBookcontext.DeleteStoryBookAsync(id);
            return true;
        }

        public async Task<IEnumerable<StoryBookDTO>> SearchStoryBookAsync(string search_word)
        {
            var stories = await _StoryBookcontext.SearchStoryBookAsync(search_word);
            return stories.Select(sb => sb.ToDto()).ToList();
        }
        public async Task<bool> StoryBookExistsAsync(CreateStoryBookDTO dto)
        {
            var story = dto.ToEntity();
            return await _StoryBookcontext.StoryBookExistsAsync(story);
        }
        public async Task<string> ConvertFormFileToStringPathAsync(IFormFile file)
        {
            string? imagePath = null;
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                 await file.CopyToAsync(stream);
            }
            imagePath = $"/images/{uniqueFileName}";
            return imagePath;
        }
    }
}

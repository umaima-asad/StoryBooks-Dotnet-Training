using Microsoft.EntityFrameworkCore;
using StoryBooks.Application.DTOs;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Domain.Models;
using StoryBooks.Application.MappingProfiles;

namespace StoryBooks.Application.Services
{
    public class StoryBookService : IStoryBookServices
    {
        private readonly IStoryBookRepository _StoryBookcontext;
        public StoryBookService(IStoryBookRepository StoryBookcontext)
        {
            _StoryBookcontext = StoryBookcontext;
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
    }
}

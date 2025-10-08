using Microsoft.EntityFrameworkCore;
using StoryBooks.Application.DTOs;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Domain.Models;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

            var storyBookDtos = items.Select(sb => new StoryBookDTO
            {
                BookName = sb.BookName,
                Author = sb.Author,
                Cover = sb.Cover
            });

            return (storyBookDtos, totalCount);
        }
        public async Task<StoryBookDTO?> GetStoryBookByIdAsync(int id)
        {
            var storyBook = await _StoryBookcontext.GetStoryBooksByIdAsync(id);
            if (storyBook == null)
                return null;
            return new StoryBookDTO
            {
                BookName = storyBook.BookName,
                Author = storyBook.Author,
                Cover = storyBook.Cover
            };
        }
        public async Task<StoryBookDTO> CreateStoryBookAsync(StoryBookDTO storyBookDto)
        {
            var storyBook = new StoryBook
            {
                BookName = storyBookDto.BookName,
                Author = storyBookDto.Author,
                Cover = storyBookDto.Cover
            };
            var createdStoryBook = await _StoryBookcontext.AddStoryBookAsync(storyBook);
            return storyBookDto;
        }
        public async Task<StoryBook> UpdateStoryBookAsync(int id, StoryBookDTO storyBookDto)
        {
            var storyBook = new StoryBook
            {
                BookName = storyBookDto.BookName,
                Author = storyBookDto.Author,
                Cover = storyBookDto.Cover
            };
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
            return stories.Select(sb => new StoryBookDTO
            {
                BookName = sb.BookName,
                Author = sb.Author,
                Cover = sb.Cover
            }).ToList();
        }
        public async Task<bool> StoryBookExistsAsync(CreateStoryBookDTO dto)
        {
            var story = new StoryBook
            {
                BookName = dto.BookName,
                Author = dto.Author,
            };
            return await _StoryBookcontext.StoryBookExistsAsync(story);
        }
    }
}

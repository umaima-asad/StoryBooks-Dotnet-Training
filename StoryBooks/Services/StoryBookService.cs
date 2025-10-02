using Microsoft.EntityFrameworkCore;
using StoryBooks.Data; 
using StoryBooks.DTOs;
using StoryBooks.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StoryBooks.Services
{
    public class StoryBookService : IStoryBookServices
    {
        private readonly StoryBookContext _context;
        public StoryBookService(StoryBookContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StoryBookDTO>> GetStoryBooksAsync(int pageSize, int pageNumber)
        {
            return await _context.StoryBooks
                .OrderBy(sb => sb.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(sb => new StoryBookDTO
                {
                    BookName = sb.BookName,
                    Author = sb.Author,
                    Cover = sb.Cover
                })
                .ToListAsync();
        }
        public async Task<StoryBookDTO> GetStoryBookByIdAsync(int id)
        {
            var storyBook = await _context.StoryBooks.FindAsync(id);
            if (storyBook == null) return null;

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

            _context.StoryBooks.Add(storyBook);
            await _context.SaveChangesAsync();
            return storyBookDto;
        }
        public async Task<StoryBookDTO> UpdateStoryBookAsync(int id, StoryBookDTO storyBookDto)
        {
            var storyBook = await _context.StoryBooks.FindAsync(id);
            if (storyBook == null) return null;

            storyBook.BookName = storyBookDto.BookName;
            storyBook.Author = storyBookDto.Author;
            storyBook.Cover = storyBookDto.Cover;

            await _context.SaveChangesAsync();
            return storyBookDto;
        }
        public async Task<bool> DeleteStoryBookAsync(int id)
        {
            var storyBook = await _context.StoryBooks.FindAsync(id);
            if (storyBook == null) return false;

            _context.StoryBooks.Remove(storyBook);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StoryBookDTO>> SearchStoryBookAsync(string search_word)
        {
            return await _context.StoryBooks
                .Where(b => b.BookName.Contains(search_word) || b.Author.Contains(search_word))
                .Select(b => new StoryBookDTO
                {
                    BookName = b.BookName,
                    Author = b.Author,
                   Cover = b.Cover
                })
                .ToListAsync();
        }
        public async Task<bool> StoryBookExistsAsync(CreateStoryBookDTO dto)
        {
            return await _context.StoryBooks.AnyAsync(b => b.BookName == dto.BookName && b.Author == dto.Author);
        }

    }
}

using Microsoft.EntityFrameworkCore;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Domain.Models;
using StoryBooks.Infrastructure.Data;


namespace StoryBooks.Infrastructure.Repositories
{
    public class StoryBookRepository : IStoryBookRepository
    {
        private readonly StoryBookContext _context;

        public StoryBookRepository(StoryBookContext context)
        {
            _context = context;
        }
        public async Task<(IEnumerable<StoryBook> Items, int TotalCount)> GetAllStoryBooksAsync(int pageNumber, int pageSize)
        {
            var query = _context.StoryBooks.AsNoTracking();
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<StoryBook?> GetStoryBooksByIdAsync(int id)
        {
            return await _context.StoryBooks.FindAsync(id);
        }
        public async Task<StoryBook> AddStoryBookAsync(StoryBook storyBook)
        {
            _context.StoryBooks.Add(storyBook);
            await _context.SaveChangesAsync();
            return storyBook;
        }
        public async Task<StoryBook> UpdateStoryBookAsync(int id, StoryBook storyBookDto)
        {
            var storyBook = await _context.StoryBooks.FindAsync(id);
            if (storyBook == null) return null;

            bool modified = false;
            if (!string.IsNullOrWhiteSpace(storyBookDto.BookName))
            {
                storyBook.BookName = storyBookDto.BookName;
                modified = true;
            }
            if (!string.IsNullOrWhiteSpace(storyBookDto.Author))
            {
                storyBook.Author = storyBookDto.Author;
                modified = true;
            }
            if (!string.IsNullOrWhiteSpace(storyBookDto.Cover))
            {
                storyBook.Cover = storyBookDto.Cover;
                modified = true;
            }
            if (modified == true)
                await _context.SaveChangesAsync();

            return storyBook;
        }
        public async Task<bool> DeleteStoryBookAsync(int id)
        {
            var storyBook = await _context.StoryBooks.FindAsync(id);
            if (storyBook == null) return false;
            _context.StoryBooks.Remove(storyBook);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<StoryBook>> SearchStoryBookAsync(string search_word)
        {
            return await _context.StoryBooks
                .Where(b => b.BookName.Contains(search_word) || b.Author.Contains(search_word))
                .ToListAsync();
        }
        public async Task<bool>StoryBookExistsAsync(StoryBook story)
        {
            return await _context.StoryBooks
                .AnyAsync(b => b.BookName == story.BookName && b.Author == story.Author);
        }
    }
}

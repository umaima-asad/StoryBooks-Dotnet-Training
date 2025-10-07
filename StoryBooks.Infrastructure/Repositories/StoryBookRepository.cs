using Microsoft.EntityFrameworkCore;
using StoryBooks.Application.DTOs;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Domain.Models;
using StoryBooks.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryBooks.Infrastructure.Repositories
{
    public class StoryBookRepository : IStoryBookRepository
    {
        private readonly StoryBookContext _context;

        public StoryBookRepository(StoryBookContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StoryBook>> GetAllStoryBooksAsync(int pageNumber, int pageSize)
        {
            return await _context.StoryBooks
                 .OrderBy(sb => sb.Id)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        }
        public async Task<StoryBook> GetStoryBooksByIdAsync(int id)
        {
            var storyBook = await _context.StoryBooks.FindAsync(id);
            return storyBook;
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
        public async Task<IEnumerable<StoryBook>> SearchStoryBookAsync(string search_word)
        {
            return await _context.StoryBooks
                .Where(b => b.BookName.Contains(search_word) || b.Author.Contains(search_word))
                .ToListAsync();
        }
        public async Task<bool>StoryBookExistsAsync(StoryBook story)
        {
            var storyBook = await _context.StoryBooks
                .Where(b => b.BookName.Contains(story.BookName) && b.Author.Contains(story.Author))
                .ToListAsync(); ;
            if (storyBook == null)
            {
                return false;
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryBooks.Domain.Models;
namespace StoryBooks.Domain.Interfaces
{
    public interface IStoryBookRepository
    {
        Task<(IEnumerable<StoryBook> Items, int TotalCount)> GetAllStoryBooksAsync(int pageNumber, int pageSize);
        Task<StoryBook> AddStoryBookAsync(StoryBook storyBook);
        Task<StoryBook?> GetStoryBooksByIdAsync(int id);
        Task<StoryBook> UpdateStoryBookAsync(int id, StoryBook storyBookDto);
        Task<bool> DeleteStoryBookAsync(int id);
        Task<IEnumerable<StoryBook>> SearchStoryBookAsync(string search_word);
        Task<bool> StoryBookExistsAsync(StoryBook story);
    }
}

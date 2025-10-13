using Microsoft.AspNetCore.Http;
using StoryBooks.Application.DTOs;
using StoryBooks.Domain.Models;
namespace StoryBooks.Application.Services
{
    public interface IStoryBookServices
    {
        Task<(IEnumerable<StoryBookDTO> StoryBooks, int TotalCount)> GetStoryBooksAsync(int pageNumber, int pageSize);
        Task<StoryBookDTO?> GetStoryBookByIdAsync(int id);
        Task<StoryBookDTO> CreateStoryBookAsync(StoryBookDTO storyBookDto);
        Task<StoryBook> UpdateStoryBookAsync(int id,StoryBookDTO storyBookDto);
        Task<bool> DeleteStoryBookAsync(int id);
        Task<IEnumerable<StoryBookDTO>> SearchStoryBookAsync(string search_word);
        Task<bool> StoryBookExistsAsync(CreateStoryBookDTO dto);
        Task <string> ConvertFormFileToStringPathAsync(IFormFile file);
    }
}

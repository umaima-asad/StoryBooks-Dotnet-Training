using StoryBooks.DTOs;
namespace StoryBooks.Services
{
    public interface IStoryBookServices
    {
        Task<IEnumerable<StoryBookDTO>>GetStoryBooksAsync(int pageSize, int pageNumber);
        Task<StoryBookDTO> GetStoryBookByIdAsync(int id);
        Task<StoryBookDTO> CreateStoryBookAsync(StoryBookDTO storyBookDto);
        Task<StoryBookDTO> UpdateStoryBookAsync(int id, StoryBookDTO storyBookDto);
        Task<bool> DeleteStoryBookAsync(int id);
        Task<IEnumerable<StoryBookDTO>> SearchStoryBookAsync(string search_word);
        Task<bool> StoryBookExistsAsync(CreateStoryBookDTO dto);
    }
}

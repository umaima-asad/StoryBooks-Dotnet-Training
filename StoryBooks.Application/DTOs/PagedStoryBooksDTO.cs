using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryBooks.Application.DTOs
{
    public class PagedStoryBooksDTO
    {
        public IEnumerable<StoryBookDTO> Items { get; set; } = Enumerable.Empty<StoryBookDTO>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }
}

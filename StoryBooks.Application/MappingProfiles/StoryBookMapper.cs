using AutoMapper;
using StoryBooks.Domain.Models;
using StoryBooks.Application.DTOs;

namespace StoryBooks.Application.MappingProfiles
{
    public static class StoryBookMapper
    {
        private static readonly IMapper _mapper;

        static StoryBookMapper()
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                // Create → Entity
                cfg.CreateMap<CreateStoryBookDTO, StoryBook>()
                    .ForMember(dest => dest.Cover, opt => opt.Ignore()); // handled manually if needed

                // Entity → DTO
                cfg.CreateMap<StoryBook, StoryBookDTO>().ReverseMap();
            });

            _mapper = config.CreateMapper();
        }

        // Convert Create DTO → Entity
        public static StoryBook ToEntity(this StoryBookDTO dto)
        {
            return _mapper.Map<StoryBook>(dto);
        }
        public static StoryBook ToEntity(this CreateStoryBookDTO dto)
        {
            return _mapper.Map<StoryBook>(dto);
        }
        // Update existing entity from Create DTO
        public static StoryBook ToEntity(this CreateStoryBookDTO dto, StoryBook entity)
        {
            return _mapper.Map(dto, entity);
        }

        // Convert Entity → DTO
        public static StoryBookDTO ToDto(this StoryBook entity)
        {
            return _mapper.Map<StoryBookDTO>(entity);
        }
    }
}

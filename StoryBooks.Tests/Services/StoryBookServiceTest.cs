using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using StoryBooks.Application.DTOs;
using StoryBooks.Application.MappingProfiles;
using StoryBooks.Application.Services;
using StoryBooks.Application.Validators;
using StoryBooks.Domain.Interfaces;
using StoryBooks.Domain.Models;
using Xunit;

namespace StoryBooks.Tests.Services
{
    public class StoryBookServiceTests
    {
        private readonly Mock<IStoryBookRepository> _mockRepo;
        private readonly StoryBookService _service;
        private readonly Mock<TenantProvider> _tenantProvider;

        public StoryBookServiceTests()
        {
            _mockRepo = new Mock<IStoryBookRepository>();
            _service = new StoryBookService(_mockRepo.Object,_tenantProvider.Object);
            _tenantProvider = new Mock<TenantProvider>();
        }

        // Helpers to create fake data
        private static StoryBook GetFakeEntity(int id = 1) => new()
        {
            Id = id,
            BookName = "The Testing Book",
            Author = "Jane Doe",
            Cover = "cover.png"
        };

        private static StoryBookDTO GetFakeDto() => new()
        {
            BookName = "The Testing Book",
            Author = "Jane Doe",
            Cover = "cover.png"
        };

        // -------------------- TESTS --------------------

        [Fact]
        public async Task GetStoryBooksAsync_ShouldReturnDtosAndCount()
        {
            // Arrange
            var stories = new List<StoryBook> { GetFakeEntity(1), GetFakeEntity(2) };
            _mockRepo.Setup(r => r.GetAllStoryBooksAsync(1, 10))
                .ReturnsAsync((stories, stories.Count));

            // Act
            var (result, count) = await _service.GetStoryBooksAsync(1, 10);

            // Assert
            count.Should().Be(2);
            result.Should().HaveCount(2);
            result.First().BookName.Should().Be("The Testing Book");
            _mockRepo.Verify(r => r.GetAllStoryBooksAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetStoryBookByIdAsync_ShouldReturnDto_WhenExists()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetStoryBooksByIdAsync(1))
                .ReturnsAsync(GetFakeEntity(1));

            // Act
            var result = await _service.GetStoryBookByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.BookName.Should().Be("The Testing Book");
        }

        [Fact]
        public async Task GetStoryBookByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetStoryBooksByIdAsync(99))
                .ReturnsAsync((StoryBook?)null);

            // Act
            var result = await _service.GetStoryBookByIdAsync(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateStoryBookAsync_ShouldAddAndReturnDto()
        {
            // Arrange
            var dto = GetFakeDto();
            var entity = GetFakeEntity();
            _mockRepo.Setup(r => r.AddStoryBookAsync(It.IsAny<StoryBook>()))
                     .ReturnsAsync(entity);

            // Act
            var result = await _service.CreateStoryBookAsync(dto);

            // Assert
            result.Should().BeEquivalentTo(dto);
            _mockRepo.Verify(r => r.AddStoryBookAsync(It.IsAny<StoryBook>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStoryBookAsync_ShouldCallRepoAndReturnUpdatedEntity()
        {
            // Arrange
            var dto = GetFakeDto();
            var updatedEntity = GetFakeEntity();
            _mockRepo.Setup(r => r.UpdateStoryBookAsync(1, It.IsAny<StoryBook>()))
                     .ReturnsAsync(updatedEntity);

            // Act
            var result = await _service.UpdateStoryBookAsync(1, dto);

            // Assert
            result.Should().NotBeNull();
            result.BookName.Should().Be("The Testing Book");
            _mockRepo.Verify(r => r.UpdateStoryBookAsync(1, It.IsAny<StoryBook>()), Times.Once);
        }

        [Fact]
        public async Task DeleteStoryBookAsync_ShouldReturnTrue_WhenDeleted()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteStoryBookAsync(1))
                     .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteStoryBookAsync(1);

            // Assert
            result.Should().BeTrue();
            _mockRepo.Verify(r => r.DeleteStoryBookAsync(1), Times.Once);
        }

        [Fact]
        public async Task SearchStoryBookAsync_ShouldReturnMatchingDtos()
        {
            // Arrange
            var stories = new List<StoryBook> { GetFakeEntity(1) };
            _mockRepo.Setup(r => r.SearchStoryBookAsync("Book"))
                     .ReturnsAsync(stories);

            // Act
            var result = await _service.SearchStoryBookAsync("Book");

            // Assert
            result.Should().ContainSingle();
            result.First().BookName.Should().Be("The Testing Book");
        }

        [Fact]
        public async Task StoryBookExistsAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var dto = new CreateStoryBookDTO
            {
                BookName = "The Testing Book",
                Author = "Jane Doe"
            };
            _mockRepo.Setup(r => r.StoryBookExistsAsync(It.IsAny<StoryBook>()))
                     .ReturnsAsync(true);

            // Act
            var result = await _service.StoryBookExistsAsync(dto);

            // Assert
            result.Should().BeTrue();
        }
        [Fact]
        public void CreateStoryBookDTOValidator_Should_Have_Error_When_BookName_Is_Empty()
        {
            //Arrange
            var validator = new CreateStoryBookDTOValidator();
            
            //Act
            var dto = new CreateStoryBookDTO { BookName = "" };

            //Assert
            var result = validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.BookName);
        }
        [Fact]
        public void ToEntity_Should_Update_Existing_StoryBook()
        {
            // Arrange
            var dto = new CreateStoryBookDTO { BookName = "Updated", Author = "Author" };
            var entity = new StoryBook { BookName = "Old", Author = "OldAuthor" };

            // Act
            StoryBookMapper.ToEntity(dto, entity);

            // Assert
            Assert.Equal("Updated", entity.BookName);
            Assert.Equal("Author", entity.Author);
        }

    }
}

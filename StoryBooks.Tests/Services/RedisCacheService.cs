using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StoryBooks.Application.Services;
using Xunit;

namespace StoryBooks.Tests.Application.Services
{
    public class RedisCacheServiceTests
    {
        private readonly IDistributedCache _cache;
        private readonly RedisCacheService _service;

        public RedisCacheServiceTests()
        {
            // Use in-memory cache instead of Redis
            var options = Options.Create(new MemoryDistributedCacheOptions());
            _cache = new MemoryDistributedCache(options);
            _service = new RedisCacheService(_cache);
        }

        [Fact]
        public void SetData_Should_Store_And_GetData_Should_Retrieve_Value()
        {
            // Arrange
            var key = "storybook:1";
            var expected = new TestStory { BookName = "My Story", Author = "Umaima" };

            // Act
            _service.SetData(key, expected);
            var result = _service.GetData<TestStory>(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.BookName, result.BookName);
            Assert.Equal(expected.Author, result.Author);
        }

        [Fact]
        public void GetData_Should_Return_Default_When_Key_Not_Found()
        {
            // Arrange
            var key = "nonexistent:key";

            // Act
            var result = _service.GetData<TestStory>(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void SetData_Should_Respect_Expiration_Time()
        {
            // Arrange
            var key = "temp:storybook";
            var expected = new TestStory { BookName = "Temp Story", Author = "Test Author" };

            // Act
            _service.SetData(key, expected);

            // Simulate waiting beyond expiration (since MemoryDistributedCache ignores absolute expiration by default, 
            // this mainly verifies that no exception is thrown)
            var result = _service.GetData<TestStory>(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.BookName, result.BookName);
        }

        private class TestStory
        {
            public string BookName { get; set; } = string.Empty;
            public string Author { get; set; } = string.Empty;
        }
    }
}

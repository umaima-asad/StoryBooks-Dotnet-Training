using FluentAssertions;
using StoryBooks.Application.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace StoryBooks.IntegrationTests
{
    public class StoryBookControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public StoryBookControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            var token = "CfDJ8ITPxih0VKJMuzLlFHFkmGAziOr0AT01f7I1Y4UoOU-FACavvKomXSfxClvorlQO0cBlz9k_ob-JPwDJKUN-dA9hpyccEVKwhXkQ8cqFYuCyUUn0Nf2muhcEk2k8ooNDsQK5e-pqXo0wK8j0pjzM814BUgqMQ6UPzZWBxm_yk0UMvjXG_UUghEqPCfkgb8CDbXPNm99Bro3munOXz1Dakn-QRG9VvrEse48T4gTvBh2_qSRbC_jrOTX6CKbOaEMkCEzU_gvFAmuKT7W2mdi7owMDBSTztz8-syCEwBWEfHPvo3wGvrK7cj2jxyLZj5nDsqKT_HQpI97RomipwuQp_lgF-jRupBr87fMRwkoeJ-C4VO0SnsiKtLZ8rGCyb3OjOIq4Eg7anTMV2fTAu7oROwo9z_4AHyuUJ-T2-vhvgRMejUKm9dlpf9oHjHUO2K_aYcs5gWjDSPSry6Mk0v4J2vK7-lBrduBItxNFL0cQBk33uL44qm7wq_k4_HA63U6QBw794IF-DiiasSVVgh8n8xsCLLmwHyVuX5Jpeg5Wa2Puzlzzay9CdWPJUhdgqA1SpgPOofjLzvBzH1vXrRSUqSBe90VP8fSH8PwEn6kRhwZ3JhTvArjQjUC_fmQV45asrnyNlDTSR7_YK4UVXNqHWtLDME0n1cOsajfBl6_8wS0vYu1sQy0-tWOWbslEf9NO-v26V5bdBRz6C6sJbxB0QP0cggeV7kVj5EC3glzTtwohIqdrifGP6VInwIY68GTTnNpm2HbFtA_m4I95d13rda5yd4BZffr6_keaN91MTvqN87At49NWc7dqqCPsNH5yDw";

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            _client.DefaultRequestHeaders.Add("X-Tenant-ID", "1");
        }

        [Fact]
        public async Task Ping_ShouldReturnPong()
        {
            // Act
            var response = await _client.GetAsync("/api/StoryBook/ping");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsStringAsync();
            result.Should().Contain("pong");
        }
        [Fact]
        public async Task CreateStoryBook_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            var newBook = new CreateStoryBookDTO
            {
                BookName = "Integration Testing 103",
                Author = "Umaima Dev3",
                Cover = null
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/StoryBook", newBook);
            var problemDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine(problemDetails);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdBook = await response.Content.ReadFromJsonAsync<StoryBookDTO>();
            createdBook.Should().NotBeNull();
            createdBook!.BookName.Should().Be("Integration Testing 103");
        }
        [Fact]
        public async Task GetStoryBooks_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/StoryBook");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

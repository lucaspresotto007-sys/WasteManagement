using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WasteManagementAPI.ViewModels;
using Xunit;

namespace WasteManagementAPI.Tests
{
    public class DisposalControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DisposalControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetMyHistory_ReturnsHttpStatusCode200()
        {
            // Arrange
            // 1. Authenticate to get a token
            var loginRequest = "/api/Auth/login";
            var loginDto = new LoginViewModel
            {
                Email = "resident@test.com",
                Password = "password123"
            };

            var loginResponse = await _client.PostAsJsonAsync(loginRequest, loginDto);
            loginResponse.EnsureSuccessStatusCode();

            var authResult = await loginResponse.Content.ReadFromJsonAsync<UserTokenViewModel>();
            Assert.NotNull(authResult);
            Assert.False(string.IsNullOrEmpty(authResult.Token));

            // 2. Set token in authorization header
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.Token);

            var request = "/api/Disposal/my-history";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode(); // Verifies status code is 200 OK
        }
    }
}

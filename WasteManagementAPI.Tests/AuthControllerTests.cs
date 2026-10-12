using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WasteManagementAPI.ViewModels;
using Xunit;

namespace WasteManagementAPI.Tests
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ReturnsHttpStatusCode200()
        {
            // Arrange
            var request = "/api/Auth/login";
            var loginDto = new LoginViewModel
            {
                Email = "resident@test.com",
                Password = "password123"
            };

            // Act
            var response = await _client.PostAsJsonAsync(request, loginDto);

            // Assert
            response.EnsureSuccessStatusCode(); // Verifies status code is 200 OK
        }
    }
}

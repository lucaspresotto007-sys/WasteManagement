using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WasteManagementAPI.Tests
{
    public class EducationalTipControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EducationalTipControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsHttpStatusCode200()
        {
            // Arrange
            var request = "/api/EducationalTip";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Xunit.Sdk.XunitException($"Request failed. Status: {response.StatusCode}. Response: {body}");
            }
        }
    }
}

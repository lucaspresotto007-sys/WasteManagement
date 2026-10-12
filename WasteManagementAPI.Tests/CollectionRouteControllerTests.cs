using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WasteManagementAPI.Tests
{
    public class CollectionRouteControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CollectionRouteControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsHttpStatusCode200()
        {
            // Arrange
            var request = "/api/CollectionRoute";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode(); // Verifies status code is 200-299 (200 OK)
        }
    }
}

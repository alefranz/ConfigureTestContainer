using ConfigureTestContainer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
    public class IntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public IntegrationTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("ConfigureServices,ConfigureContainer", content);
        }

        [Fact]
        public async Task ConfigureTestServices()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => services.AddSingleton(new TestService("ConfigureTestServices")));
            }).CreateClient();

            // Act
            var response = await client.GetAsync("");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("ConfigureServices,ConfigureTestServices,ConfigureContainer", content);
        }

        [Fact]
        public async Task ConfigureTestContainer()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder => {
                builder.ConfigureTestContainer<ThirdPartyContainer>(services => services.Services.AddSingleton(new TestService("ConfigureTestContainer")));
            }).CreateClient();

            // Act
            var response = await client.GetAsync("");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("ConfigureServices,ConfigureContainer,ConfigureTestContainer", content);
        }

        [Fact]
        public async Task ConfigureTestServices_ConfigureTestContainer()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => services.AddSingleton(new TestService("ConfigureTestServices")));
                builder.ConfigureTestContainer<ThirdPartyContainer>(services => services.Services.AddSingleton(new TestService("ConfigureTestContainer")));
            }).CreateClient();

            // Act
            var response = await client.GetAsync("");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("ConfigureServices,ConfigureTestServices,ConfigureContainer,ConfigureTestContainer", content);
        }
    }
}

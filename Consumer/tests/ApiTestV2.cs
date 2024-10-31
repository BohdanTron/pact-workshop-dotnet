using Consumer;
using PactNet;
using PactNet.Matchers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace tests
{
    public class ApiTestV2
    {
        private const int Port = 9000;

        private readonly List<object> _products =
        [
            new { id = 9, type = "CREDIT_CARD", name = "GEM Visa", version = "v2" },
            new { id = 10, type = "CREDIT_CARD", name = "28 Degrees", version = "v1" }
        ];

        private readonly IPactBuilderV4 _pact;
        private readonly ApiClient _apiClient;

        public ApiTestV2(ITestOutputHelper output)
        {
            _pact = Pact.V4("ApiClient", "ProductService", new PactConfig
            {
                PactDir = Path.Join("..", "..", "..", "..", "..", "pacts"),
                Outputters = [new XUnitOutput(output)]

            }).WithHttpInteractions(port: Port);

            _apiClient = new ApiClient(new System.Uri($"http://localhost:{Port}"));
        }

        [Fact]
        public async Task GetAllProducts()
        {
            // Arrange
            _pact.UponReceiving("A valid request for all products")
                    .Given("products exist")
                    .WithRequest(HttpMethod.Get, "/api/products")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new TypeMatcher(_products));

            await _pact.VerifyAsync(async ctx =>
            {
                // Act
                var response = await _apiClient.GetAllProducts();

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Fact]
        public async Task GetProduct()
        {
            // Arrange
            _pact.UponReceiving("A valid request for a product")
                    .Given("product with ID 10 exists")
                    .WithRequest(HttpMethod.Get, "/api/products/10")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new TypeMatcher(_products[1]));

            await _pact.VerifyAsync(async ctx =>
            {
                // Act
                var response = await _apiClient.GetProduct(10);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }
    }
}

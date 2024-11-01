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
                Outputters = [new XUnitOutput(output)],
                LogLevel = PactLogLevel.Debug
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
                    .WithHeader("Authorization", Match.Regex("Bearer 2019-01-14T11:34:18.045Z", "Bearer \\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}\\.\\d{3}Z"))
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
                    .WithHeader("Authorization", Match.Regex("Bearer 2019-01-14T11:34:18.045Z", "Bearer \\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}\\.\\d{3}Z"))
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new TypeMatcher(_products[1]));

            // Act / Assert
            await _pact.VerifyAsync(async ctx =>
            {
                var response = await _apiClient.GetProduct(10);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Fact]
        public async Task NoProductsExist()
        {
            // Arrange
            _pact.UponReceiving("A valid request for all products")
                    .Given("no products exist")
                    .WithRequest(HttpMethod.Get, "/api/products")
                    .WithHeader("Authorization", Match.Regex("Bearer 2019-01-14T11:34:18.045Z", "Bearer \\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}\\.\\d{3}Z"))  // STEP_8
                .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new TypeMatcher(new List<object>()));

            // Act / Assert
            await _pact.VerifyAsync(async ctx =>
            {
                var response = await _apiClient.GetAllProducts();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Fact]
        public async Task ProductDoesNotExist()
        {
            // Arrange
            _pact.UponReceiving("A valid request for product")
                    .Given("product with ID 11 does not exist")
                    .WithRequest(HttpMethod.Get, "/api/products/11")
                    .WithHeader("Authorization", Match.Regex("Bearer 2019-01-14T11:34:18.045Z", "Bearer \\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}\\.\\d{3}Z"))
                .WillRespond()
                    .WithStatus(HttpStatusCode.NotFound);

            // Act / Assert
            await _pact.VerifyAsync(async ctx =>
            {
                var response = await _apiClient.GetProduct(11);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            });
        }

        [Fact]
        public async Task GetProductMissingAuthHeader()
        {
            // Arrange
            _pact.UponReceiving("A valid request for product")
                .Given("No auth token is provided")
                    .WithRequest(HttpMethod.Get, "/api/products/10")
                .WillRespond()
                    .WithStatus(HttpStatusCode.Unauthorized);

            // Act / Assert
            await _pact.VerifyAsync(async ctx =>
            {
                var response = await _apiClient.GetProduct(10);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            });
        }
    }
}

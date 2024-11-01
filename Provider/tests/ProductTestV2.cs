using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace tests
{
    public class ProductTestV2
    {
        private const string PactServiceUrl = "http://localhost:9001";

        private readonly ITestOutputHelper _output;

        public ProductTestV2(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task EnsureProviderApiHonorsPactWithConsumer()
        {
            // Arrange
            using var webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>()
                .UseUrls(PactServiceUrl)
                .Build();

            await webHost.StartAsync();

            //await Task.Delay(1000);

            // Act / Assert
            var pactVerifier = new PactVerifier("ProductService",
                new PactVerifierConfig { Outputters = new List<IOutput> { new XunitOutput(_output) } });

            var pactFile = new FileInfo(Path.Join("..", "..", "..", "..", "..", "pacts", "ApiClient-ProductService.json"));

            pactVerifier
                .WithHttpEndpoint(new Uri(PactServiceUrl))
                .WithFileSource(pactFile)
                .WithProviderStateUrl(new Uri($"{PactServiceUrl}/provider-states"))
                .Verify();
        }
    }
}

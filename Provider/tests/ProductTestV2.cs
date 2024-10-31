using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using System;
using System.Collections.Generic;
using System.IO;
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
        public void EnsureProviderApiHonorsPactWithConsumer()
        {
            // Arrange
            using var webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>()
                .UseUrls(PactServiceUrl)
                .Build();

            webHost.Start();

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

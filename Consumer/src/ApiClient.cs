using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Consumer
{
    public class ApiClient
    {
        private readonly Uri _baseUri;

        public ApiClient(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public async Task<HttpResponseMessage> GetAllProducts()
        {
            using var client = new HttpClient();
            
            client.BaseAddress = _baseUri;
            client.DefaultRequestHeaders.Add("Authorization", AuthorizationHeaderValue());

            try
            {
                var response = await client.GetAsync("/api/products");
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem connecting to Products API.", ex);
            }
        }

        public async Task<HttpResponseMessage> GetProduct(int id)
        {
            using var client = new HttpClient();
            
            client.BaseAddress = _baseUri;
            client.DefaultRequestHeaders.Add("Authorization", AuthorizationHeaderValue());

            try
            {
                var response = await client.GetAsync($"/api/products/{id}");
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem connecting to Products API.", ex);
            }
        }

        private string AuthorizationHeaderValue() => $"Bearer {DateTime.Now:yyyy-MM-ddTHH:mm:ss.fffZ}";
    }
}
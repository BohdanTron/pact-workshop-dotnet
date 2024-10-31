using Consumer;
using System;

var apiClient = new ApiClient(new Uri("http://localhost:9001"));

Console.WriteLine("**Retrieving product list**");

var response = await apiClient.GetAllProducts();
var responseBody = await response.Content.ReadAsStringAsync();

Console.WriteLine($"Response.Code={response.StatusCode}, Response.Body={responseBody}\n\n");


var productId = 10;
Console.WriteLine($"**Retrieving product with id={productId}");

response = await apiClient.GetProduct(productId);
responseBody = await response.Content.ReadAsStringAsync();

Console.WriteLine($"Response.Code={response.StatusCode}, Response.Body={responseBody}");
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5000/");

Console.WriteLine("Logging in...");
var loginResponse = await client.PostAsJsonAsync("api/v1/auth/login", new { Email = "admin@capled.com", Password = "Admin123!" });
if (!loginResponse.IsSuccessStatusCode)
{
    Console.WriteLine($"Login failed: {loginResponse.StatusCode}");
    return;
}

var loginData = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
var token = loginData.GetProperty("token").GetString();
client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

Console.WriteLine("Fetching alerts...");
var response = await client.GetAsync("api/v1/alerts/low-stock");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine($"Alerts failed: {response.StatusCode}");
    return;
}

var json = await response.Content.ReadAsStringAsync();
Console.WriteLine("API Response:");
Console.WriteLine(json);

var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var alerts = JsonSerializer.Deserialize<JsonElement>(json, options);
foreach (var alert in alerts.EnumerateArray())
{
    Console.WriteLine($"Equipment: {alert.GetProperty("equipmentName").GetString()}, Qty: {alert.GetProperty("currentQuantity").GetInt32()}, AlertLevel: {alert.GetProperty("alertLevel").GetString()}");
}

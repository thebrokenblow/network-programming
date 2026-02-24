using HttpAppClient;
using System.Text.Json;
using System.Text;

await PrintPersons();
await CreatePerson();
await PrintPersons();
await UpdatePerson();
await PrintPersons();
await DeletePerson();
await PrintPersons();

Console.ReadLine();

async Task PrintPersons()
{
    var httpClient = new HttpClient();
    var responce = await httpClient.GetAsync("https://localhost:5001/api/person");
    var content = await responce.Content.ReadAsStringAsync();
    var persons = JsonSerializer.Deserialize<List<Person>>(content, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    });

    Console.WriteLine(string.Join(", ", persons));
}

async Task CreatePerson()
{
    var httpClient = new HttpClient();
    string baseUrl = "https://localhost:5001/api/person";
    var uri = new Uri(baseUrl);
    httpClient.BaseAddress = uri;

    var person = new Person
    {
        FirstName = "Some First Name",
        SecondName = "Some Second Name"
    };

    var json = JsonSerializer.Serialize(person);
    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

    var responce = await httpClient.PostAsync(string.Empty , stringContent);
}

async Task UpdatePerson()
{
    var httpClient = new HttpClient();
    string baseUrl = "https://localhost:5001/api/person";
    var uri = new Uri(baseUrl);
    httpClient.BaseAddress = uri;

    var person = new Person
    {
        Id = 1,
        FirstName = "Магомед",
        SecondName = "Дагиров"
    };

    var json = JsonSerializer.Serialize(person);
    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

    var responce = await httpClient.PutAsync(string.Empty, stringContent);
}

async Task DeletePerson()
{
    using var httpClient = new HttpClient();
    var id = int.Parse(Console.ReadLine());
    var response = await httpClient.DeleteAsync($"https://localhost:5001/api/person/{id}");
}
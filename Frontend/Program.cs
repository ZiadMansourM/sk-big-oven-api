using System.Text;
using System.Text.Json;

var app = new Frontend.Services.ConsoleService();
await app.Run();

public static class Requests
{
    private const string _baseAddress = "http://localhost:5100";

    public static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        )!;
    }

    async public static Task<List<Frontend.Models.Recipe>> ListRecipes()
    {
        var uri = new Uri($"{_baseAddress}/recipes");
        using var client = new HttpClient();
        var json = await client.GetAsync(uri);
        var result = await json.Content.ReadAsStringAsync();
        return Deserialize<List<Frontend.Models.Recipe>>(result);
    }

    async public static Task<List<Frontend.Models.Category>> ListCategories()
    {
        var uri = new Uri($"{_baseAddress}/categories");
        using var client = new HttpClient();
        var json = await client.GetAsync(uri);
        var result = await json.Content.ReadAsStringAsync();
        return Deserialize<List<Frontend.Models.Category>>(result);
    }

    async public static Task<Frontend.Models.Recipe> GetRecipe(Guid id)
    {
        var uri = new Uri($"{_baseAddress}/recipes/{id}");
        using var client = new HttpClient();
        var json = await client.GetAsync(uri);
        var result = await json.Content.ReadAsStringAsync();
        return Deserialize<Frontend.Models.Recipe>(result);
    }

    async public static Task<Frontend.Models.Category> GetCategory(Guid id)
    {
        var uri = new Uri($"{_baseAddress}/categories/{id}");
        using var client = new HttpClient();
        var json = await client.GetAsync(uri);
        var result = await json.Content.ReadAsStringAsync();
        return Deserialize<Frontend.Models.Category>(result);
    }

    async public static Task<Frontend.Models.Recipe> CreateRecipe(string name, List<string> ingredients, List<string> instructions, List<Guid> categoriesIds)
    {
        var uri = new Uri($"{_baseAddress}/recipes");
        var recipe = new Frontend.Models.Recipe(name, ingredients, instructions, categoriesIds);
        var json = JsonSerializer.Serialize(recipe);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var result = await client.PostAsync(uri, payload);
        var responce = await result.Content.ReadAsStringAsync();
        return Deserialize<Frontend.Models.Recipe>(json);
    }

    async public static Task<Frontend.Models.Category> CreateCategory(string name)
    {
        var uri = new Uri($"{_baseAddress}/categories");
        var category = new Frontend.Models.Category(name);
        var json = JsonSerializer.Serialize(category);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var result = await client.PostAsync(uri, payload);
        var responce = await result.Content.ReadAsStringAsync();
        return Deserialize<Frontend.Models.Category>(json);
    }

    async public static Task<Frontend.Models.Recipe> UpdateRecipe(Guid id, string name, List<string> ingredients, List<string> instructions, List<Guid> categoriesIds)
    {
        var uri = new Uri($"{_baseAddress}/recipes/{id}");
        var recipe = new Frontend.Models.Recipe(name, ingredients, instructions, categoriesIds);
        var json = JsonSerializer.Serialize(recipe);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var result = await client.PostAsync(uri, payload);
        var responce = await result.Content.ReadAsStringAsync();
        return Deserialize<Frontend.Models.Recipe>(json);
    }

    async public static Task<Frontend.Models.Category> UpdateCategory(Guid id, string name)
    {
        var uri = new Uri($"{_baseAddress}/categories/{id}");
        var category = new Frontend.Models.Category(name);
        var json = JsonSerializer.Serialize(category);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var result = await client.PostAsync(uri, payload);
        var responce = await result.Content.ReadAsStringAsync();
        return Deserialize<Frontend.Models.Category>(json);
    }

    async public static void DeleteRecipe(Guid id)
    {
        var uri = new Uri($"{_baseAddress}/recipes/{id}");
        using var client = new HttpClient();
        var result = await client.DeleteAsync(uri);
        var responce = await result.Content.ReadAsStringAsync();
    }

    async public static void DeleteCategory(Guid id)
    {
        var uri = new Uri($"{_baseAddress}/categories/{id}");
        using var client = new HttpClient();
        var result = await client.DeleteAsync(uri);
        var responce = await result.Content.ReadAsStringAsync();
    }
}
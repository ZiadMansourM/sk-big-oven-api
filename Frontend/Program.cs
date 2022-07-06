// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Text;
using System.Text.Json;

var app = new Frontend.Services.ConsoleService();
app.Run();

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

    public static List<Frontend.Models.Recipe> ListRecipes()
    {
        var uri = new Uri($"{_baseAddress}/recipes");
        using var client = new HttpClient();
        var json = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
        return Deserialize<List<Frontend.Models.Recipe>>(json);
    }

    public static List<Frontend.Models.Category> ListCategories()
    {
        var uri = new Uri($"{_baseAddress}/categories");
        using var client = new HttpClient();
        var json = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
        return Deserialize<List<Frontend.Models.Category>>(json);
    }

    public static Frontend.Models.Recipe GetRecipe(Guid id)
    {
        var uri = new Uri($"{_baseAddress}/recipes/{id}");
        using var client = new HttpClient();
        var json = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
        return Deserialize<Frontend.Models.Recipe>(json);
    }

    public static Frontend.Models.Category GetCategory(Guid id)
    {
        var uri = new Uri($"{_baseAddress}/categories/{id}");
        using var client = new HttpClient();
        var json = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;
        return Deserialize<Frontend.Models.Category>(json);
    }

    public static Frontend.Models.Recipe CreateRecipe(string name, List<string> ingredients, List<string> instructions, List<Guid> categoriesIds)
    {
        var uri = new Uri($"{_baseAddress}/recipes");
        var recipe = new Frontend.Models.Recipe(name, ingredients, instructions, categoriesIds);
        var json = JsonSerializer.Serialize(recipe);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var result = client.PostAsync(uri, payload).Result.Content.ReadAsStringAsync().Result;
        return Deserialize<Frontend.Models.Recipe>(json);
    }

    public static Frontend.Models.Category CreateCategory(string name)
    {
        var uri = new Uri($"{_baseAddress}/categories");
        var category = new Frontend.Models.Category(name);
        var json = JsonSerializer.Serialize(category);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var result = client.PostAsync(uri, payload).Result.Content.ReadAsStringAsync().Result;
        return Deserialize<Frontend.Models.Category>(json);
    }

    public static Frontend.Models.Recipe UpdateRecipe(Guid id, string name, List<string> ingredients, List<string> instructions, List<Guid> categoriesIds)
    {
        var uri = new Uri($"{_baseAddress}/recipes/{id}");
        var recipe = new Frontend.Models.Recipe(name, ingredients, instructions, categoriesIds);
        var json = JsonSerializer.Serialize(recipe);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var result = client.PutAsync(uri, payload).Result.Content.ReadAsStringAsync().Result;
        return Deserialize<Frontend.Models.Recipe>(json);
    }

    public static Frontend.Models.Category UpdateCategory(Guid id, string name)
    {
        var uri = new Uri($"{_baseAddress}/categories/{id}");
        var category = new Frontend.Models.Category(name);
        var json = JsonSerializer.Serialize(category);
        var payload = new StringContent(json, Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var result = client.PutAsync(uri, payload).Result.Content.ReadAsStringAsync().Result;
        return Deserialize<Frontend.Models.Category>(json);
    }

    public static void DeleteRecipe(Guid id)
    {
        var uri = new Uri($"{_baseAddress}/recipes/{id}");
        using var client = new HttpClient();
        var result = client.DeleteAsync(uri).Result.Content.ReadAsStringAsync().Result;
        //Console.WriteLine(result);
    }

    public static void DeleteCategory(Guid id)
    {
        var uri = new Uri($"{_baseAddress}/categories/{id}");
        using var client = new HttpClient();
        var result = client.DeleteAsync(uri).Result.Content.ReadAsStringAsync().Result;
        //Console.WriteLine(result);
    }
}

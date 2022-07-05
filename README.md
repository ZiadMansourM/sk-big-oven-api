# 🫡 Welcome to BigOven API Using .NET 🤖
This is my second task in my internship at [SilverKey](https://www.silverkeytech.com/). 

# 🦦 Checklist of the day
- [X] Studied this [folder](https://github.com/dodyg/practical-aspnetcore/tree/net6.0/projects/minimal-api).
- [X] Implemented API
```Console
// Recipe
- GET /recipes
- POST /recipes
- GET /recipes/{id:guid}
- PUT /recipes/{id:guid}
- DELETE /recipes/{id:guid}
// Category
- GET /categories
- POST /categories
- GET /categories/{id:guid}
- PUT /categories/{id:guid}
- DELETE /categories/{id:guid}
```
- [X] Added Swagger at /swagger/index.html
- [X] Tried to understand how to call our API from console app.
```c#
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
```
- [X] Updated ConsoleService
```
- Use Guid in Operations, still hide it from users.
```
- [X] Added json files Paths to appsettings.json
- [ ] ignore appsettings.json.
- [ ] Fix instructions display format.
- [ ] asynchronous programming.
```
- I have written programs with multiple processes and threads, and understands CPU/IO bound.
- I struggled a lot previously trying to tackle asynchronous programming. 
```

# Structure of BigOven:
```Console
ziadh@Ziads-MacBook-Air BigOven % tree -I obj -I bin
.
├── Backend
│   ├── Backend.csproj
│   ├── Categories.json
│   ├── Models
│   │   ├── Category.cs
│   │   └── Recipe.cs
│   ├── Program.cs
│   ├── Properties
│   │   └── launchSettings.json
│   ├── Recipes.json
│   ├── Services
│   │   └── JsonService.cs
│   ├── appsettings.Development.json
│   └── appsettings.json
├── BigOven.sln
├── Frontend
│   ├── Frontend.csproj
│   ├── Models
│   │   ├── Category.cs
│   │   └── Recipe.cs
│   ├── Program.cs
│   ├── Properties
│   │   └── launchSettings.json
│   ├── Services
│   │   └── ConsoleService.cs
│   ├── appsettings.Development.json
│   └── appsettings.json
└── REAMME.md

8 directories, 20 files
```

> I haven't included screenshots because there were no changes in the UI today.
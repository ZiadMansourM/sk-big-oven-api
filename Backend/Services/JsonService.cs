// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Text.Json;

namespace Backend.Services;

public class JsonService
{
    private readonly string _fileNameCategories;
    private readonly string _fileNameRecipes;

    public JsonService(string recipesPath, string categoriesPath)
    {
        if (string.IsNullOrEmpty(recipesPath) || string.IsNullOrEmpty(categoriesPath))
        {
            throw new ArgumentException("Error in appsettings.json");
        }
        // Recipes
        _fileNameRecipes = recipesPath;
        if (!File.Exists(_fileNameRecipes))
        {
            File.WriteAllText(_fileNameRecipes, "[]");
        }
        // Categories
        _fileNameCategories = categoriesPath;
        if (!File.Exists(_fileNameCategories))
        {
            File.WriteAllText(_fileNameCategories, "[]");
        }
    }

    public void OverWriteCategories(List<Models.Category> categories)
    {
        var newString = JsonSerializer.Serialize(categories);
        File.WriteAllText(_fileNameCategories, newString);
    }

    public void OverWriteRecipes(List<Models.Recipe> recipes)
    {
        var newString = JsonSerializer.Serialize(recipes);
        File.WriteAllText(_fileNameRecipes, newString);
    }

    public string ReadCategories()
    {
        return File.ReadAllText(_fileNameCategories);
    }

    public string ReadRecipes()
    {
        return File.ReadAllText(_fileNameRecipes);
    }

    public Models.Category SaveCategories(Models.Category category)
    {
        var oldString = ReadCategories();
        var categories = JsonSerializer.Deserialize<List<Models.Category>>(oldString)!;
        categories.Add(category);
        OverWriteCategories(categories);
        return category;
    }

    public Models.Recipe SaveRecipes(Models.Recipe recipe)
    {
        var oldString = ReadRecipes();
        var recipes = JsonSerializer.Deserialize<List<Models.Recipe>>(oldString)!;
        recipes.Add(recipe);
        OverWriteRecipes(recipes);
        return recipe;
    }

    public Models.Category CreateCategory(string name)
    {
        return SaveCategories(new Models.Category(name));
    }

    public Models.Recipe CreateRecipe(string name, List<string> ingredients, List<string> instructions, List<Guid> categoriesIds)
    {
        return SaveRecipes(new Models.Recipe(name, ingredients, instructions, categoriesIds));
    }

    public void DeleteCategory(Guid id)
    {
        // Cascade to Recipe
        var category = ListCategories().Where(c => c.Id == id).First();
        var jsonString = ReadRecipes();
        var recipes = JsonSerializer.Deserialize<List<Models.Recipe>>(jsonString)!;
        foreach (var recipe in recipes)
        {
            recipe.CategoriesIds.Remove(category.Id);
        }

        OverWriteRecipes(recipes);
        // Delete Category
        var categories = ListCategories().FindAll(c => c.Id != id);
        OverWriteCategories(categories);
    }

    public void DeleteRecipe(Guid id)
    {
        var recipes = ListRecipes().FindAll(r => r.Id != id);
        OverWriteRecipes(recipes);
    }

    public Models.Category GetCategory(Guid id)
    {
        var oldString = ReadCategories();
        var categories = JsonSerializer.Deserialize<List<Models.Category>>(oldString)!;
        return categories.Where(c => c.Id == id).First();
    }

    public Models.Recipe GetRecipe(Guid id)
    {
        var oldString = ReadRecipes();
        var recipes = JsonSerializer.Deserialize<List<Models.Recipe>>(oldString)!;
        return recipes.Where(r => r.Id == id).First();
    }

    public List<Models.Category> ListCategories()
    {
        var jsonString = ReadCategories();
        var categories = JsonSerializer.Deserialize<List<Models.Category>>(jsonString)!;
        return categories;
    }

    public List<Models.Recipe> ListRecipes()
    {
        var jsonString = ReadRecipes();
        var recipes = JsonSerializer.Deserialize<List<Models.Recipe>>(jsonString)!;
        return recipes;
    }

    public Models.Category UpdateCategory(Guid id, string name)
    {
        var categories = ListCategories().FindAll(c => c.Id != id);
        var category = ListCategories().Where(c => c.Id == id).First();
        category.Name = name;
        categories.Add(category);
        OverWriteCategories(categories);
        return category;
    }

    public Models.Recipe UpdateRecipe(Guid id, string name, List<string> ingredients, List<string> instructions, List<Guid> categoriesIds)
    {
        var recipes = ListRecipes().FindAll(r => r.Id != id);
        var recipe = ListRecipes().Where(r => r.Id == id).First();
        recipe.Name = name;
        recipe.Ingredients = ingredients;
        recipe.Instructions = instructions;
        recipe.CategoriesIds = categoriesIds;
        recipes.Add(recipe);
        OverWriteRecipes(recipes);
        return recipe;
    }
}


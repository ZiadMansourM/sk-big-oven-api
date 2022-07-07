// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Spectre.Console;
namespace Frontend.Services;

public class ConsoleService
{
    private readonly Dictionary<string, Func<Task>> _modeSelector;

    public ConsoleService()
    {
        _modeSelector = new Dictionary<string, Func<Task>>
        {
            ["Categories"] = Painter.CategoriesMain,
            ["Recipes"] = Painter.RecipesMain,
            ["Exit"] = () => Painter.Exit(),
        };
    }

    async public Task Run()
    {
        Painter.Setup("BigOven ...");
        Painter.WriteDivider("Talk to me ^^", true);
        while (true)
            await _modeSelector[Painter.GetMode()]();
    }
}

public class Painter
{
    private static readonly Dictionary<string, Func<Task>> _recipeSelector = new()
    {
        ["List"] = ListRecipes,
        ["Get"] = GetRecipe,
        ["Create"] = CreateRecipe,
        ["Update"] = UpdateRecipe,
        ["Delete"] = DeleteRecipe,
    };
    private static readonly Dictionary<string, Func<Task>> _categorySelector = new()
    {
        ["List"] = ListCategories,
        ["Get"] = GetCategory,
        ["Create"] = CreateCategory,
        ["Update"] = UpdateCategory,
        ["Delete"] = DeleteCategory,
    };

    public static void Setup(string name)
    {
        AnsiConsole.Write(
            new Panel(
                new FigletText(name)
                .Centered()
                .Color(Color.Red)
            ).RoundedBorder()
        );
    }

    public static void WriteDivider(string text, bool center = false)
    {
        AnsiConsole.WriteLine();
        var rule = new Rule($"[yellow]{text}[/]")
            .RuleStyle("grey");
        if (center)
        {
            rule = rule.Centered();
        }
        else
        {
            rule = rule.LeftAligned();
        }

        AnsiConsole.Write(rule);
    }

    public static string GetMode()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("What do you [green] want to do today ?[/]")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more Modes)[/]")
            .AddChoices(new[] {
                "Recipes", "Categories", "Exit"
            })
        );
    }

    public static T Ask<T>(string question)
    {
        return AnsiConsole.Prompt<T>(
            new TextPrompt<T>(question)
            .PromptStyle("red")
        );
    }

    public static List<string> AskMultiLine(string flag)
    {
        AnsiConsole.Markup($"What's the [green]recipe {flag}[/]?\n");
        AnsiConsole.Markup("[grey](Press Enter to add continue adding or write DONE to exit)[/]\n");
        List<string> flagList = new();
        var i = 1;
        while (true)
        {
            var input = AnsiConsole.Prompt(new TextPrompt<string>($"\n{i} - ")).Trim();
            if (input.ToUpper() == "DONE")
            {
                break;
            }

            flagList.Add(input);
            i++;
        }

        return flagList;
    }

    public static void DrawCategories(List<Models.Category> categories, string message)
    {
        AnsiConsole.Write(
            new Rule($"[yellow]{message}[/]")
            .RuleStyle("grey")
            .LeftAligned()
        );
        var table = new Table()
            .AddColumns("[grey]id[/]", "[grey]Name[/]")
            .RoundedBorder()
            .BorderColor(Color.Grey);
        var i = 1;
        foreach (var category in categories)
        {
            table = table.AddRow($"[grey]{i}[/]", category.Name);
            i++;
        }

        AnsiConsole.Write(table);
    }

    async public static void DrawRecipes(List<Models.Recipe> recipes, string message)
    {
        AnsiConsole.Write(
            new Rule($"[yellow]{message}[/]")
            .RuleStyle("grey")
            .LeftAligned()
        );
        var table = new Table()
            .AddColumns("[grey]Id[/]", "[grey]Title[/]", "[grey]Ingredients[/]", "[grey]Instructions[/]", "[grey]Categories[/]")
            .RoundedBorder()
            .BorderColor(Color.Grey);
        var i = 1;
        foreach (var recipe in recipes)
        {
            // Prepare data
            var categories = await Requests.ListCategories();
            Dictionary<Guid, string> categoriesDict = new();
            foreach (var category in categories)
                categoriesDict.Add(category.Id, category.Name);
            List<string> categoryNames = new();
            var j = 1;
            foreach (var guidId in recipe.CategoriesIds)
                categoryNames.Add($"{j++}) {categoriesDict[guidId]}");
            // Better Format ingredients
            List<string> ing = new();
            j = 1;
            foreach (var ingredient in recipe.Ingredients)
                ing.Add($"{j++}) {ingredient}");
            // Better Format instructions
            List<string> ins = new();
            j = 1;
            foreach (var instruction in recipe.Instructions)
                ins.Add($"{j++}) {instruction}");
            // Draw
            table = table.AddRow(
                $"{i}",
                $"[grey]{recipe.Name}[/]",
                string.Join("\n", ing),
                string.Join("\n", ins),
                string.Join("\n", categoryNames)
            );
            i++;
            table.AddEmptyRow();
        }
        AnsiConsole.Write(table);
    }

    async public static Task RecipesMain()
    {
        var request = AnsiConsole.Prompt(
           new SelectionPrompt<string>()
           .Title("About [red]recipes[/], [green] pick what want?[/]")
           .PageSize(10)
           .MoreChoicesText("[grey](Move up and down to reveal more Options)[/]")
           .AddChoices(new[] {
                "List", "Get", "Create", "Update", "Delete"
           })
       );
       await _recipeSelector[request]();
    }

    async public static Task CategoriesMain()
    {
        var request = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("About categories, [green] pick what want?[/]")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more Options)[/]")
            .AddChoices(new[] {
                "List", "Get", "Create", "Update", "Delete"
            })
        );
        await _categorySelector[request]();
    }

    public static Task Exit()
    {
        Environment.Exit(0);
        return null;
    }

    async public static Task<Guid> GetRecipeGuid()
    {
        var recipes = await Requests.ListRecipes();
        Dictionary<string, Guid> recipesDict = new();
        foreach (var recipe in recipes)
            recipesDict.Add(recipe.Name, recipe.Id);
        var name = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Which one '[green]Pick Recipe[/]' ?")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more Modes)[/]")
            .AddChoices(recipesDict.Keys.ToList())
        );
        return recipesDict[name];
    }

    async public static Task<Guid> GetCategoryGuid()
    {
        var categories = await  Requests.ListCategories();
        Dictionary<string, Guid> categoriesDict = new();
        foreach (var category in categories)
            categoriesDict.Add(category.Name, category.Id);
        var name = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Which one '[green]Pick Category[/]' ?")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more Modes)[/]")
            .AddChoices(categoriesDict.Keys.ToList())
        );
        return categoriesDict[name];
    }

    // Recipes
    async public static Task ListRecipes()
    {
        var recipes = await Requests.ListRecipes();
        DrawRecipes(recipes, "Recipes List");
    }

    async public static Task GetRecipe()
    {
        var guid = await GetRecipeGuid();
        var recipe = await Requests.GetRecipe(guid);
        DrawRecipes(recipe.ToList(), "Here you are ^^");
    }

    async public static Task<List<Guid>> GetGuidsList()
    {
        var categories = await Requests.ListCategories();
        //DrawCategories(categories, "Categories List");
        Dictionary<string, Guid> categoriesNames = new();
        foreach (var category in categories)
            categoriesNames.Add(category.Name, category.Id);
        var Names = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
            .Title("choose [green]categories[/]?")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more categories)[/]")
            .InstructionsText(
                "[grey](Press [blue]<space>[/] to toggle a category, " +
                "[green]<enter>[/] to accept)[/]")
            .AddChoices(categoriesNames.Keys.ToList())
        );
        List<Guid> GuidsList = new();
        foreach (var n in Names)
            GuidsList.Add(categoriesNames[n]);
        return GuidsList;
    }

    async public static Task CreateRecipe()
    {
        // [Get Title]
        var name = Ask<string>("What's the [green]recipe name[/]?").Trim();
        // [validate doesn't exist]
        var respies = await Requests.ListRecipes();
        foreach (var r in respies)
        {
            if (name == r.Name)
            {
                AnsiConsole.Markup($"Recipe already, [red]exists[/]!\n");
                return;
            }
        }
        // [Get ingredients]
        var ingredients = AskMultiLine("ingredients");
        // [Get instructions]
        var instructions = AskMultiLine("instructions");
        // [get guids of categories]
        var guidsList = await GetGuidsList();
        // Create recipe
        var recipe = await Requests.CreateRecipe(name, ingredients, instructions, guidsList);
        DrawRecipes(recipe.ToList(), "Created Successfully ^^");
    }

    async public static Task UpdateRecipe()
    {
        var guid = await GetRecipeGuid();
        var recipe = await Requests.GetRecipe(guid);
        var fieldName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Which field you want to [green]Update[/]?")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to reveal more Modes)[/]")
            .AddChoices(new[] {
                "Name", "Ingredients", "Instructions", "Categories"
            })
        );
        switch (fieldName)
        {
            case "Name":
                var name = Ask<string>("What's the Recipe [green]Name[/]?").Trim();
                recipe.Name = name;
                break;
            case "Ingredients":
                var ingredients = AskMultiLine("ingredients");
                recipe.Ingredients = ingredients;
                break;
            case "Instructions":
                var instructions = AskMultiLine("instructions");
                recipe.Instructions = instructions;
                break;
            case "Categories":
                var guidsList = await GetGuidsList();
                recipe.CategoriesIds = guidsList;
                break;
        }
        var updatedRecipe = await Requests.UpdateRecipe(
            guid,
            recipe.Name,
            recipe.Ingredients,
            recipe.Ingredients,
            recipe.CategoriesIds
        );
        DrawRecipes(updatedRecipe.ToList(), "Created Successfully ^^");
    }

    async public static Task DeleteRecipe()
    {
        var guid = await GetRecipeGuid();
        Requests.DeleteRecipe(guid);
        DrawRecipes(await Requests.ListRecipes(), "Done ^^");
    }

    //// Categories
    async public static Task ListCategories()
    {
        var categories = await Requests.ListCategories();
        DrawCategories(categories, "Categories List");
    }

    async public static Task GetCategory()
    {
        var guid = await GetCategoryGuid();
        var category = await Requests.GetCategory(guid);
        DrawCategories(category.ToList(), "Here you are ^^");
    }

    async public static Task CreateCategory()
    {
        var name = Ask<string>("What's the [green]Category name?[/]").Trim();
        var categories = await Requests.ListCategories();
        foreach (var c in categories)
        {
            if (name == c.Name)
            {
                AnsiConsole.Markup($"Category already, [red]exists[/]!\n");
                return;
            }
        }
        var category = await Requests.CreateCategory(name);
        DrawCategories(category.ToList(), "Created Successfully ^^");
    }

    async public static Task UpdateCategory()
    {
        var guid = await GetCategoryGuid();
        var name = Ask<string>("What's the new [green]Name?[/]").Trim();
        var category = await Requests.UpdateCategory(guid, name);
        DrawCategories(category.ToList(), "Updated Successfully ^^");
    }

    async public static Task DeleteCategory()
    {
        var guid = await GetCategoryGuid();
        Requests.DeleteCategory(guid);
        DrawCategories(await Requests.ListCategories(), "Done ^^");
    }
}

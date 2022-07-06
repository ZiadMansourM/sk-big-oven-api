﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Backend.Models;

public class Recipe
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<string> Ingredients { get; set; }
    public List<string> Instructions { get; set; }
    public List<Guid> CategoriesIds { get; set; }

    public Recipe(string name, List<string> ingredients, List<string> instructions, List<Guid> categoriesIds)
    {
        Id = Guid.NewGuid();
        Name = name;
        Ingredients = ingredients;
        Instructions = instructions;
        CategoriesIds = categoriesIds;
    }

    public List<Recipe> toList()
    {
        return new List<Recipe> { this };
    }
}

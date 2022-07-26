﻿using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend.Controllers
{
    public class RecipeController
    {
        private static List<Recipe> s_recipes { get; set; } = new List<Recipe>();
        private static List<string> s_categoriesNames { get; set; } = new List<string>();
        public RecipeController()
        {
            string fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            string jsonString = File.ReadAllText(fileName);
            s_recipes = JsonSerializer.Deserialize<List<Recipe>>(jsonString);
            fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
            jsonString = File.ReadAllText(fileName);
            s_categoriesNames = JsonSerializer.Deserialize<List<string>>(jsonString);
        }
        [HttpGet]
        [Route("api/list-recipes")]
        public List<Recipe> ListRecipes()
        {
            if (s_recipes.Count == 0)
                throw new InvalidOperationException("Cant be empty");
            else
                return s_recipes;
        }
        [HttpGet]
        [Route("api/list-categories")]
        public List<string> ListCategories()
        {
            if (s_categoriesNames.Count == 0)
                throw new InvalidOperationException("Cant be empty");
            else
                return s_categoriesNames;
        }
        [HttpPost]
        [Route("api/add-category/{category}")]
        public void AddCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new InvalidOperationException("Cant be empty");
            else
            {
                s_categoriesNames.Add(category);
                string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
                string jsonString = JsonSerializer.Serialize(s_categoriesNames);
                File.WriteAllText(fileName, jsonString);
            }
        }
        [HttpPost]
        [Route("api/add-recipe/{jsonRecipe}")]
        public void AddRecipe(string jsonRecipe)
        {
            Recipe recipe = JsonSerializer.Deserialize<Recipe>(jsonRecipe);
            recipe.Ingredients = recipe.Ingredients.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
            recipe.Instructions = recipe.Instructions.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
            if (recipe.Ingredients.Count == 0 || recipe.Instructions.Count == 0 || recipe.Categories.Count == 0 || string.IsNullOrWhiteSpace(recipe.Title))
                throw new InvalidOperationException("Cant be empty");
            else
            {
                s_recipes.Add(recipe);
                string fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
                string jsonString = JsonSerializer.Serialize(s_recipes);
                File.WriteAllText(fileName, jsonString);
            }
        }
        [HttpDelete]
        [Route("api/delete-category/{category}")]
        public void DeleteCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new InvalidOperationException("Cant be empty");
            else
            {
                s_categoriesNames.Remove(category);
                foreach (Recipe recipe in s_recipes)
                {
                    if (recipe.Categories.Contains(category))
                        recipe.Categories.Remove(category);
                }
                string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
                string jsonString = JsonSerializer.Serialize(s_categoriesNames);
                File.WriteAllText(fileName, jsonString);
                fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
                jsonString = JsonSerializer.Serialize(s_recipes);
                File.WriteAllText(fileName, jsonString);
            }
        }
        [HttpPut]
        [Route("api/update-category/{position}/{newCategory}")]
        public void UpdateCategory(string position, string newCategory)
        {
            if (string.IsNullOrEmpty(newCategory))
                throw new InvalidOperationException("Cant be empty");
            else
            {
                foreach (Recipe recipe in s_recipes)
                {
                    if (recipe.Categories.Contains(s_categoriesNames[int.Parse(position) - 1]))
                    {
                        recipe.Categories[recipe.Categories.IndexOf(s_categoriesNames[int.Parse(position) - 1])] = newCategory;
                    }
                }
                s_categoriesNames[int.Parse(position) - 1] = newCategory;
                string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
                string jsonString = JsonSerializer.Serialize(s_categoriesNames);
                File.WriteAllText(fileName, jsonString);
                fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
                jsonString = JsonSerializer.Serialize(s_recipes);
                File.WriteAllText(fileName, jsonString);
            }
        }
        [HttpDelete]
        [Route("api/delete-recipe/{id}")]
        public void DeleteRecipe(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidOperationException("Cant be empty");
            else
            {
                Recipe recipe = s_recipes.FirstOrDefault(x => x.Id == id);
                s_recipes.Remove(recipe);
                string fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
                var jsonString = JsonSerializer.Serialize(s_recipes);
                File.WriteAllText(fileName, jsonString);
            }
        }
        [HttpPut]
        [Route("api/update-recipe/{jsonRecipe}/{id}")]
        public void UpdateRecipe(string jsonRecipe, Guid id)
        {
            if (id == Guid.Empty || string.IsNullOrEmpty(jsonRecipe))
                throw new InvalidOperationException("Cant be empty");
            else
            {
                Recipe oldRecipe = s_recipes.FirstOrDefault(x => x.Id == id);
                Recipe newRecipe = JsonSerializer.Deserialize<Recipe>(jsonRecipe);
                newRecipe.Ingredients = newRecipe.Ingredients.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
                newRecipe.Instructions = newRecipe.Instructions.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
                if (newRecipe.Ingredients.Count == 0 || newRecipe.Instructions.Count == 0 || newRecipe.Categories.Count == 0 ||string.IsNullOrWhiteSpace(newRecipe.Title))
                    throw new InvalidOperationException("Cant be empty");
                else
                {
                    oldRecipe.Title = newRecipe.Title;
                    oldRecipe.Categories = newRecipe.Categories;
                    oldRecipe.Ingredients = newRecipe.Ingredients;
                    oldRecipe.Instructions = newRecipe.Instructions;
                    var fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
                    var jsonString = JsonSerializer.Serialize(s_recipes);
                    File.WriteAllText(fileName, jsonString);
                }
            }
        }
        [HttpGet]
        [Route("api/get-recipe/{id}")]
        public Recipe GetRecipe(Guid id)
        {
            if (id == Guid.Empty)
                throw new InvalidOperationException("Cant be empty");
            else
            {
                var recipe = s_recipes.FirstOrDefault(x => x.Id == id);
                return recipe;
            }
        }
        public string PathCombine(string path1, string path2)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }
            return Path.Combine(path1, path2);
        }
    }
}

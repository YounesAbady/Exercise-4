using Backend.Models;
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
            fileName = PathCombine(Environment.CurrentDirectory,@"\Categories.json");
            jsonString = File.ReadAllText(fileName);
            s_categoriesNames = JsonSerializer.Deserialize<List<string>>(jsonString);
        }
        [HttpGet]
        [Route("api/list-recipes")]
        public List<Recipe> ListRecipes()
        {
            return s_recipes;
        }
        [HttpGet]
        [Route("api/list-categories")]
        public List<string> ListCategories()
        {
            return s_categoriesNames;
        }
        [HttpPost]
        [Route("api/add-category/{category}")]
        public void AddCategory(string category)
        {
            s_categoriesNames.Add(category);
            string fileName = PathCombine(Environment.CurrentDirectory, @"\Categories.json");
            string jsonString = JsonSerializer.Serialize(s_categoriesNames);
            File.WriteAllText(fileName, jsonString);
        }
        [HttpPost]
        [Route("api/add-recipe/{jsonRecipe}")]
        public void AddRecipe(string jsonRecipe)
        {
            Recipe recipe = JsonSerializer.Deserialize<Recipe>(jsonRecipe);
            s_recipes.Add(recipe);
            string fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            string jsonString = JsonSerializer.Serialize(s_recipes);
            File.WriteAllText(fileName, jsonString);
        }
        [HttpDelete]
        [Route("api/delete-category/{category}")]
        public void DeleteCategory(string category)
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
        [HttpPut]
        [Route("api/update-category/{position}/{newCategory}")]
        public void UpdateCategory(string position, string newCategory)
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
        [HttpDelete]
        [Route("api/delete-recipe/{id}")]
        public void DeleteRecipe(Guid id)
        {
            Recipe recipe = s_recipes.FirstOrDefault(x => x.Id == id);
            s_recipes.Remove(recipe);
            string fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            var jsonString = JsonSerializer.Serialize(s_recipes);
            File.WriteAllText(fileName, jsonString);
        }
        [HttpPut]
        [Route("api/update-recipe/{jsonRecipe}/{id}")]
        public void UpdateRecipe(string jsonRecipe, Guid id)
        {
            Recipe oldRecipe = s_recipes.FirstOrDefault(x => x.Id == id);
            Recipe newRecipe = JsonSerializer.Deserialize<Recipe>(jsonRecipe);
            oldRecipe.Title = newRecipe.Title;
            oldRecipe.Categories = newRecipe.Categories;
            oldRecipe.Ingredients = newRecipe.Ingredients;
            oldRecipe.Instructions = newRecipe.Instructions;
            var fileName = PathCombine(Environment.CurrentDirectory, @"\Recipes.json");
            var jsonString = JsonSerializer.Serialize(s_recipes);
            File.WriteAllText(fileName, jsonString);
        }
        [HttpGet]
        [Route("api/get-recipe/{id}")]
        public Recipe GetRecipe(Guid id)
        {
            var recipe = s_recipes.FirstOrDefault(x => x.Id == id);
            return recipe;
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

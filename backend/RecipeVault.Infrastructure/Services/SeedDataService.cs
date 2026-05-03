using Microsoft.EntityFrameworkCore;
using RecipeVault.Core.Entities;
using RecipeVault.Infrastructure.Data;

namespace RecipeVault.Infrastructure.Services;

public class SeedDataService
{
    private readonly AppDbContext _db;

    public SeedDataService(AppDbContext db)
    {
        _db = db;
    }

    public async Task SeedForNewUserAsync(int userId)
    {
        var tagIds = await EnsureTagsAsync(
            "Italian", "Asian", "Mediterranean", "Mexican",
            "Breakfast", "Vegetarian", "Vegan", "Seafood",
            "Quick", "Healthy", "High Protein", "Gluten-Free", "Comfort Food"
        );

        var recipes = new List<(Recipe recipe, string[] tags)>
        {
            (new Recipe
            {
                UserId = userId,
                Name = "Spaghetti Bolognese",
                Description = "A classic Italian meat sauce slow-cooked and served over spaghetti.",
                ServingSize = 4,
                PrepTimeMinutes = 15,
                CookTimeMinutes = 45,
                CookCount = 0,
                Instructions = "1. Brown ground beef in a large pan over medium-high heat.\n2. Add diced onion and garlic, cook until soft.\n3. Stir in tomato paste and cook 2 minutes.\n4. Add crushed tomatoes, season with salt and pepper.\n5. Simmer on low heat for 30 minutes.\n6. Cook spaghetti according to package, serve with sauce.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Ground Beef", Quantity = 500, Unit = "g", IsStaple = false },
                    new() { Name = "Spaghetti", Quantity = 400, Unit = "g", IsStaple = false },
                    new() { Name = "Crushed Tomatoes", Quantity = 400, Unit = "g", IsStaple = false },
                    new() { Name = "Onion", Quantity = 1, Unit = "pc", IsStaple = true },
                    new() { Name = "Garlic", Quantity = 3, Unit = "cloves", IsStaple = true },
                    new() { Name = "Tomato Paste", Quantity = 2, Unit = "tbsp", IsStaple = true }
                }
            }, new[] { "Italian", "High Protein", "Comfort Food" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Chicken Tikka Masala",
                Description = "Tender chicken in a rich, spiced tomato cream sauce.",
                ServingSize = 4,
                PrepTimeMinutes = 20,
                CookTimeMinutes = 40,
                CookCount = 0,
                Instructions = "1. Marinate chicken in yogurt and spices for at least 1 hour.\n2. Grill or pan-fry chicken until cooked through.\n3. Sauté onions, garlic, and ginger until golden.\n4. Add spices, cook 1 minute, then add tomato purée.\n5. Simmer 10 minutes, stir in cream.\n6. Add chicken and simmer 10 more minutes. Serve with rice.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Chicken Breast", Quantity = 600, Unit = "g", IsStaple = false },
                    new() { Name = "Greek Yogurt", Quantity = 150, Unit = "ml", IsStaple = false },
                    new() { Name = "Tomato Purée", Quantity = 200, Unit = "ml", IsStaple = false },
                    new() { Name = "Heavy Cream", Quantity = 100, Unit = "ml", IsStaple = false },
                    new() { Name = "Garam Masala", Quantity = 2, Unit = "tsp", IsStaple = true },
                    new() { Name = "Garlic", Quantity = 4, Unit = "cloves", IsStaple = true }
                }
            }, new[] { "Asian", "High Protein" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Avocado Toast with Eggs",
                Description = "Creamy avocado on toasted bread topped with poached eggs.",
                ServingSize = 2,
                PrepTimeMinutes = 5,
                CookTimeMinutes = 10,
                CookCount = 0,
                Instructions = "1. Toast bread slices until golden.\n2. Mash avocado with lemon juice, salt, and pepper.\n3. Bring a pot of water to a gentle simmer, add a splash of vinegar.\n4. Crack eggs into water and poach 3 minutes.\n5. Spread avocado on toast and top with eggs.\n6. Season with chilli flakes and serve immediately.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Sourdough Bread", Quantity = 2, Unit = "slices", IsStaple = true },
                    new() { Name = "Avocado", Quantity = 1, Unit = "pc", IsStaple = false },
                    new() { Name = "Eggs", Quantity = 2, Unit = "pc", IsStaple = true },
                    new() { Name = "Lemon", Quantity = 0.5m, Unit = "pc", IsStaple = true }
                }
            }, new[] { "Breakfast", "Vegetarian", "Quick" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Greek Salad",
                Description = "A fresh salad with tomatoes, cucumber, olives, and feta cheese.",
                ServingSize = 2,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 0,
                CookCount = 0,
                Instructions = "1. Chop tomatoes, cucumber into chunks.\n2. Thinly slice red onion.\n3. Combine tomatoes, cucumber, onion, and olives in a bowl.\n4. Top with crumbled feta cheese.\n5. Drizzle with olive oil and sprinkle dried oregano.\n6. Season with salt and pepper, toss gently and serve.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Tomatoes", Quantity = 3, Unit = "pc", IsStaple = false },
                    new() { Name = "Cucumber", Quantity = 1, Unit = "pc", IsStaple = false },
                    new() { Name = "Feta Cheese", Quantity = 100, Unit = "g", IsStaple = false },
                    new() { Name = "Kalamata Olives", Quantity = 50, Unit = "g", IsStaple = false },
                    new() { Name = "Red Onion", Quantity = 0.5m, Unit = "pc", IsStaple = true },
                    new() { Name = "Olive Oil", Quantity = 2, Unit = "tbsp", IsStaple = true }
                }
            }, new[] { "Mediterranean", "Vegetarian", "Gluten-Free", "Quick" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Grilled Salmon with Vegetables",
                Description = "Juicy salmon fillet grilled alongside colourful roasted vegetables.",
                ServingSize = 2,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 20,
                CookCount = 0,
                Instructions = "1. Preheat oven to 200°C.\n2. Season salmon with olive oil, lemon zest, salt, and pepper.\n3. Chop vegetables and toss with olive oil and seasoning.\n4. Roast vegetables for 20 minutes.\n5. Grill or pan-fry salmon 4 minutes each side.\n6. Serve together with a wedge of lemon.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Salmon Fillet", Quantity = 300, Unit = "g", IsStaple = false },
                    new() { Name = "Zucchini", Quantity = 1, Unit = "pc", IsStaple = false },
                    new() { Name = "Bell Pepper", Quantity = 1, Unit = "pc", IsStaple = false },
                    new() { Name = "Cherry Tomatoes", Quantity = 100, Unit = "g", IsStaple = false },
                    new() { Name = "Olive Oil", Quantity = 2, Unit = "tbsp", IsStaple = true },
                    new() { Name = "Lemon", Quantity = 1, Unit = "pc", IsStaple = true }
                }
            }, new[] { "Seafood", "Healthy", "Gluten-Free" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Beef Tacos",
                Description = "Seasoned ground beef in warm tortillas with fresh toppings.",
                ServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 15,
                CookCount = 0,
                Instructions = "1. Brown ground beef in a pan over medium-high heat.\n2. Drain fat and add taco seasoning with a splash of water.\n3. Simmer 5 minutes until sauce thickens.\n4. Warm tortillas in a dry pan.\n5. Fill tortillas with beef, shredded lettuce, tomato, cheese, and sour cream.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Ground Beef", Quantity = 500, Unit = "g", IsStaple = false },
                    new() { Name = "Flour Tortillas", Quantity = 8, Unit = "pc", IsStaple = false },
                    new() { Name = "Taco Seasoning", Quantity = 2, Unit = "tbsp", IsStaple = true },
                    new() { Name = "Cheddar Cheese", Quantity = 100, Unit = "g", IsStaple = false },
                    new() { Name = "Sour Cream", Quantity = 80, Unit = "ml", IsStaple = false },
                    new() { Name = "Lettuce", Quantity = 1, Unit = "cup", IsStaple = false }
                }
            }, new[] { "Mexican", "Quick", "High Protein" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Mushroom Risotto",
                Description = "Creamy Italian rice dish loaded with sautéed mushrooms and parmesan.",
                ServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 35,
                CookCount = 0,
                Instructions = "1. Heat stock in a separate pot and keep warm.\n2. Sauté shallots and garlic in butter until soft.\n3. Add mushrooms and cook until golden.\n4. Add arborio rice, stir to coat, then add white wine.\n5. Add stock one ladle at a time, stirring continuously.\n6. Stir in parmesan and butter, season and serve.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Arborio Rice", Quantity = 320, Unit = "g", IsStaple = false },
                    new() { Name = "Mushrooms", Quantity = 300, Unit = "g", IsStaple = false },
                    new() { Name = "Vegetable Stock", Quantity = 1000, Unit = "ml", IsStaple = true },
                    new() { Name = "Parmesan", Quantity = 60, Unit = "g", IsStaple = false },
                    new() { Name = "White Wine", Quantity = 100, Unit = "ml", IsStaple = false },
                    new() { Name = "Butter", Quantity = 30, Unit = "g", IsStaple = true }
                }
            }, new[] { "Italian", "Vegetarian", "Comfort Food" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Overnight Oats",
                Description = "No-cook oats soaked overnight in oat milk, topped with fruit and seeds.",
                ServingSize = 1,
                PrepTimeMinutes = 5,
                CookTimeMinutes = 0,
                CookCount = 0,
                Instructions = "1. Combine oats and oat milk in a jar or bowl.\n2. Add chia seeds and maple syrup, stir well.\n3. Cover and refrigerate overnight.\n4. In the morning, top with fresh berries and banana slices.\n5. Add a spoonful of nut butter if desired.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Rolled Oats", Quantity = 80, Unit = "g", IsStaple = true },
                    new() { Name = "Oat Milk", Quantity = 150, Unit = "ml", IsStaple = false },
                    new() { Name = "Chia Seeds", Quantity = 1, Unit = "tbsp", IsStaple = true },
                    new() { Name = "Mixed Berries", Quantity = 100, Unit = "g", IsStaple = false },
                    new() { Name = "Maple Syrup", Quantity = 1, Unit = "tbsp", IsStaple = true }
                }
            }, new[] { "Breakfast", "Vegan", "Healthy", "Quick" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Beef Stir Fry",
                Description = "Tender strips of beef and crisp vegetables tossed in a savoury sauce.",
                ServingSize = 4,
                PrepTimeMinutes = 15,
                CookTimeMinutes = 15,
                CookCount = 0,
                Instructions = "1. Slice beef thinly against the grain and marinate in soy sauce.\n2. Mix sauce: soy sauce, oyster sauce, cornstarch, and sesame oil.\n3. Heat wok over high heat, stir-fry beef until browned, set aside.\n4. Stir-fry vegetables for 2-3 minutes.\n5. Return beef, pour over sauce and toss to coat.\n6. Serve immediately over steamed rice.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Beef Sirloin", Quantity = 400, Unit = "g", IsStaple = false },
                    new() { Name = "Broccoli", Quantity = 200, Unit = "g", IsStaple = false },
                    new() { Name = "Snap Peas", Quantity = 100, Unit = "g", IsStaple = false },
                    new() { Name = "Soy Sauce", Quantity = 3, Unit = "tbsp", IsStaple = true },
                    new() { Name = "Oyster Sauce", Quantity = 2, Unit = "tbsp", IsStaple = true },
                    new() { Name = "Sesame Oil", Quantity = 1, Unit = "tbsp", IsStaple = true }
                }
            }, new[] { "Asian", "Quick", "High Protein" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Margherita Pizza",
                Description = "Classic pizza with tomato sauce, fresh mozzarella, and basil.",
                ServingSize = 2,
                PrepTimeMinutes = 20,
                CookTimeMinutes = 15,
                CookCount = 0,
                Instructions = "1. Preheat oven to 250°C with a pizza stone if available.\n2. Stretch pizza dough into a round.\n3. Spread tomato sauce evenly, leaving a border.\n4. Tear mozzarella and scatter over sauce.\n5. Bake 12-15 minutes until crust is golden.\n6. Top with fresh basil leaves and a drizzle of olive oil.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Pizza Dough", Quantity = 250, Unit = "g", IsStaple = false },
                    new() { Name = "Tomato Sauce", Quantity = 80, Unit = "ml", IsStaple = true },
                    new() { Name = "Fresh Mozzarella", Quantity = 150, Unit = "g", IsStaple = false },
                    new() { Name = "Fresh Basil", Quantity = 10, Unit = "leaves", IsStaple = false },
                    new() { Name = "Olive Oil", Quantity = 1, Unit = "tbsp", IsStaple = true }
                }
            }, new[] { "Italian", "Vegetarian", "Comfort Food" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Lentil Soup",
                Description = "Hearty red lentil soup spiced with cumin and finished with lemon.",
                ServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 30,
                CookCount = 0,
                Instructions = "1. Sauté onion, garlic, and carrot in olive oil until soft.\n2. Add cumin, coriander, and turmeric, cook 1 minute.\n3. Add red lentils and vegetable stock.\n4. Bring to boil then simmer 25 minutes until lentils break down.\n5. Blend partially for a creamy texture.\n6. Finish with lemon juice and season to taste.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Red Lentils", Quantity = 250, Unit = "g", IsStaple = false },
                    new() { Name = "Vegetable Stock", Quantity = 1200, Unit = "ml", IsStaple = true },
                    new() { Name = "Carrot", Quantity = 2, Unit = "pc", IsStaple = false },
                    new() { Name = "Cumin", Quantity = 2, Unit = "tsp", IsStaple = true },
                    new() { Name = "Onion", Quantity = 1, Unit = "pc", IsStaple = true },
                    new() { Name = "Lemon", Quantity = 1, Unit = "pc", IsStaple = true }
                }
            }, new[] { "Vegan", "Healthy", "Gluten-Free" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Chicken Fried Rice",
                Description = "A quick and satisfying fried rice with chicken, egg, and vegetables.",
                ServingSize = 4,
                PrepTimeMinutes = 10,
                CookTimeMinutes = 15,
                CookCount = 0,
                Instructions = "1. Cook and cool rice (day-old rice works best).\n2. Dice chicken and stir-fry until golden, set aside.\n3. Scramble eggs in the same wok and break into pieces.\n4. Add vegetables and stir-fry 2 minutes.\n5. Add rice and chicken, season with soy sauce.\n6. Stir-fry on high heat for 3 minutes and serve.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Cooked Rice", Quantity = 400, Unit = "g", IsStaple = true },
                    new() { Name = "Chicken Breast", Quantity = 300, Unit = "g", IsStaple = false },
                    new() { Name = "Eggs", Quantity = 2, Unit = "pc", IsStaple = true },
                    new() { Name = "Frozen Peas", Quantity = 100, Unit = "g", IsStaple = true },
                    new() { Name = "Soy Sauce", Quantity = 3, Unit = "tbsp", IsStaple = true },
                    new() { Name = "Spring Onion", Quantity = 2, Unit = "stalks", IsStaple = false }
                }
            }, new[] { "Asian", "Quick", "High Protein" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Banana Pancakes",
                Description = "Fluffy pancakes made with ripe banana — naturally sweet and satisfying.",
                ServingSize = 2,
                PrepTimeMinutes = 5,
                CookTimeMinutes = 15,
                CookCount = 0,
                Instructions = "1. Mash ripe banana in a bowl.\n2. Whisk in eggs, flour, milk, and a pinch of salt.\n3. Heat a non-stick pan over medium heat with a little butter.\n4. Pour in small rounds of batter and cook until bubbles form.\n5. Flip and cook 1 more minute.\n6. Serve with maple syrup and fresh fruit.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Banana", Quantity = 1, Unit = "pc", IsStaple = false },
                    new() { Name = "Eggs", Quantity = 2, Unit = "pc", IsStaple = true },
                    new() { Name = "Flour", Quantity = 100, Unit = "g", IsStaple = true },
                    new() { Name = "Milk", Quantity = 100, Unit = "ml", IsStaple = true },
                    new() { Name = "Maple Syrup", Quantity = 2, Unit = "tbsp", IsStaple = true }
                }
            }, new[] { "Breakfast", "Vegetarian", "Quick" }),

            (new Recipe
            {
                UserId = userId,
                Name = "Tomato Basil Pasta",
                Description = "A light and fresh pasta tossed with cherry tomatoes, garlic, and basil.",
                ServingSize = 2,
                PrepTimeMinutes = 5,
                CookTimeMinutes = 15,
                CookCount = 0,
                Instructions = "1. Cook pasta in well-salted boiling water, reserve some pasta water.\n2. Halve cherry tomatoes and sauté in olive oil with garlic.\n3. Season with salt, pepper, and a pinch of chilli flakes.\n4. Drain pasta and add to the pan with a splash of pasta water.\n5. Toss well, remove from heat.\n6. Stir in fresh basil and serve with grated parmesan.",
                CreatedAt = DateTime.UtcNow,
                Ingredients = new List<Ingredient>
                {
                    new() { Name = "Pasta", Quantity = 200, Unit = "g", IsStaple = true },
                    new() { Name = "Cherry Tomatoes", Quantity = 250, Unit = "g", IsStaple = false },
                    new() { Name = "Garlic", Quantity = 3, Unit = "cloves", IsStaple = true },
                    new() { Name = "Fresh Basil", Quantity = 15, Unit = "leaves", IsStaple = false },
                    new() { Name = "Olive Oil", Quantity = 3, Unit = "tbsp", IsStaple = true },
                    new() { Name = "Parmesan", Quantity = 30, Unit = "g", IsStaple = false }
                }
            }, new[] { "Italian", "Vegetarian", "Quick" })
        };

        foreach (var (recipe, tags) in recipes)
        {
            _db.Recipes.Add(recipe);
            await _db.SaveChangesAsync();

            foreach (var tagName in tags)
            {
                if (tagIds.TryGetValue(tagName, out var tagId))
                    _db.RecipeTags.Add(new RecipeTag { RecipeId = recipe.Id, TagId = tagId });
            }
        }

        await _db.SaveChangesAsync();
    }

    private async Task<Dictionary<string, int>> EnsureTagsAsync(params string[] names)
    {
        var result = new Dictionary<string, int>();

        foreach (var name in names)
        {
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == name);
            if (tag == null)
            {
                tag = new Tag { Name = name };
                _db.Tags.Add(tag);
                await _db.SaveChangesAsync();
            }
            result[name] = tag.Id;
        }

        return result;
    }
}

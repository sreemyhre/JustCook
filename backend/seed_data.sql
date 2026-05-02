-- =============================================================================
-- JustCook Seed Data — 30 Recipes for UserId 3
-- Run against: justcookdb (PostgreSQL)
-- Usage: psql -U postgres -d justcookdb -f seed_data.sql
--
-- Assumes UserId 3 already exists. If not, uncomment the Users insert below.
-- Uses RETURNING to avoid hardcoded IDs — safe to run alongside existing data.
-- =============================================================================

BEGIN;

DO $$
DECLARE
  -- Tag IDs
  t_veg      INT; t_vegan    INT; t_quick    INT; t_slow     INT;
  t_italian  INT; t_asian    INT; t_mexican  INT; t_med      INT;
  t_healthy  INT; t_comfort  INT; t_brekkie  INT; t_soup     INT;
  t_salad    INT; t_gf       INT; t_protein  INT; t_budget   INT;
  t_seafood  INT; t_dessert  INT;

  -- Recipe IDs
  r1  INT; r2  INT; r3  INT; r4  INT; r5  INT;
  r6  INT; r7  INT; r8  INT; r9  INT; r10 INT;
  r11 INT; r12 INT; r13 INT; r14 INT; r15 INT;
  r16 INT; r17 INT; r18 INT; r19 INT; r20 INT;
  r21 INT; r22 INT; r23 INT; r24 INT; r25 INT;
  r26 INT; r27 INT; r28 INT; r29 INT; r30 INT;

  -- Meal Plan IDs
  mp1 INT; mp2 INT;

BEGIN

  -- ===========================================================================
  -- OPTIONAL: create user 3 if it doesn't exist
  -- ===========================================================================
  -- INSERT INTO "Users" ("FirebaseUid", "Email", "FirstName", "LastName", "Provider", "CreatedAt")
  -- SELECT 'seed-firebase-uid-3', 'seed.user3@justcook.dev', 'Seed', 'User', 'password', NOW()
  -- WHERE NOT EXISTS (SELECT 1 FROM "Users" WHERE "Id" = 3);

  -- ===========================================================================
  -- TAGS  (upsert by name so duplicates are safely ignored)
  -- ===========================================================================
  INSERT INTO "Tags" ("Name") VALUES ('Vegetarian')    ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_veg;
  INSERT INTO "Tags" ("Name") VALUES ('Vegan')         ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_vegan;
  INSERT INTO "Tags" ("Name") VALUES ('Quick')         ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_quick;
  INSERT INTO "Tags" ("Name") VALUES ('Slow Cook')     ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_slow;
  INSERT INTO "Tags" ("Name") VALUES ('Italian')       ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_italian;
  INSERT INTO "Tags" ("Name") VALUES ('Asian')         ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_asian;
  INSERT INTO "Tags" ("Name") VALUES ('Mexican')       ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_mexican;
  INSERT INTO "Tags" ("Name") VALUES ('Mediterranean') ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_med;
  INSERT INTO "Tags" ("Name") VALUES ('Healthy')       ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_healthy;
  INSERT INTO "Tags" ("Name") VALUES ('Comfort Food')  ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_comfort;
  INSERT INTO "Tags" ("Name") VALUES ('Breakfast')     ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_brekkie;
  INSERT INTO "Tags" ("Name") VALUES ('Soup')          ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_soup;
  INSERT INTO "Tags" ("Name") VALUES ('Salad')         ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_salad;
  INSERT INTO "Tags" ("Name") VALUES ('Gluten-Free')   ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_gf;
  INSERT INTO "Tags" ("Name") VALUES ('High Protein')  ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_protein;
  INSERT INTO "Tags" ("Name") VALUES ('Budget-Friendly') ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_budget;
  INSERT INTO "Tags" ("Name") VALUES ('Seafood')       ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_seafood;
  INSERT INTO "Tags" ("Name") VALUES ('Dessert')       ON CONFLICT ("Name") DO UPDATE SET "Name" = EXCLUDED."Name" RETURNING "Id" INTO t_dessert;

  -- ===========================================================================
  -- RECIPES + INGREDIENTS + RECIPE TAGS
  -- DayOfWeek stored as int: Monday=0 … Sunday=6
  -- MealType stored as string: 'Breakfast' | 'Lunch' | 'Dinner'
  -- ===========================================================================

  -- 1. Spaghetti Bolognese
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Spaghetti Bolognese','A rich and hearty Italian meat sauce served over perfectly cooked spaghetti.',4,15,45,3,
    'Brown ground beef with onions and garlic. Add tomato sauce and simmer 30 mins. Cook spaghetti al dente. Serve sauce over pasta with grated parmesan.',
    NOW() - INTERVAL '45 days') RETURNING "Id" INTO r1;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r1,'Ground Beef',500,'g',false),(r1,'Spaghetti',400,'g',false),
    (r1,'Tomato Sauce',2,'cups',false),(r1,'Onion',1,'large',false),
    (r1,'Garlic',3,'cloves',true),(r1,'Olive Oil',2,'tbsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r1,t_italian),(r1,t_comfort),(r1,t_protein);

  -- 2. Chicken Tikka Masala
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Chicken Tikka Masala','Tender chicken in a creamy spiced tomato sauce — a British-Indian classic.',4,20,35,5,
    'Marinate chicken in tikka paste. Grill until charred. Make sauce with tomatoes, cream, and spices. Add chicken and simmer 15 mins. Serve with basmati rice.',
    NOW() - INTERVAL '30 days') RETURNING "Id" INTO r2;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r2,'Chicken Breast',600,'g',false),(r2,'Heavy Cream',1,'cup',false),
    (r2,'Tomato Puree',1,'cup',false),(r2,'Tikka Masala Paste',3,'tbsp',false),
    (r2,'Onion',1,'large',false),(r2,'Garlic',2,'cloves',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r2,t_asian),(r2,t_protein);

  -- 3. Avocado Toast with Eggs
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Avocado Toast with Eggs','Creamy smashed avocado on toasted sourdough topped with perfectly cooked eggs.',2,5,10,12,
    'Toast bread. Mash avocado with lemon, salt, and pepper. Fry or poach eggs. Spread avocado on toast and top with eggs. Finish with red pepper flakes.',
    NOW() - INTERVAL '10 days') RETURNING "Id" INTO r3;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r3,'Sourdough Bread',4,'slices',false),(r3,'Avocado',2,'whole',false),
    (r3,'Eggs',4,'whole',false),(r3,'Lemon Juice',1,'tbsp',true),
    (r3,'Salt',1,'tsp',true),(r3,'Red Pepper Flakes',0.5,'tsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r3,t_brekkie),(r3,t_veg),(r3,t_quick);

  -- 4. Greek Salad
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Greek Salad','Fresh vegetables, feta cheese, and olives dressed in olive oil and dried oregano.',4,10,0,7,
    'Chop tomatoes, cucumber, and red onion into chunks. Combine with olives and crumbled feta. Drizzle with olive oil and season generously with oregano and salt.',
    NOW() - INTERVAL '20 days') RETURNING "Id" INTO r4;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r4,'Tomatoes',4,'whole',false),(r4,'Cucumber',1,'whole',false),
    (r4,'Feta Cheese',200,'g',false),(r4,'Kalamata Olives',0.5,'cup',false),
    (r4,'Red Onion',1,'whole',false),(r4,'Olive Oil',3,'tbsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r4,t_med),(r4,t_veg),(r4,t_salad),(r4,t_gf);

  -- 5. Beef Tacos
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Beef Tacos','Seasoned ground beef in crispy taco shells with all your favourite toppings.',4,10,15,8,
    'Brown ground beef with taco seasoning. Warm taco shells in oven. Fill shells with meat, shredded cheese, lettuce, salsa, and sour cream.',
    NOW() - INTERVAL '15 days') RETURNING "Id" INTO r5;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r5,'Ground Beef',500,'g',false),(r5,'Taco Shells',12,'whole',false),
    (r5,'Salsa',0.5,'cup',false),(r5,'Shredded Cheese',1,'cup',false),
    (r5,'Shredded Lettuce',2,'cups',false),(r5,'Sour Cream',0.5,'cup',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r5,t_mexican),(r5,t_quick);

  -- 6. Tomato Basil Soup
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Tomato Basil Soup','Velvety roasted tomato soup with fresh basil and a swirl of cream.',4,10,30,4,
    'Sauté garlic in olive oil. Add crushed tomatoes and broth, simmer 20 mins. Blend until smooth. Stir in cream and fresh basil. Season to taste.',
    NOW() - INTERVAL '25 days') RETURNING "Id" INTO r6;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r6,'Crushed Tomatoes',2,'cans',false),(r6,'Vegetable Broth',2,'cups',false),
    (r6,'Fresh Basil',0.5,'cup',false),(r6,'Garlic',3,'cloves',true),
    (r6,'Olive Oil',2,'tbsp',true),(r6,'Heavy Cream',0.25,'cup',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r6,t_soup),(r6,t_veg),(r6,t_italian);

  -- 7. Grilled Salmon with Vegetables
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Grilled Salmon with Vegetables','Herb-seasoned salmon fillets grilled alongside colourful seasonal vegetables.',2,10,20,6,
    'Season salmon with garlic powder, salt, and lemon zest. Toss vegetables in olive oil. Grill salmon 4 mins each side. Grill vegetables until tender. Serve with lemon wedges.',
    NOW() - INTERVAL '5 days') RETURNING "Id" INTO r7;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r7,'Salmon Fillets',2,'whole',false),(r7,'Zucchini',1,'whole',false),
    (r7,'Bell Peppers',2,'whole',false),(r7,'Lemon',1,'whole',false),
    (r7,'Olive Oil',2,'tbsp',true),(r7,'Garlic Powder',1,'tsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r7,t_seafood),(r7,t_healthy),(r7,t_gf);

  -- 8. Margherita Pizza
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Margherita Pizza','Classic Neapolitan pizza with tomato sauce, fresh mozzarella, and basil.',4,20,15,9,
    'Stretch dough on a floured surface. Spread tomato sauce. Add torn mozzarella. Bake at 250°C for 12–15 mins until crust is golden and cheese is bubbling. Top with fresh basil.',
    NOW() - INTERVAL '40 days') RETURNING "Id" INTO r8;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r8,'Pizza Dough',1,'ball',false),(r8,'Tomato Sauce',1,'cup',false),
    (r8,'Fresh Mozzarella',300,'g',false),(r8,'Fresh Basil',0.5,'cup',false),
    (r8,'Olive Oil',1,'tbsp',true),(r8,'Salt',1,'tsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r8,t_italian),(r8,t_veg);

  -- 9. Chicken Caesar Salad
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Chicken Caesar Salad','Crispy romaine with grilled chicken, shaved parmesan, croutons, and creamy Caesar dressing.',2,15,20,11,
    'Season and grill chicken breast until cooked through. Rest 5 mins then slice. Toss romaine with Caesar dressing. Add croutons and parmesan. Top with sliced chicken.',
    NOW() - INTERVAL '3 days') RETURNING "Id" INTO r9;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r9,'Chicken Breast',400,'g',false),(r9,'Romaine Lettuce',1,'head',false),
    (r9,'Caesar Dressing',0.25,'cup',false),(r9,'Parmesan Cheese',0.5,'cup',false),
    (r9,'Croutons',1,'cup',false),(r9,'Lemon',1,'whole',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r9,t_salad),(r9,t_protein);

  -- 10. Overnight Oats
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Overnight Oats','No-cook oats soaked overnight for a creamy, grab-and-go breakfast.',1,5,0,15,
    'Combine oats, almond milk, chia seeds, and maple syrup in a jar. Stir well. Seal and refrigerate overnight. In the morning, top with sliced banana and fresh berries.',
    NOW() - INTERVAL '2 days') RETURNING "Id" INTO r10;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r10,'Rolled Oats',0.5,'cup',false),(r10,'Almond Milk',0.75,'cup',false),
    (r10,'Chia Seeds',1,'tbsp',false),(r10,'Maple Syrup',1,'tbsp',false),
    (r10,'Banana',1,'whole',false),(r10,'Mixed Berries',0.5,'cup',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r10,t_brekkie),(r10,t_vegan),(r10,t_healthy);

  -- 11. Beef Stir Fry
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Beef Stir Fry','Tender beef strips with colourful vegetables in a savory soy-ginger sauce.',4,15,10,6,
    'Slice beef thin against the grain. Mix soy sauce, sesame oil, and ginger for the sauce. Cook beef in a hot wok until browned, remove. Stir-fry vegetables 3 mins. Return beef, add sauce, toss. Serve over rice.',
    NOW() - INTERVAL '8 days') RETURNING "Id" INTO r11;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r11,'Beef Strips',500,'g',false),(r11,'Mixed Vegetables',3,'cups',false),
    (r11,'Soy Sauce',3,'tbsp',true),(r11,'Sesame Oil',1,'tbsp',false),
    (r11,'Fresh Ginger',1,'tsp',false),(r11,'Garlic',2,'cloves',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r11,t_asian),(r11,t_quick),(r11,t_protein);

  -- 12. Veggie Buddha Bowl
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Veggie Buddha Bowl','A nourishing bowl with quinoa, roasted sweet potato, crispy chickpeas, and tahini dressing.',2,20,30,4,
    'Cook quinoa per packet. Toss chickpeas and diced sweet potato with olive oil and roast at 200°C for 25 mins. Massage kale with lemon juice. Assemble bowls and drizzle generously with tahini.',
    NOW() - INTERVAL '12 days') RETURNING "Id" INTO r12;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r12,'Quinoa',1,'cup',false),(r12,'Chickpeas',1,'can',false),
    (r12,'Sweet Potato',1,'whole',false),(r12,'Kale',2,'cups',false),
    (r12,'Tahini',2,'tbsp',false),(r12,'Lemon',1,'whole',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r12,t_vegan),(r12,t_healthy),(r12,t_gf);

  -- 13. French Onion Soup
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'French Onion Soup','Slowly caramelised onions in rich beef broth, topped with toasted baguette and melted Gruyère.',4,10,60,2,
    'Caramelise onions in butter over low heat for 40 mins. Add broth and thyme, simmer 15 mins. Ladle into oven-safe bowls. Top with a baguette slice and Gruyère. Broil until cheese is bubbling.',
    NOW() - INTERVAL '60 days') RETURNING "Id" INTO r13;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r13,'Onions',4,'large',false),(r13,'Beef Broth',4,'cups',false),
    (r13,'Gruyère Cheese',200,'g',false),(r13,'Baguette',4,'slices',false),
    (r13,'Butter',3,'tbsp',true),(r13,'Fresh Thyme',1,'tsp',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r13,t_soup),(r13,t_comfort);

  -- 14. Banana Pancakes
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Banana Pancakes','Fluffy golden pancakes with mashed banana baked right in, served with maple syrup.',2,5,15,10,
    'Mash bananas in a bowl. Whisk in eggs, milk, flour, and baking powder until just combined. Heat a buttered non-stick pan. Pour ¼ cup batter per pancake. Cook until bubbles form, then flip. Serve with maple syrup.',
    NOW() - INTERVAL '7 days') RETURNING "Id" INTO r14;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r14,'Bananas',2,'whole',false),(r14,'Eggs',2,'whole',false),
    (r14,'All-Purpose Flour',1,'cup',true),(r14,'Milk',0.75,'cup',false),
    (r14,'Baking Powder',1,'tsp',true),(r14,'Maple Syrup',2,'tbsp',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r14,t_brekkie),(r14,t_veg);

  -- 15. Shrimp Fried Rice
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Shrimp Fried Rice','Restaurant-style fried rice with juicy shrimp, scrambled eggs, and a soy-sesame finish.',4,15,15,7,
    'Cook shrimp in a hot wok and set aside. Scramble eggs in the wok. Add cold cooked rice and stir-fry 3 mins. Add soy sauce and sesame oil. Return shrimp. Top with sliced green onions.',
    NOW() - INTERVAL '18 days') RETURNING "Id" INTO r15;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r15,'Shrimp',400,'g',false),(r15,'Cooked Rice',3,'cups',false),
    (r15,'Eggs',2,'whole',false),(r15,'Soy Sauce',2,'tbsp',true),
    (r15,'Sesame Oil',1,'tbsp',false),(r15,'Green Onions',3,'whole',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r15,t_asian),(r15,t_seafood);

  -- 16. Turkey Meatballs
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Turkey Meatballs','Light and juicy baked turkey meatballs, perfect over pasta or as an appetiser.',4,20,25,5,
    'Mix ground turkey with breadcrumbs, egg, parmesan, minced garlic, and Italian seasoning. Roll into golf-ball-sized balls. Bake at 200°C for 20–25 mins until golden. Serve with marinara.',
    NOW() - INTERVAL '35 days') RETURNING "Id" INTO r16;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r16,'Ground Turkey',500,'g',false),(r16,'Breadcrumbs',0.5,'cup',false),
    (r16,'Egg',1,'whole',false),(r16,'Parmesan Cheese',0.25,'cup',false),
    (r16,'Garlic',2,'cloves',true),(r16,'Italian Seasoning',1,'tbsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r16,t_italian),(r16,t_protein);

  -- 17. Quinoa Tabbouleh
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Quinoa Tabbouleh','A gluten-free twist on the classic Middle Eastern salad using protein-rich quinoa.',4,20,15,3,
    'Cook quinoa and spread on a tray to cool. Finely chop parsley, tomatoes, and cucumber. Combine with cooled quinoa. Dress with fresh lemon juice, olive oil, and season generously with salt.',
    NOW() - INTERVAL '22 days') RETURNING "Id" INTO r17;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r17,'Quinoa',1,'cup',false),(r17,'Fresh Parsley',2,'cups',false),
    (r17,'Tomatoes',3,'whole',false),(r17,'Cucumber',1,'whole',false),
    (r17,'Lemon Juice',3,'tbsp',true),(r17,'Olive Oil',2,'tbsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r17,t_med),(r17,t_vegan),(r17,t_salad),(r17,t_gf);

  -- 18. Chicken Enchiladas
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Chicken Enchiladas','Shredded chicken wrapped in flour tortillas, smothered in enchilada sauce and melted cheese.',4,20,30,6,
    'Poach and shred chicken. Mix with half the enchilada sauce. Fill tortillas, roll tightly, and place seam-down in a baking dish. Pour remaining sauce and cheese over top. Bake at 180°C for 25 mins. Garnish with cilantro.',
    NOW() - INTERVAL '28 days') RETURNING "Id" INTO r18;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r18,'Chicken Breast',400,'g',false),(r18,'Flour Tortillas',8,'whole',false),
    (r18,'Enchilada Sauce',2,'cups',false),(r18,'Mexican Cheese Blend',2,'cups',false),
    (r18,'Sour Cream',0.5,'cup',false),(r18,'Fresh Cilantro',0.25,'cup',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r18,t_mexican);

  -- 19. Beef Chili
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Beef Chili','A thick, hearty chili packed with ground beef, kidney beans, and bold smoky spices.',6,15,60,8,
    'Brown ground beef with diced onions and bell pepper. Drain excess fat. Add kidney beans, diced tomatoes, chili powder, cumin, and garlic. Simmer uncovered 45–60 mins. Adjust seasoning. Serve with sour cream and shredded cheese.',
    NOW() - INTERVAL '50 days') RETURNING "Id" INTO r19;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r19,'Ground Beef',700,'g',false),(r19,'Kidney Beans',2,'cans',false),
    (r19,'Diced Tomatoes',1,'can',false),(r19,'Chili Powder',2,'tbsp',true),
    (r19,'Onion',1,'whole',false),(r19,'Bell Pepper',1,'whole',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r19,t_comfort),(r19,t_protein);

  -- 20. Lemon Herb Roast Chicken
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Lemon Herb Roast Chicken','A classic Sunday roast with crispy golden skin infused with lemon, garlic, and fresh herbs.',6,15,90,4,
    'Pat chicken dry. Mix softened butter with minced garlic, chopped rosemary, and thyme. Rub under and over skin. Stuff cavity with lemon halves. Roast at 200°C for 90 mins, basting every 30 mins. Rest 15 mins before carving.',
    NOW() - INTERVAL '55 days') RETURNING "Id" INTO r20;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r20,'Whole Chicken',1.8,'kg',false),(r20,'Lemon',2,'whole',false),
    (r20,'Fresh Rosemary',3,'sprigs',false),(r20,'Garlic',4,'cloves',true),
    (r20,'Butter',4,'tbsp',true),(r20,'Fresh Thyme',3,'sprigs',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r20,t_gf),(r20,t_protein);

  -- 21. Mushroom Risotto
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Mushroom Risotto','Creamy restaurant-quality risotto with earthy mixed mushrooms and aged parmesan.',4,10,35,3,
    'Sauté mushrooms until golden, set aside. Toast arborio rice in butter 2 mins. Add white wine and stir until absorbed. Add warm broth one ladle at a time over 20–25 mins, stirring constantly. Fold in mushrooms, a knob of butter, and parmesan.',
    NOW() - INTERVAL '33 days') RETURNING "Id" INTO r21;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r21,'Arborio Rice',1.5,'cups',false),(r21,'Mixed Mushrooms',400,'g',false),
    (r21,'Vegetable Broth',4,'cups',false),(r21,'White Wine',0.5,'cup',false),
    (r21,'Parmesan Cheese',0.75,'cup',false),(r21,'Butter',2,'tbsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r21,t_italian),(r21,t_veg);

  -- 22. Thai Green Curry
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Thai Green Curry','Fragrant coconut milk curry with tofu and fresh vegetables over jasmine rice.',4,15,25,5,
    'Fry green curry paste in oil for 1 min until fragrant. Pour in coconut milk and bring to a gentle simmer. Add tofu, zucchini, and bell peppers. Cook 15 mins. Season with soy sauce and a squeeze of lime. Serve with jasmine rice.',
    NOW() - INTERVAL '14 days') RETURNING "Id" INTO r22;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r22,'Green Curry Paste',3,'tbsp',false),(r22,'Coconut Milk',2,'cans',false),
    (r22,'Firm Tofu',400,'g',false),(r22,'Zucchini',1,'whole',false),
    (r22,'Bell Peppers',2,'whole',false),(r22,'Jasmine Rice',2,'cups',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r22,t_asian),(r22,t_veg);

  -- 23. Black Bean Burgers
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Black Bean Burgers','Hearty, satisfying veggie burgers made with spiced black beans — budget-friendly and delicious.',4,20,15,4,
    'Drain and thoroughly mash black beans. Mix in breadcrumbs, egg, minced garlic, and cumin until combined. Form into 4 patties. Pan-fry in olive oil 4–5 mins each side until crispy. Serve on toasted buns with your favourite toppings.',
    NOW() - INTERVAL '42 days') RETURNING "Id" INTO r23;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r23,'Black Beans',2,'cans',false),(r23,'Breadcrumbs',0.5,'cup',false),
    (r23,'Egg',1,'whole',false),(r23,'Garlic',2,'cloves',true),
    (r23,'Cumin',1,'tsp',true),(r23,'Burger Buns',4,'whole',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r23,t_mexican),(r23,t_veg),(r23,t_budget);

  -- 24. Caprese Salad
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Caprese Salad','Simple Italian salad with ripe tomatoes, fresh mozzarella, and basil drizzled in olive oil.',4,10,0,13,
    'Slice tomatoes and mozzarella into 1cm rounds. Alternate slices on a platter with fresh basil leaves. Drizzle generously with olive oil and balsamic glaze. Season with flaky salt and black pepper.',
    NOW() - INTERVAL '1 days') RETURNING "Id" INTO r24;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r24,'Large Tomatoes',4,'whole',false),(r24,'Fresh Mozzarella',300,'g',false),
    (r24,'Fresh Basil',1,'cup',false),(r24,'Olive Oil',3,'tbsp',true),
    (r24,'Balsamic Glaze',2,'tbsp',false),(r24,'Salt',1,'tsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r24,t_italian),(r24,t_veg),(r24,t_salad),(r24,t_gf),(r24,t_quick);

  -- 25. Slow Cooker Pulled Pork Tacos
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Slow Cooker Pulled Pork Tacos','Fall-apart tender pork cooked low and slow in chipotle and citrus — perfect for taco night.',8,15,480,2,
    'Rub pork shoulder with salt, cumin, and smoked paprika. Place in slow cooker with chipotle peppers, garlic cloves, and orange juice. Cook on LOW 8 hours. Shred with two forks. Serve in warm tortillas topped with coleslaw.',
    NOW() - INTERVAL '70 days') RETURNING "Id" INTO r25;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r25,'Pork Shoulder',1.5,'kg',false),(r25,'Chipotle Peppers in Adobo',2,'whole',false),
    (r25,'Garlic',4,'cloves',true),(r25,'Orange Juice',0.5,'cup',false),
    (r25,'Flour Tortillas',16,'whole',false),(r25,'Coleslaw Mix',2,'cups',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r25,t_mexican),(r25,t_slow);

  -- 26. Egg Fried Rice
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Egg Fried Rice','A quick and satisfying Asian staple that makes excellent use of leftover rice.',2,5,15,18,
    'Heat wok over high heat until smoking. Add oil, then day-old rice. Stir-fry 3 mins. Push rice to the side, crack eggs into the wok and scramble. Combine. Add frozen peas, soy sauce, and sesame oil. Top with green onions.',
    NOW() - INTERVAL '4 days') RETURNING "Id" INTO r26;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r26,'Cooked Rice',2,'cups',false),(r26,'Eggs',3,'whole',false),
    (r26,'Soy Sauce',2,'tbsp',true),(r26,'Sesame Oil',1,'tsp',false),
    (r26,'Green Onions',2,'whole',false),(r26,'Frozen Peas',0.5,'cup',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r26,t_asian),(r26,t_quick),(r26,t_budget);

  -- 27. Pesto Pasta
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Pesto Pasta','Vibrant basil pesto tossed with pasta, sweet cherry tomatoes, and toasted pine nuts.',4,5,15,14,
    'Cook pasta al dente and reserve ½ cup pasta water before draining. Toss hot pasta immediately with pesto, adding pasta water a splash at a time until silky. Stir in halved cherry tomatoes. Top with pine nuts and parmesan.',
    NOW() - INTERVAL '6 days') RETURNING "Id" INTO r27;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r27,'Pasta',400,'g',false),(r27,'Basil Pesto',0.5,'cup',false),
    (r27,'Cherry Tomatoes',1,'cup',false),(r27,'Parmesan Cheese',0.5,'cup',false),
    (r27,'Pine Nuts',0.25,'cup',false),(r27,'Olive Oil',1,'tbsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r27,t_italian),(r27,t_veg),(r27,t_quick);

  -- 28. Lamb Kofta
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Lamb Kofta','Spiced ground lamb skewers grilled until charred, served with warm pita and tzatziki.',4,20,20,3,
    'Mix ground lamb with grated onion, minced garlic, cumin, coriander, salt, and pepper. Knead 2 mins so mixture holds. Shape onto flat metal skewers. Grill on high heat 3–4 mins per side. Serve with warm pita and tzatziki.',
    NOW() - INTERVAL '48 days') RETURNING "Id" INTO r28;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r28,'Ground Lamb',500,'g',false),(r28,'Onion',1,'whole',false),
    (r28,'Cumin',1,'tsp',true),(r28,'Ground Coriander',1,'tsp',true),
    (r28,'Garlic',3,'cloves',true),(r28,'Pita Bread',4,'whole',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r28,t_med);

  -- 29. Smoothie Bowl
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Smoothie Bowl','A thick, vibrant blended bowl topped with granola, fresh fruit, and chia seeds.',1,10,0,9,
    'Blend frozen banana and frozen berries with just enough almond milk to get the blender moving — keep it very thick. Pour into a bowl. Top with granola, sliced fresh fruit, chia seeds, and a drizzle of honey.',
    NOW() - INTERVAL '9 days') RETURNING "Id" INTO r29;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r29,'Frozen Banana',1,'whole',false),(r29,'Frozen Berries',1,'cup',false),
    (r29,'Almond Milk',0.25,'cup',false),(r29,'Granola',0.25,'cup',false),
    (r29,'Chia Seeds',1,'tbsp',false),(r29,'Fresh Fruit',0.5,'cup',false);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r29,t_brekkie),(r29,t_vegan),(r29,t_healthy);

  -- 30. Chocolate Lava Cake
  INSERT INTO "Recipes" ("UserId","Name","Description","ServingSize","PrepTimeMinutes","CookTimeMinutes","CookCount","Instructions","CreatedAt")
  VALUES (3,'Chocolate Lava Cake','Decadent individual chocolate cakes with a warm, gooey molten centre.',4,15,12,1,
    'Melt dark chocolate and butter together. Whisk in sugar until dissolved. Add eggs one at a time, whisking well. Fold in flour. Divide into 4 greased ramekins. Bake at 220°C for exactly 12 mins — edges set but centre jiggles. Rest 1 min, invert onto plates, serve immediately.',
    NOW() - INTERVAL '90 days') RETURNING "Id" INTO r30;
  INSERT INTO "Ingredients" ("RecipeId","Name","Quantity","Unit","IsStaple") VALUES
    (r30,'Dark Chocolate',200,'g',false),(r30,'Butter',100,'g',true),
    (r30,'Eggs',4,'whole',false),(r30,'Sugar',0.5,'cup',true),
    (r30,'All-Purpose Flour',2,'tbsp',true),(r30,'Vanilla Extract',1,'tsp',true);
  INSERT INTO "RecipeTags" ("RecipeId","TagId") VALUES (r30,t_dessert);

  -- ===========================================================================
  -- MEAL PLANS
  -- DayOfWeek: Monday=0, Tuesday=1, Wednesday=2, Thursday=3,
  --            Friday=4, Saturday=5, Sunday=6
  -- MealType: 'Breakfast' | 'Lunch' | 'Dinner'
  -- ===========================================================================

  -- Week 1: 2026-04-27 (last week)
  INSERT INTO "MealPlans" ("UserId","WeekStartDate")
  VALUES (3,'2026-04-27 00:00:00+00') RETURNING "Id" INTO mp1;

  INSERT INTO "MealPlanItems" ("MealPlanId","RecipeId","DayOfWeek","MealType") VALUES
    (mp1,r3, 0,'Breakfast'),  -- Mon  Breakfast : Avocado Toast with Eggs
    (mp1,r1, 0,'Dinner'),     -- Mon  Dinner    : Spaghetti Bolognese
    (mp1,r9, 1,'Lunch'),      -- Tue  Lunch     : Chicken Caesar Salad
    (mp1,r2, 1,'Dinner'),     -- Tue  Dinner    : Chicken Tikka Masala
    (mp1,r14,2,'Breakfast'),  -- Wed  Breakfast : Banana Pancakes
    (mp1,r7, 2,'Dinner'),     -- Wed  Dinner    : Grilled Salmon with Vegetables
    (mp1,r4, 3,'Lunch'),      -- Thu  Lunch     : Greek Salad
    (mp1,r19,3,'Dinner'),     -- Thu  Dinner    : Beef Chili
    (mp1,r5, 4,'Dinner'),     -- Fri  Dinner    : Beef Tacos
    (mp1,r17,5,'Lunch'),      -- Sat  Lunch     : Quinoa Tabbouleh
    (mp1,r8, 5,'Dinner'),     -- Sat  Dinner    : Margherita Pizza
    (mp1,r10,6,'Breakfast'),  -- Sun  Breakfast : Overnight Oats
    (mp1,r6, 6,'Lunch'),      -- Sun  Lunch     : Tomato Basil Soup
    (mp1,r20,6,'Dinner');     -- Sun  Dinner    : Lemon Herb Roast Chicken

  -- Week 2: 2026-05-04 (next week)
  INSERT INTO "MealPlans" ("UserId","WeekStartDate")
  VALUES (3,'2026-05-04 00:00:00+00') RETURNING "Id" INTO mp2;

  INSERT INTO "MealPlanItems" ("MealPlanId","RecipeId","DayOfWeek","MealType") VALUES
    (mp2,r29,0,'Breakfast'),  -- Mon  Breakfast : Smoothie Bowl
    (mp2,r21,0,'Dinner'),     -- Mon  Dinner    : Mushroom Risotto
    (mp2,r24,1,'Lunch'),      -- Tue  Lunch     : Caprese Salad
    (mp2,r11,1,'Dinner'),     -- Tue  Dinner    : Beef Stir Fry
    (mp2,r26,2,'Lunch'),      -- Wed  Lunch     : Egg Fried Rice
    (mp2,r22,2,'Dinner'),     -- Wed  Dinner    : Thai Green Curry
    (mp2,r12,3,'Lunch'),      -- Thu  Lunch     : Veggie Buddha Bowl
    (mp2,r27,3,'Dinner'),     -- Thu  Dinner    : Pesto Pasta
    (mp2,r15,4,'Lunch'),      -- Fri  Lunch     : Shrimp Fried Rice
    (mp2,r18,4,'Dinner'),     -- Fri  Dinner    : Chicken Enchiladas
    (mp2,r23,5,'Lunch'),      -- Sat  Lunch     : Black Bean Burgers
    (mp2,r28,5,'Dinner'),     -- Sat  Dinner    : Lamb Kofta
    (mp2,r16,6,'Lunch'),      -- Sun  Lunch     : Turkey Meatballs
    (mp2,r30,6,'Dinner');     -- Sun  Dinner    : Chocolate Lava Cake

  -- ===========================================================================
  -- PANTRY STAPLES  (for UserId 3)
  -- ===========================================================================
  INSERT INTO "PantryStaples" ("UserId","Name") VALUES
    (3,'Olive Oil'),
    (3,'Salt'),
    (3,'Black Pepper'),
    (3,'Garlic'),
    (3,'Onions'),
    (3,'Soy Sauce'),
    (3,'Chicken Broth'),
    (3,'Canned Tomatoes'),
    (3,'Rice'),
    (3,'Pasta'),
    (3,'Eggs'),
    (3,'Butter'),
    (3,'All-Purpose Flour'),
    (3,'Baking Powder');

END $$;

COMMIT;

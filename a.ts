backend/
├── RecipeVault.sln
│
├── RecipeVault.API/                         # ASP.NET 8 Web API — composition root
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── RecipesController.cs
│   │   ├── MealPlansController.cs
│   │   └── GroceryListController.cs
│   ├── Extensions/
│   │   ├── ServiceCollectionExtensions.cs   # AddApplication(), AddInfrastructure()
│   │   └── MiddlewareExtensions.cs
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── RecipeVault.API.csproj
│
├── RecipeVault.Core/                        # Domain models + interfaces (zero dependencies)
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Recipe.cs
│   │   ├── Ingredient.cs
│   │   ├── Tag.cs
│   │   ├── RecipeTag.cs                     # Many-to-many join entity
│   │   ├── MealPlan.cs
│   │   ├── MealPlanItem.cs
│   │   └── PantryStaple.cs                  # User-defined staples excluded from grocery list
│   ├── Enums/
│   │   ├── MealType.cs                      # Breakfast, Lunch, Dinner
│   │   └── DayOfWeek.cs                     # Monday(0) … Sunday(6)
│   ├── Interfaces/
│   │   ├── IRecipeRepository.cs
│   │   ├── IMealPlanRepository.cs
│   │   └── IUnitOfWork.cs
│   └── RecipeVault.Core.csproj
│
├── RecipeVault.Application/                 # Business logic, DTOs, service interfaces
│   ├── DTOs/
│   │   ├── RecipeDto.cs
│   │   ├── CreateRecipeDto.cs
│   │   ├── MealPlanDto.cs
│   │   ├── GroceryListDto.cs
│   │   ├── GroceryItemDto.cs
│   │   └── AuthDtos.cs
│   ├── Interfaces/
│   │   ├── IRecipeService.cs
│   │   ├── IMealPlanningService.cs
│   │   ├── IGroceryListService.cs
│   │   └── IAuthService.cs
│   ├── Services/
│   │   ├── RecipeService.cs
│   │   ├── MealPlanningService.cs
│   │   ├── GroceryListService.cs
│   │   ├── RecommendationEngine.cs
│   │   ├── UnitConverter.cs
│   │   └── AuthService.cs
│   ├── Mappings/
│   │   └── AutoMapperProfile.cs
│   └── RecipeVault.Application.csproj
│
├── RecipeVault.Infrastructure/              # EF Core + external services
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/                  # EF Fluent API configs
│   │   │   ├── RecipeConfiguration.cs
│   │   │   ├── RecipeTagConfiguration.cs    # Many-to-many mapping
│   │   │   └── MealPlanItemConfiguration.cs
│   │   └── Migrations/
│   ├── Repositories/
│   │   ├── RecipeRepository.cs
│   │   ├── MealPlanRepository.cs
│   │   └── UnitOfWork.cs
│   ├── Services/
│   │   └── BlobStorageService.cs            # Azure Blob for recipe images
│   └── RecipeVault.Infrastructure.csproj
│
└── RecipeVault.Tests/                       # xUnit tests
    ├── Services/
    │   ├── RecipeServiceTests.cs
    │   ├── RecommendationEngineTests.cs
    │   ├── UnitConverterTests.cs
    │   └── GroceryListServiceTests.cs
    ├── Controllers/
    │   └── RecipesControllerTests.cs
    └── RecipeVault.Tests.csproj
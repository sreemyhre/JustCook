# Stage 1: Build Angular
FROM node:22-alpine AS angular-build
WORKDIR /app/frontend
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ ./
RUN npx ng build --configuration production

# Stage 2: Publish .NET API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
WORKDIR /src
COPY backend/ ./
RUN dotnet publish RecipeVault.API/RecipeVault.API.csproj \
    -c Release -o /app/publish --no-self-contained

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=dotnet-build /app/publish ./
COPY --from=angular-build /app/frontend/dist/frontend/browser ./wwwroot/
ENTRYPOINT ["dotnet", "RecipeVault.API.dll"]

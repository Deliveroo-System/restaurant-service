# Use official ASP.NET Core runtime as a base image (for running the app)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY ["RestaurantService.csproj", "."]
RUN dotnet restore "RestaurantService.csproj"

# Copy the entire project and build the release
COPY . .
RUN dotnet build "RestaurantService.csproj" -c Release -o /app/build

# Publish the app (optimized for runtime)
FROM build AS publish
RUN dotnet publish "RestaurantService.csproj" -c Release -o /app/publish

# Final image to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RestaurantService.dll"]

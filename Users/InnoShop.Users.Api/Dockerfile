# Используем официальный .NET 8 SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Копируем решение и проекты Users
COPY InnoShop.sln . 
COPY Users/InnoShop.Users.Api/*.csproj ./Users/InnoShop.Users.Api/
COPY Users/InnoShop.Users.Application/*.csproj ./Users/InnoShop.Users.Application/
COPY Users/InnoShop.Users.Domain/*.csproj ./Users/InnoShop.Users.Domain/
COPY Users/InnoShop.Users.Infrastructure/*.csproj ./Users/InnoShop.Users.Infrastructure/
COPY Users/InnoShop.Users.Tests/*.csproj ./Users/InnoShop.Users.Tests/

# Products
COPY Products/InnoShop.Products.Api/*.csproj ./Products/InnoShop.Products.Api/
COPY Products/InnoShop.Products.Application/*.csproj ./Products/InnoShop.Products.Application/
COPY Products/InnoShop.Products.Domain/*.csproj ./Products/InnoShop.Products.Domain/
COPY Products/InnoShop.Products.Infrastructure/*.csproj ./Products/InnoShop.Products.Infrastructure/

# Gateway
COPY InnoShop.ApiGateway/*.csproj ./InnoShop.ApiGateway/

# Shared
COPY InnoShop.Shared/*.csproj ./InnoShop.Shared/

# Копируем весь исходный код
COPY . .

# Восстанавливаем зависимости решения
RUN dotnet restore InnoShop.sln --no-cache

# Публикуем проект Users.Api
RUN dotnet publish Users/InnoShop.Users.Api/InnoShop.Users.Api.csproj -c Release -o /app/publish --no-restore

# Используем runtime образ для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Копируем опубликованное приложение
COPY --from=build /app/publish .

# Запускаем приложение напрямую
ENTRYPOINT ["dotnet", "InnoShop.Users.Api.dll"]

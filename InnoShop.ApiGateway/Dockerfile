FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Копируем csproj для кэша restore
COPY InnoShop.ApiGateway/*.csproj ./InnoShop.ApiGateway/
COPY InnoShop.Shared/*.csproj ./InnoShop.Shared/
COPY Products/InnoShop.Products.Application/*.csproj ./Products/InnoShop.Products.Application/
COPY Users/InnoShop.Users.Application/*.csproj ./Users/InnoShop.Users.Application/

RUN dotnet restore InnoShop.ApiGateway/InnoShop.ApiGateway.csproj

# Копируем исходники по папкам
COPY InnoShop.ApiGateway ./InnoShop.ApiGateway
COPY InnoShop.Shared ./InnoShop.Shared
COPY Products ./Products
COPY Users ./Users

RUN dotnet publish InnoShop.ApiGateway/InnoShop.ApiGateway.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InnoShop.ApiGateway.dll"]

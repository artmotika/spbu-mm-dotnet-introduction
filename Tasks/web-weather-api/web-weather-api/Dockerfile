#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

ENV LAT=59.93863
ENV LNG=30.31413
ENV StormGlassApiKey=API_KEY_STORM_GLASS
ENV OpenWeatherMapApiKey=API_KEY_OPEN_WEATHER_MAP

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["web-weather-api/web-weather-api.csproj", "web-weather-api/"]
RUN dotnet restore "./web-weather-api/./web-weather-api.csproj"
COPY . .
WORKDIR "/src/web-weather-api"
RUN dotnet build "./web-weather-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./web-weather-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "web-weather-api.dll"]
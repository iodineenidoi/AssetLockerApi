#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AssetLockerApi/AssetLockerApi.csproj", "AssetLockerApi/"]
RUN dotnet restore "AssetLockerApi/AssetLockerApi.csproj"
COPY . .
WORKDIR "/src/AssetLockerApi"
RUN dotnet build "AssetLockerApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AssetLockerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AssetLockerApi.dll"]
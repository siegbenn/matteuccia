#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Matteuccia.Api/Matteuccia.Api.csproj", "Matteuccia.Api/"]
COPY ["Matteuccia.Contracts/Matteuccia.Contracts.csproj", "Matteuccia.Contracts/"]
RUN dotnet restore "Matteuccia.Api/Matteuccia.Api.csproj"
COPY . .
WORKDIR "/src/Matteuccia.Api"
RUN dotnet build "Matteuccia.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Matteuccia.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Matteuccia.Api.dll"]
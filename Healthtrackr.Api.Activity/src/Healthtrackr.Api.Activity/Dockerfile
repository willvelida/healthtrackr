#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Healthtrackr.Api.Activity/Healthtrackr.Api.Activity.csproj", "Healthtrackr.Api.Activity/"]
COPY ["Healthtrackr.Api.Activity.Services/Healthtrackr.Api.Activity.Services.csproj", "Healthtrackr.Api.Activity.Services/"]
COPY ["Healthtrackr.Api.Activity.Repository/Healthtrackr.Api.Activity.Repository.csproj", "Healthtrackr.Api.Activity.Repository/"]
COPY ["Healthtrackr.Api.Activity.Common/Healthtrackr.Api.Activity.Common.csproj", "Healthtrackr.Api.Activity.Common/"]
RUN dotnet restore "Healthtrackr.Api.Activity/Healthtrackr.Api.Activity.csproj"
COPY . .
WORKDIR "/src/Healthtrackr.Api.Activity"
RUN dotnet build "Healthtrackr.Api.Activity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Healthtrackr.Api.Activity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Healthtrackr.Api.Activity.dll"]
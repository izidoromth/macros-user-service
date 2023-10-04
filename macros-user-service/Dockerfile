#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 443
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["macros-user-service/macros-user-service.csproj", "macros-user-service/"]
RUN dotnet restore "macros-user-service/macros-user-service.csproj"
COPY . .
WORKDIR "/src/macros-user-service"
RUN dotnet build "macros-user-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "macros-user-service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "macros-user-service.dll"]
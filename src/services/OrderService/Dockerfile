FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["OrderService.Api.csproj", "OrderService.Api/"]
RUN dotnet restore "OrderService.Api/OrderService.Api.csproj"

COPY ["OrderService.Api/", "OrderService.Api/"]
RUN dotnet build "OrderService.Api/OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS test
WORKDIR /src
COPY ["../OrderService.Tests/OrderService.Tests.csproj", "../OrderService.Tests/"]
RUN dotnet restore "../OrderService.Tests/OrderService.Tests.csproj"
COPY ../OrderService.Tests ../OrderService.Tests
RUN dotnet test --no-build --verbosity normal

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService.Api/OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Api.dll"]

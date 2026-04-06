FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src.csproj", "./"]
RUN dotnet restore

COPY . .
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS test
WORKDIR /src
COPY ["../tests/tests.csproj", "../tests/"]
RUN dotnet restore ../tests/tests.csproj
COPY ../tests ../tests
RUN dotnet test --no-build --verbosity normal

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "src.dll"]

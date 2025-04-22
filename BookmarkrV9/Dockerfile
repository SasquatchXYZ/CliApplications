FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookmarkrV9/BookmarkrV9.csproj", "BookmarkrV9/"]
RUN dotnet restore "BookmarkrV9/BookmarkrV9.csproj"
COPY . .
WORKDIR "/src/BookmarkrV9"
RUN dotnet build "./BookmarkrV9.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BookmarkrV9.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookmarkrV9.dll"]

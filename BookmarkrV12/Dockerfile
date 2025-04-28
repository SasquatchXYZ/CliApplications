FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookmarkrV12/BookmarkrV12.csproj", "BookmarkrV12/"]
RUN dotnet restore "BookmarkrV12/BookmarkrV12.csproj"
COPY . .
WORKDIR "/src/BookmarkrV12"
RUN dotnet build "./BookmarkrV12.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BookmarkrV12.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookmarkrV12.dll"]

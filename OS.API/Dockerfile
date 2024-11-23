FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Temporarily switch to root user to install FFmpeg
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install FFmpeg
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*

# Switch back to non-root user
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OS.API/OS.API.csproj", "OS.API/"]
RUN dotnet restore "OS.API/OS.API.csproj"
COPY . . 
WORKDIR "/src/OS.API"
RUN dotnet build "OS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OS.API.dll"]

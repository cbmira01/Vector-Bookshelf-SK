# Base runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Build and restore dependencies
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/GutenbergGraphBuilder/GutenbergGraphBuilder.csproj", "Src/GutenbergGraphBuilder/"]
RUN dotnet restore "./Src/GutenbergGraphBuilder/GutenbergGraphBuilder.csproj"
COPY . .
WORKDIR "/src/Src/GutenbergGraphBuilder"
RUN dotnet build "./GutenbergGraphBuilder.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GutenbergGraphBuilder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Src/GutenbergGraphBuilder/Resources /app/Resources 

# Install .NET Debugging tools
RUN apt-get update \
    && apt-get install -y unzip curl \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

# Expose debugging port
EXPOSE 4025

ENTRYPOINT ["dotnet", "GutenbergGraphBuilder.dll"]

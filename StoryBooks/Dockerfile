# ---------- STAGE 1: Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY *.sln .
COPY StoryBooks.API/*.csproj StoryBooks.API/
COPY StoryBooks.Application/*.csproj StoryBooks.Application/
COPY StoryBooks.Data/*.csproj StoryBooks.Data/
RUN dotnet restore StoryBooks.API/StoryBooks.API.csproj

# Copy everything and build the app
COPY . .
RUN dotnet publish StoryBooks.API/StoryBooks.API.csproj -c Release -o /app/publish /p:UseAppHost=false


# ---------- STAGE 2: Run ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy only the published output from build stage
COPY --from=build /app/publish .

# Configure environment and port
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Start the application
ENTRYPOINT ["dotnet", "StoryBooks.API.dll"]

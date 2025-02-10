# Use the official .NET SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY QuizGameServer/*.csproj ./QuizGameServer/
RUN dotnet restore

# Copy everything else and build
COPY QuizGameServer/. ./QuizGameServer/
WORKDIR /app/QuizGameServer
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/QuizGameServer/out ./
ENTRYPOINT ["dotnet", "QuizGameServer.dll"]
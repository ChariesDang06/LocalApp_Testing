# Use the .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore 

# Copy everything else and build
COPY . ./
RUN dotnet publish  -c Release -o out 

# Use the .NET runtime for running the application
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port the app runs on
#EXPOSE 5000

# Set environment variables
#ENV ASPNETCORE_URLS=http://+:80

# Define the entry point for the application
ENTRYPOINT ["dotnet", "LocalEdu_App.dll"]

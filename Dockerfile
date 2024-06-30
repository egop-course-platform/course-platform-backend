# Use the SDK image to build and publish the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Cp.Presentation/Cp.Presentation.csproj", "Cp.Presentation/"]
RUN dotnet restore "Cp.Presentation/Cp.Presentation.csproj"

# Copy the rest of the application and build it
COPY ./Cp.Presentation ./Cp.Presentation
WORKDIR "/src/Cp.Presentation"
RUN dotnet publish "Cp.Presentation.csproj" --no-restore -c Release -o /app/publish /p:UseAppHost=false

# Use the runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cp.Presentation.dll"]

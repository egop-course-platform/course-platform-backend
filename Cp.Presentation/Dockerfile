FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Cp.Presentation/Cp.Presentation.csproj", "Cp.Presentation/"]
RUN dotnet restore "Cp.Presentation/Cp.Presentation.csproj"
COPY . .
WORKDIR "/src/Cp.Presentation"
RUN dotnet build "Cp.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cp.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cp.Presentation.dll"]

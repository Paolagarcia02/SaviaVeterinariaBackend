# Etapa 1: Compilación (SDK 8.0)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackEnd.csproj", "."]
RUN dotnet restore "./BackEnd.csproj"
COPY . .
RUN dotnet build "BackEnd.csproj" -c Release -o /app/build

# Etapa 2: Publicación
FROM build AS publish
RUN dotnet publish "BackEnd.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Final (¡ESTA ES LA QUE TIENE QUE SER 8.0 TAMBIÉN!)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackEnd.dll"]
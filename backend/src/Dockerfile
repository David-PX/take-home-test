# ===== Build =====
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiamos la solución y csproj para restaurar dependencias
COPY src.sln ./
COPY Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj Fundo.Applications.WebApi/
COPY Fundo.Services.Tests/Fundo.Services.Tests.csproj Fundo.Services.Tests/

# Restore
RUN dotnet restore src.sln

# Copiamos todo el código
COPY . .

# Publish solo el WebApi
RUN dotnet publish Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj -c Release -o /app/publish

# ===== Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Fundo.Applications.WebApi.dll"]

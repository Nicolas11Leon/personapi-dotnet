# syntax=docker/dockerfile:1

# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restaurar primero (capa cacheable mientras no cambie el csproj)
COPY personapi-dotnet.csproj ./
RUN dotnet restore personapi-dotnet.csproj

COPY . .
RUN dotnet publish personapi-dotnet.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

# Usuario no-root incluido en las imágenes de .NET 8
USER $APP_UID

ENTRYPOINT ["dotnet", "personapi-dotnet.dll"]

# Étape 1 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copier les fichiers projet et restaurer les dépendances
COPY *.csproj ./
RUN dotnet restore

# Copier le reste du code source et compiler
COPY . .
RUN dotnet publish -c Release -o out

# Étape 2 : Exécution dans une image légère
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose le port utilisé par ton API
EXPOSE 8888

# Commande d’entrée
ENTRYPOINT ["dotnet", "powerplant-coding-challenge.dll"]

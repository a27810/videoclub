# 1) Build stage: compila y publica la aplicación
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# Copia el csproj y restaura dependencias
COPY Videoclub.csproj ./
RUN dotnet restore

# Copia el resto del código y publica en Release
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# 2) Runtime stage: la imagen final más ligera
FROM mcr.microsoft.com/dotnet/runtime:5.0 AS runtime
WORKDIR /app

# Copia sólo los artefactos publicados
COPY --from=build /app/publish ./

# Volumen para datos persistentes (JSON, logs…)
VOLUME /app/data

# Puerto de la aplicación
EXPOSE 27810

# Variable de entorno de ejemplo (ajusta a tu DB real)
ENV ConnectionString="Server=db;Database=videoclub;Uid=miusuario;Pwd=micontraseña;"

# Arranca la aplicación
ENTRYPOINT ["dotnet", "Videoclub.dll"]

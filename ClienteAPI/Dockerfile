# Usa la imagen de SDK de .NET para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia los archivos de proyecto y restaura dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia el resto de la aplicación y compila
COPY . ./
RUN dotnet publish -c Release -o out

# Usa la imagen de runtime de .NET para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ClienteAPI.dll"]

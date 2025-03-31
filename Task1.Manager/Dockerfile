# Используем официальный образ .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем проект и восстанавливаем зависимости
COPY *.csproj ./ 
RUN dotnet restore

# Копируем остальные файлы и собираем приложение
COPY . ./
RUN dotnet publish -c Release -o out

# Финальный образ, в котором запускаем приложение
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Устанавливаем curl в финальном контейнере
RUN apt-get update && apt-get install -y curl

# Копируем собранное приложение из build-слоя
COPY --from=build /app/out .

# Запуск API при старте контейнера
CMD ["dotnet", "Task1.Manager.dll"]

# Базовый образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем проект и устанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем все файлы и собираем проект
COPY . ./
RUN dotnet publish -c Release -o out

# Финальный образ (используется в продакшене)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Устанавливаем curl В ФИНАЛЬНОМ ОБРАЗЕ
RUN apt-get update && apt-get install -y curl

# Копируем собранный проект из `build`
COPY --from=build /app/out .

CMD ["dotnet", "Task1.Worker.dll"]

# CrackHash Distributed System

## Описание
Распределённая система для взлома хэшей методом перебора

## Компоненты
- Менеджер (Manager): принимает задачи, распределяет подзадачи
- Воркеры (Workers): выполняют вычисления

## Быстрый старт

### 1. Установка зависимостей
```bash
# Для .NET (Ubuntu)
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-6.0
'''
# Для Docker
sudo apt-get install docker.io docker-compose

### 2. Запуск системы
# Вариант 1 - Локальный запуск
(cd Task1.Manager && dotnet run) & (cd Task1.Worker && dotnet run)

# Вариант 2 - Через Docker
docker-compose up --build

### 3. Примеры запросов
# Создание задачи
curl -X POST http://localhost:8080/api/hash/crack \
  -H "Content-Type: application/json" \
  -d '{"hash":"5d41402abc4b2a76b9719d911017c592","maxLength":4}'

# Проверка статуса
curl "http://localhost:8080/api/hash/status?requestId=123e4567-e89b-12d3-a456-426614174000"

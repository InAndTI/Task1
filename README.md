CrackHash Distributed System
====================================

CrackHash — распределенная система для взлома хэшей методом перебора. Состоит из:
- Менеджер: Принимает задачи, разделяет на части, управляет воркерами
- Воркеры: Выполняют вычислительные задачи

Архитектура:
1. Клиент → POST /api/hash/crack → Менеджер
2. Менеджер разбивает задачу на 33 части → рассылает воркерам
3. Воркеры → PATCH /api/hash/update-status → Менеджер
4. Клиент проверяет статус → GET /api/hash/status

Требования:
- .NET 6+
- Docker (опционально)

Структура:
- Менеджер: Task1.Manager/Program.cs
  API:
  - POST /api/hash/crack – создать задачу
  - GET /api/hash/status – проверить статус
  - PATCH /api/hash/update-status – обновить прогресс

- Воркеры: Task1.Worker/Program.cs

Конфигурация (appsettings.json):
{
  "AppSettings": {
    "WorkerUrls": ["http://worker1:8080/api/hash/task", ...],
    "WorkerUrl": "http://worker:8080/api/hash/task",
    "ManagerUrl": "http://manager:8080/api/hash/update-status"
  }
}

Запуск:
Локально:
1. Менеджер:
   cd Task1.Manager
   dotnet run

2. Воркеры:
   cd ../Task1.Worker
   dotnet run

Через Docker:
docker-compose up --build

API Примеры:
1. Создать задачу:
POST /api/hash/crack
{
  "hash": "e2fc714c4727ee9395f324cd2e7f331f",
  "maxLength": 4
}
→ { "requestId": "123e4567..." }

2. Проверить статус:
GET /api/hash/status?requestId=123...
→ {
  "status": "PARTIAL_READY",
  "progress": 45.45,
  "data": ["result1", ...]
}

3. Обновить прогресс (для воркеров):
PATCH /api/hash/update-status
{
  "requestId": "123...",
  "results": ["31", "32"],
  "partCount": 33
}

Логи:
Отправка части 0 на http://worker1:8080/api/hash/task
Processed part 31, Total completed: 1

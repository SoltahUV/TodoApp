# TodoApp

Учебный REST API для заметок и задач. ASP.NET Core Web API (.NET 10), EF Core, PostgreSQL, JWT-аутентификация.

## Структура

- `backend/WebAPI` — контроллеры, настройка приложения
- `backend/BusinessLogic` — сервисы и DTO
- `backend/DataAccess` — EF Core, модели, репозитории, миграции
- `backend/Auth` — регистрация, вход, выдача JWT
- `bruno/` — коллекция запросов для Bruno

## Запуск

Нужен Docker.

```
cp .env.example .env
# впиши свои значения DB_PASSWORD и JWT_SECRET (секрет — минимум 32 символа)
docker compose up --build
```

API доступен на `http://localhost:5000`. Документация Scalar: `http://localhost:5000/scalar`.

Таблицы в базе создаются автоматически при старте (EF Core миграции).

## Запуск без Docker

Нужны .NET 10 SDK и PostgreSQL. Заполни секции `DatabaseConfiguration` и `JwtConfiguration` через user-secrets или переменные окружения, затем:

```
dotnet run --project backend/WebAPI
```

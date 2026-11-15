# TodoApp

Учебный REST API для заметок и задач. ASP.NET Core Web API (.NET 10), EF Core, PostgreSQL, JWT.

## Что внутри

- регистрация и вход, токен JWT (HS256), пароли хешируются BCrypt
- CRUD для заметок и задач, пагинация через `skip` и `take`
- записи видны только владельцу
- лимит 5 запросов в минуту на `/auth/*`
- API подключается к БД ролью без прав DDL, миграции идут отдельным сервисом
- юнит- и интеграционные тесты, запускаются без БД и Docker

## Структура

- `backend/WebAPI`: контроллеры, middleware
- `backend/BusinessLogic`: сервисы и DTO
- `backend/DataAccess`: EF Core, репозитории, миграции
- `backend/Auth`: регистрация, вход, выдача JWT
- `backend/Shared`: лимиты полей, исключения, валидация
- `tests`: xUnit и интеграционные тесты
- `bruno`: коллекция запросов
- `docs/security-audit.md`: аудит безопасности

## Запуск

Нужен Docker.

```
cp .env.example .env
# вписать DB_PASSWORD, DB_APP_PASSWORD и JWT_SECRET
# пароли: openssl rand -hex 24
# JWT_SECRET: openssl rand -base64 48
docker compose up --build
```

API: `http://localhost:5000`. Документация Scalar: `http://localhost:5000/scalar`. Она работает только в среде Development, для Docker нужен локальный `docker-compose.override.yml` (в `.gitignore`):

```yaml
services:
  api:
    environment:
      ASPNETCORE_ENVIRONMENT: Development
```

## API

| Метод | Путь | Описание |
|---|---|---|
| POST | `/auth/register` | регистрация |
| POST | `/auth/login` | вход, ответ `{ "token": "..." }` |
| GET, POST | `/api/Notes`, `/api/Tasks` | список, создание |
| GET, PUT, PATCH, DELETE | `/api/Notes/{id}`, `/api/Tasks/{id}` | одна запись |

Все пути `/api/*` требуют заголовок `Authorization: Bearer <token>`. Полная схема лежит в Scalar и в `/openapi/v1.json`.

## Тесты

```
dotnet test
```

## Ограничения

- только HTTP, для публичного доступа нужен reverse proxy с TLS
- нет refresh-токенов и отзыва токена
- rate limit хранится в памяти процесса
- список не возвращает общее число записей
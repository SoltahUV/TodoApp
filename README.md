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
# впиши свои значения DB_PASSWORD и JWT_SECRET
# JWT_SECRET сгенерируй: openssl rand -base64 48
docker compose up --build
```

API доступен на `http://localhost:5000` (порт опубликован только на `127.0.0.1`). Документация Scalar: `http://localhost:5000/scalar`.

Таблицы в базе создаются автоматически при старте (EF Core миграции).

## Конфигурация

Рабочие секреты не хранятся в репозитории и не имеют значений по умолчанию. При старте API проверяет конфигурацию и завершается с ошибкой, если она отсутствует, содержит placeholder или слишком слабый секрет.

### Docker Compose

| Переменная | Обязательна | По умолчанию | Назначение |
|---|---|---|---|
| `DB_PASSWORD` | да | — | пароль PostgreSQL |
| `JWT_SECRET` | да | — | ключ подписи JWT, минимум 32 символа и не меньше 10 различных символов |
| `JWT_ISSUER` | нет | `TodoApi` | значение `iss` в токене |
| `JWT_AUDIENCE` | нет | `TodoApiClients` | значение `aud` в токене |
| `JWT_EXPIRES_IN_MINUTES` | нет | `60` | срок жизни access-токена, от 5 до 1440 минут |

### Запуск без Docker

Нужны .NET 10 SDK и PostgreSQL. Задай `DatabaseConfiguration` и `JwtConfiguration` через user-secrets или переменные окружения:

```
dotnet user-secrets init --project backend/WebAPI
dotnet user-secrets set "DatabaseConfiguration:Host" "localhost" --project backend/WebAPI
dotnet user-secrets set "DatabaseConfiguration:Port" "5433" --project backend/WebAPI
dotnet user-secrets set "DatabaseConfiguration:Database" "tododb" --project backend/WebAPI
dotnet user-secrets set "DatabaseConfiguration:Username" "todo" --project backend/WebAPI
dotnet user-secrets set "DatabaseConfiguration:Password" "<пароль>" --project backend/WebAPI
dotnet user-secrets set "JwtConfiguration:SecretKey" "$(openssl rand -base64 48)" --project backend/WebAPI
```

Затем:

```
dotnet run --project backend/WebAPI
```

Переменные окружения имеют приоритет над `appsettings.json`; для вложенных ключей используется `__`, например `JwtConfiguration__SecretKey`.

### JWT

- подпись и проверка только `HS256`;
- проверяются `iss`, `aud`, срок действия и подпись, допустимое расхождение часов — 30 секунд;
- каждый токен получает уникальный `jti`;
- после смены `JWT_SECRET` все ранее выданные токены становятся недействительными;
- refresh-токены и механизм отзыва пока не реализованы.

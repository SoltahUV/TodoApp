# TodoApp

Учебный REST API для заметок и задач. ASP.NET Core Web API (.NET 10), EF Core, PostgreSQL, JWT-аутентификация.

## Структура

- `backend/WebAPI` — контроллеры, настройка приложения
- `backend/BusinessLogic` — сервисы и DTO
- `backend/DataAccess` — EF Core, модели, репозитории, миграции
- `backend/Auth` — регистрация, вход, выдача JWT
- `db/init` — SQL-инициализация Postgres (роль приложения)
- `tests/xUnit-Tests` — xUnit-тесты
- `bruno/` — коллекция запросов для Bruno

## Запуск

Нужен Docker.

```
cp .env.example .env
# впиши свои значения DB_PASSWORD, DB_APP_PASSWORD и JWT_SECRET
# пароли сгенерируй: openssl rand -hex 24
# JWT_SECRET: openssl rand -base64 48
docker compose up --build
```

API доступен на `http://localhost:5000` (порт опубликован только на `127.0.0.1`). Документация Scalar: `http://localhost:5000/scalar`.

Миграции EF Core применяются автоматически: при `docker compose up --build` сервис `migrator` прогоняет их до старта `api`. При локальном запуске (`dotnet run` в Development) миграции применяются при старте API.

### Права в PostgreSQL

Владелец БД `todo` (суперпользователь) используется только сервисом `migrator`. API подключается ролью `todo_app` с правами `SELECT/INSERT/UPDATE/DELETE` на `public`: без DDL, создания ролей и доступа к другим БД. Роль создаётся скриптом `db/init/01-app-role.sql` при первой инициализации пустого кластера; `ALTER DEFAULT PRIVILEGES` покрывает таблицы, которые позже создадут миграции.

Если volume `pgdata` уже существовал до этого изменения, скрипт не выполнится — удали volume (`docker compose down -v`) либо создай роль вручную.

## Тесты

```bash
dotnet test
```

93 теста, БД и Docker не нужны: валидация DTO (как её видит MVC), контракт атрибутов на record-параметрах, сервисы на фейковых репозиториях, выпуск и проверка JWT, соответствие модели EF Core лимитам из `FieldLimits`.

## Конфигурация

Рабочие секреты не хранятся в репозитории и не имеют значений по умолчанию. При старте API проверяет конфигурацию и завершается с ошибкой, если она отсутствует, содержит placeholder или слишком слабый секрет.

### Docker Compose

| Переменная | Обязательна | По умолчанию | Назначение |
|---|---|---|---|
| `DB_PASSWORD` | да | — | пароль владельца БД `todo` (используется только миграциями) |
| `DB_APP_PASSWORD` | да | — | пароль роли `todo_app`, под которой работает API |
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

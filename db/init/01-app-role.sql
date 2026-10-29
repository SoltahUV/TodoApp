-- Роль для рантайма API: только DML в public, без DDL и админских прав.
-- Выполняется docker-entrypoint при первой инициализации пустого кластера.
\getenv app_password DB_APP_PASSWORD

CREATE ROLE todo_app LOGIN PASSWORD :'app_password' NOSUPERUSER NOCREATEDB NOCREATEROLE;

GRANT CONNECT ON DATABASE tododb TO todo_app;
GRANT USAGE ON SCHEMA public TO todo_app;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO todo_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO todo_app;

-- Будущие объекты, которые создадут миграции от имени владельца (todo)
ALTER DEFAULT PRIVILEGES IN SCHEMA public
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO todo_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public
    GRANT USAGE, SELECT ON SEQUENCES TO todo_app;

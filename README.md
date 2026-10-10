# OLX Listing Parser

Консольний парсер оголошень OLX на .NET 10. Проєкт відкриває сторінки категорії через Selenium, збирає посилання на оголошення, переходить на сторінки оголошень і зберігає дані в SQLite через Entity Framework Core.

## Що збирає парсер

Для кожного оголошення зберігаються:

- `Id` — ідентифікатор оголошення OLX;
- `Title` — назва оголошення;
- `Description` — опис оголошення;
- `Url` — посилання на оголошення;
- `AuthorName` — ім'я продавця;
- `Phone` — номер телефону, якщо він доступний на сторінці.

Парсер не створює дублікати: перед збереженням перевіряються `Id` і `Url`. При повторному запуску вже збережені оголошення оновлюються.

## Технології

- .NET 10;
- Selenium WebDriver;
- Selenium Standalone Chromium;
- Entity Framework Core;
- SQLite;
- Docker Compose.

## Локальний запуск

Потрібні встановлені .NET SDK 10 та Chrome/Chromium.

```bash
dotnet restore
dotnet build
dotnet run --project OlxParser.Console
```

SQLite-база створюється за шляхом:

```text
data/olx_ads.sqlite3
```

EF Core автоматично застосовує міграції під час запуску програми.

## Запуск через Docker Compose

Запустити парсер разом із Selenium:

```bash
docker compose up --build
```

Або запустити тільки одноразовий запуск парсера:

```bash
docker compose run --rm olx-parser
```

Подивитися логи:

```bash
docker compose logs -f olx-parser
```

Зупинити контейнери:

```bash
docker compose down
```

База зберігається на хості в `data/olx_ads.sqlite3`, тому дані не втрачаються після видалення контейнера.

## Перевірка бази

Кількість оголошень:

```bash
sqlite3 data/olx_ads.sqlite3 \
  "SELECT COUNT(*) FROM advertisements;"
```

Перегляд останніх записів:

```bash
sqlite3 -header -column data/olx_ads.sqlite3 \
  "SELECT Id, Title, AuthorName, length(Description) AS DescriptionLength
   FROM advertisements
   ORDER BY rowid DESC
   LIMIT 10;"
```

## Конфігурація

Основні параметри знаходяться у `OlxParser.Console/Configuration/ParserOptions.cs`:

- URL категорії;
- шлях до SQLite-бази;
- кількість сторінок для обробки.

## Чому `Phone` може бути порожнім

OLX не завжди показує номер телефону в HTML сторінки. Він може бути прихований, доступний лише після натискання кнопки або недоступний для конкретного оголошення. У такому випадку парсер зберігає `Phone = null`, а інші доступні поля оголошення все одно зберігаються.

## Обмеження

Структура HTML OLX може змінюватися. Якщо сайт змінить `data-testid` або інші селектори, відповідні селектори в Selenium-сторінках потрібно буде оновити.

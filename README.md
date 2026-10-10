# OLX Listing Parser

A .NET 10 console application that parses OLX listings using Selenium, collects listing details, and stores them in SQLite through Entity Framework Core.

## What the parser collects

For each listing, the parser stores:

- `Id` — the OLX listing identifier;
- `Title` — the listing title;
- `Description` — the listing description;
- `Url` — the listing URL;
- `AuthorName` — the seller name;
- `Phone` — the phone number when it is available on the page.

The parser prevents duplicates by checking both `Id` and `Url`. On subsequent runs, existing listings are updated instead of being inserted again.

## Technologies

- .NET 10;
- Selenium WebDriver;
- Selenium Standalone Chromium;
- Entity Framework Core;
- SQLite;
- Docker Compose.

## Local run

The local run requires .NET SDK 10 and Chrome or Chromium.

```bash
dotnet restore
dotnet build
dotnet run --project OlxParser.Console
```

The SQLite database is created at:

```text
data/olx_ads.sqlite3
```

Entity Framework Core migrations are applied automatically when the application starts.

## Docker Compose

Start the parser and Selenium service:

```bash
docker compose up --build
```

Run the parser as a one-time job:

```bash
docker compose run --rm olx-parser
```

View parser logs:

```bash
docker compose logs -f olx-parser
```

Stop the services:

```bash
docker compose down
```

The database is mounted from the host at `data/olx_ads.sqlite3`, so it remains available after the containers are removed.

## Inspecting the database

Count stored listings:

```bash
sqlite3 data/olx_ads.sqlite3 \
  "SELECT COUNT(*) FROM advertisements;"
```

View the latest records:

```bash
sqlite3 -header -column data/olx_ads.sqlite3 \
  "SELECT Id, Title, AuthorName, length(Description) AS DescriptionLength
   FROM advertisements
   ORDER BY rowid DESC
   LIMIT 10;"
```

## Configuration

The main parser settings are located in `OlxParser.Console/Configuration/ParserOptions.cs`:

- category URL;
- SQLite database path;
- number of pages to parse.

## Why `Phone` can be empty

OLX does not always expose a phone number in the page HTML. The number may be hidden, available only after an interaction, or unavailable for a particular listing. In that case, the parser stores `Phone` as `null` while preserving the other available listing fields.

## Limitations

The OLX HTML structure may change over time. If OLX changes its `data-testid` attributes or other selectors, the corresponding Selenium page objects will need to be updated.

# Food Intake

Desktop application for importing, classifying and reporting on food intake data.

## Prerequisites

- .NET 10 SDK

## Run

From the repo root

```sh
dotnet run --project src/FoodIntake.app
```

The database lives at `<LocalApplicationData>/FoodIntake/food.db`
(`~/.local/share/FoodIntake/food.db` on macOS/Linux, `%LOCALAPPDATA%\FoodIntake\food.db` on Windows).
Delete that file to start from a clean slate.

## Tests

```sh
dotnet test
```

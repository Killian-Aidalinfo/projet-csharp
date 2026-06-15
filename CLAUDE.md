# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Clean Code Rules (MANDATORY)

These rules apply to ALL files in the project and MUST be followed at all times:

- **Max 400 lines per file** — if a file exceeds 400 lines, split it into smaller modules
- **Small, reusable files** — each file should have a single, clear responsibility
- **Favor helpers and utilities** — extract repeated logic into dedicated helper files (e.g. `utils/`, `helpers/`, `composables/`)
- **No duplication** — before writing logic, check if a helper already exists
- **Composables for shared logic** — Vue/Nuxt logic shared across components must live in a composable
- **Server utils for API logic** — shared server-side logic belongs in `server/utils/`
- **Prefer small functions** — functions should do one thing; split large functions into smaller named helpers

## What this is

ASP.NET Core 8 Minimal API ("Tasks" demo). The entire app is a single `Program.cs`
using top-level statements — no controllers, no separate service/DI layers. Data is
held in an in-memory `ConcurrentDictionary`, so state resets on every restart.

`Program.cs` only wires DI and routes — the logic lives in a thin service layer:
- `Models/` — entities (`TodoTask`, `Project`, `Category`) and the `Create*Request` DTOs.
- `Services/` — `TaskService`, `ProjectService`, `CategoryService`, each owning an
  in-memory store and validation. `Result<T>` is the success/error return type
  (no exceptions for control flow), which keeps services unit-testable in isolation.

Routes: `GET /`, and `GET`/`GET {id:int}`/`POST` for each of `/tasks`, `/projects`,
`/categories`. A `TodoTask` carries optional `ProjectId`/`CategoryId` links.

`.NET is not installed on the host` — all build/run/test happens inside Docker.

## Commands

```bash
# Production image (multi-stage build, runs the optimized aspnet:8.0 runtime)
docker compose up -d --build        # http://localhost:8080
docker compose logs -f
docker compose down

# Dev with hot reload (dotnet watch, source mounted as a volume)
docker compose -f compose.dev.yaml up --build
docker compose -f compose.dev.yaml down
```

In dev mode, editing any `.cs` file triggers `dotnet watch` hot reload (~1s, no rebuild).

```bash
# Unit tests (xUnit) — the test project lives in tests/ and references the web project
docker run --rm -v "$(pwd)":/src -w /src mcr.microsoft.com/dotnet/sdk:8.0 \
  dotnet test tests/ProjetCsharp.Tests.csproj
```

Unit tests in `tests/` (xUnit) cover the service layer. `tests/` is excluded from the
web project's compilation (`<Compile Remove="tests/**" />` in the `.csproj`) and the
prod `Dockerfile` only copies/publishes the root `*.csproj`, so tests never reach the
runtime image. You can also verify behavior by hitting the routes with `curl` against
`http://localhost:8080` (e.g. `curl -X POST .../tasks -H 'Content-Type:
application/json' -d '{"title":"..."}'`).

## Two-Dockerfile setup

The split is intentional, don't merge them:
- `Dockerfile` + `compose.yaml` — prod. Multi-stage `sdk:8.0` → `aspnet:8.0` (small runtime image).
- `Dockerfile.dev` + `compose.dev.yaml` — dev. Stays on the full `sdk:8.0`, mounts
  `.:/src`, and uses anonymous volumes for `/src/bin` and `/src/obj` so container
  build artifacts never collide with the host. `DOTNET_USE_POLLING_FILE_WATCHER=true`
  is required for the file watcher to see changes through the Docker bind mount on Linux.

The assembly is named `ProjetCsharp` (`AssemblyName` in the `.csproj`); the prod
`Dockerfile` entrypoint runs `ProjetCsharp.dll`, so keep these in sync if renamed.

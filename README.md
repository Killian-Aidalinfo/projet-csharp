# projet-csharp

API minimale ASP.NET Core (.NET 8) lançable avec Docker Compose sur Ubuntu.
Aucune installation de .NET n'est nécessaire sur la machine hôte : tout est compilé
dans le conteneur (build multi-stage).

## Lancer

```bash
docker compose up -d --build
```

L'API écoute sur http://localhost:8080

## Arrêter

```bash
docker compose down
```

## Logs

```bash
docker compose logs -f
```

## Les 4 routes

| Méthode | Chemin         | Description                  |
|---------|----------------|------------------------------|
| GET     | `/`            | Health check                 |
| GET     | `/tasks`       | Liste toutes les tâches      |
| GET     | `/tasks/{id}`  | Récupère une tâche par id    |
| POST    | `/tasks`       | Crée une tâche               |

### Exemples

```bash
curl http://localhost:8080/
curl http://localhost:8080/tasks
curl http://localhost:8080/tasks/1
curl -X POST http://localhost:8080/tasks \
  -H "Content-Type: application/json" \
  -d '{"title":"Ma nouvelle tâche"}'
```

> Les données sont stockées en mémoire : elles sont réinitialisées à chaque
> redémarrage du conteneur.

## Structure

```
projet-csharp/
├── Program.cs            # API minimale + les 4 routes
├── projet-csharp.csproj  # Projet .NET 8
├── appsettings.json
├── Dockerfile            # Build multi-stage (sdk -> aspnet)
├── compose.yaml          # Service Docker Compose
└── .dockerignore
```

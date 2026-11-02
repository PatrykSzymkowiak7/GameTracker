# GameTracker

A RESTful Web API for managing personal video game collections. Built with **.NET 8, ASP.NET Core, Entity Framework Core and SQLite**, following Clean Architecture principles.

GameTracker allows users to manage their own game libraries, track progress, filter and sort games, and securely access their data using JWT authentication.

## Features

- **Game management:** Create, read, update and delete games.
- **User authentication:** User registration and login using ASP.NET Core Identity.
- **JWT authentication:** Secure API endpoints using JSON Web Tokens.
- **User data isolation:** Each user can access and manage only their own games.
- **Game relationships:** Games can be associated with developers, genres and platforms.
- **Filtering and sorting:** Search games using available query parameters.
- **Pagination:** Retrieve large collections in smaller pages.
- **Global error handling:** Consistent HTTP error responses using ProblemDetails.
- **Database migrations:** Database schema is managed using Entity Framework Core migrations.
- **Health checks:** Separate liveness and readiness endpoints.
- **Automated tests:** Unit tests and integration tests.
- **Docker support:** Run the application using Docker Compose.
- **CI pipeline:** GitHub Actions automatically builds and tests the solution.

## Tech Stack

| Technology | Purpose |
|---|---|
| C# / .NET 8 | Application development |
| ASP.NET Core Web API | HTTP API |
| Entity Framework Core | ORM and database access |
| SQLite | Database |
| ASP.NET Core Identity | User management and password hashing |
| JWT Bearer | Authentication and authorization |
| AutoMapper | Object mapping |
| xUnit | Automated testing |
| Moq | Mocking dependencies |
| WebApplicationFactory | Integration testing |
| Docker / Docker Compose | Containerization |
| GitHub Actions | Continuous Integration |

## Architecture

The solution follows Clean Architecture principles and is divided into four main projects.

```text
GameTracker.sln
│
├── GameTracker
│   └── GameTracker.Api
│
├── Application
│   └── GameTracker.Application
│
├── Domain
│   └── GameTracker.Domain
│
└── Infrastructure
    └── GameTracker.Infrastructure
```

Additional test projects:

```text
GameTracker.Api.Tests
Tests/
└── GameTracker.Application.Tests
```

### Layer responsibilities

- **API:** Controllers, HTTP endpoints, authentication configuration, middleware and dependency injection.
- **Application:** Business logic, services, DTOs and repository interfaces.
- **Domain:** Core entities, enums and domain models.
- **Infrastructure:** Entity Framework Core, SQLite, repositories, Identity and database migrations.

The Domain layer does not depend on infrastructure concerns. The Application layer defines abstractions, while Infrastructure provides their implementations.

## Database Model

The application uses Entity Framework Core with SQLite.

Main relationships:

- One developer can be associated with many games.
- A game can have multiple genres.
- A game can be available on multiple platforms.
- Each game belongs to one user.
- Users have independent game collections.

The many-to-many relationships are implemented using join tables.

Database migrations are applied automatically when the application starts. Reference data such as genres, platforms and developers is seeded automatically.

The SQLite database itself is not stored in Git. The schema is recreated from EF Core migrations.

## Authentication and Authorization

Authentication is implemented using ASP.NET Core Identity and JWT Bearer authentication.

Available authentication endpoints:

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Authenticate and receive a JWT |

Protected game endpoints require a valid JWT.

Each game is associated with the authenticated user's ID, extracted from the token claims. Users cannot access, modify or delete games belonging to other accounts.

The API does not accept `UserId` from the game creation DTO. The server assigns the owner based on the authenticated user.

## API Endpoints

### Games

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/v1/Games` | Retrieve the current user's games |
| GET | `/api/v1/Games/{id}` | Retrieve a game by ID |
| POST | `/api/v1/Games` | Create a game |
| PUT | `/api/v1/Games/{id}` | Update a game |
| DELETE | `/api/v1/Games/{id}` | Delete a game |

### Reference data

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/v1/genres` | Retrieve available genres |
| GET | `/api/v1/platforms` | Retrieve available platforms |
| GET | `/api/v1/developers` | Retrieve available developers |

### Health checks

| Endpoint | Description |
|---|---|
| `/health/live` | Checks whether the application is running |
| `/health/ready` | Checks whether the application is ready, including database connectivity |

## Running with Docker

### Prerequisites

- Docker Desktop
- Git

### 1. Clone the repository

```bash
git clone <repository-url>
cd GameTracker
```

### 2. Configure environment variables

Create a `.env` file in the repository root, next to `compose.yml`.

```env
JWT_KEY=replace-with-a-unique-random-secret-at-least-32-characters-long
```

Use a unique, randomly generated key. Do not commit your `.env` file or share its secret.

An example configuration is provided in `.env.example`.

### 3. Start the application

```bash
docker compose up --build -d
```

Docker Compose builds the API image, creates the container and starts the application.

On startup, Entity Framework Core applies pending migrations and the application seeds reference data.

### 4. Access Swagger

Open:

```text
http://localhost:8080/swagger
```

You can use Swagger UI to explore and test the API.

### 5. Register and authenticate

Register a user using:

```http
POST /api/auth/register
```

Then log in:

```http
POST /api/auth/login
```

Copy the returned `accessToken`, click **Authorize** in Swagger UI and provide the token.

You can now access protected game endpoints.

### 6. Check application health

```text
http://localhost:8080/health/live
http://localhost:8080/health/ready
```

Check container status:

```bash
docker compose ps
```

### 7. Stop the application

```bash
docker compose down
```

The named Docker volume containing the SQLite database is preserved.

To remove the containers and the database volume:

```bash
docker compose down -v
```

**Warning:** The second command permanently removes the persisted Docker database.

## Testing

The solution contains two test projects.

### Application unit tests

`GameTracker.Application.Tests`

Tests application services independently using xUnit and Moq.

### API integration tests

`GameTracker.Api.Tests`

Uses `WebApplicationFactory` and an in-memory SQLite database to test HTTP requests through the ASP.NET Core pipeline.

Integration tests cover:

- Game CRUD operations.
- Authentication and authorization.
- User data isolation.
- Ownership validation.
- Duplicate game handling.
- HTTP response codes.

Run all tests:

```bash
dotnet test GameTracker.sln --configuration Release
```

## Continuous Integration

The project uses GitHub Actions.

On every push and pull request targeting `master` or `main`, the workflow:

1. Checks out the repository.
2. Sets up .NET 8.
3. Restores dependencies.
4. Builds the solution in Release configuration.
5. Runs automated tests.
6. Builds the Docker image.

The CI pipeline uses a dedicated test JWT configuration. Production secrets are not committed to the repository.

## Project Goals

GameTracker was created as a portfolio project to demonstrate practical backend development skills, including:

- Building RESTful APIs with ASP.NET Core.
- Applying Clean Architecture principles.
- Working with relational data using EF Core.
- Implementing authentication and authorization.
- Writing unit and integration tests.
- Containerizing applications with Docker.
- Automating build and test processes using GitHub Actions.

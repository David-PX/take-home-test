# Loan Management System — Take Home Test

This repository contains a simple **Loan Management System** composed of:

- **Backend**: .NET 6 REST API (C#) + Entity Framework Core + SQL Server (Docker)
- **Frontend**: Angular 19 application consuming the API (run locally)

> ✅ The backend is dockerized
> ✅ The frontend is executed locally (not dockerized for this delivery)

---

## Repository Structure

```
backend/
  src/
    Fundo.Applications.WebApi/
    Fundo.Services.Tests/
    Dockerfile
    docker-compose.yml
    .env
    src.sln

frontend/
  (Angular 19 project)
```

---

## Prerequisites

### Backend

- Docker Desktop (Mac / Windows / Linux)
- Docker Compose (included with Docker Desktop)

### Frontend

- Node.js 20+ (recommended)
- Angular CLI 19

```bash
npm install -g @angular/cli@19
```

---

## 1) Run Backend (API + SQL Server) with Docker Compose

All backend Docker files are located in:

```
backend/src
```

### 1.1 Configure environment variables

Create or update the file:

```
backend/src/.env
```

Example:

```env
SA_PASSWORD=yourStrong(!)Password
JWT_KEY=CHANGE_ME_TO_A_LONG_RANDOM_SECRET_32+_CHARS
```

**Notes**

- `SA_PASSWORD` must meet SQL Server password requirements
- `JWT_KEY` should be a long, random secret

---

### 1.2 Build and start containers

From the backend folder:

```bash
cd backend/src
docker compose --env-file .env up --build
```

This starts:

- **SQL Server** → `localhost:1433`
- **API** → `http://localhost:5000`

---

### 1.3 Verify backend

- API base URL:
  `http://localhost:5000`

- Swagger (if enabled):
  `http://localhost:5000/swagger`

#### Main API endpoints

```
POST   /auth/register
POST   /auth/login
GET    /customers
POST   /customers
GET    /loans
POST   /loans
GET    /loans/{id}
POST   /loans/{id}/payment
```

---

## 2) Run Frontend (Angular 19) Locally

Navigate to the frontend folder:

```bash
cd frontend
```

Install dependencies:

```bash
npm install
```

Run the application:

```bash
ng serve
```

Frontend URL:

```
http://localhost:4200
```

---

## 3) Connect Frontend to Backend

The backend runs on:

```
http://localhost:5000
```

Make sure your Angular app uses this URL when calling the API.

Typical approaches:

- `environment.ts` / `environment.prod.ts`
- Central API service configuration

Example:

```ts
const API_BASE_URL = "http://localhost:5000";
```

---

## 4) Authentication Flow

The API uses **JWT authentication**. Most endpoints require a valid token.

Recommended flow:

1. Register a user
   `POST /auth/register`

2. Login
   `POST /auth/login`

3. Store the returned JWT token (e.g. `localStorage`)

4. Send token on each request:

```
Authorization: Bearer <token>
```

---

## 5) Run Backend Tests (Optional)

Tests are located in:

```
backend/src/Fundo.Services.Tests
```

Run from `backend/src`:

```bash
dotnet test
```

---

## Troubleshooting

### API cannot connect to SQL Server

SQL Server may take time to initialize on first startup.

Suggestions:

- Wait until SQL logs show _“Recovery is complete”_
- Restart Docker Compose
- Ensure `SA_PASSWORD` meets SQL Server rules

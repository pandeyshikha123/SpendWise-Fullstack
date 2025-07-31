# SpendWise Fullstack Project

## Overview

SpendWise is a fullstack expense management application with a C# ASP.NET Core backend and an Angular frontend. It allows users to track expenses, manage categories, view analytics, and export data.

---

## Backend: ASP.NET Core

**Location:** `SpendWiseAPI/`

### Features
- JWT authentication (signup, signin, refresh token)
- Expense CRUD operations
- Category management
- MongoDB integration
- Email notifications
- Cloudinary image uploads
- Swagger API documentation

### How to Run

```sh
cd SpendWiseAPI
dotnet run
```

- Default API URL: `http://localhost:5083` (check your launchSettings.json for the actual port)
- Swagger UI: `/swagger`

### Configuration

Edit `SpendWiseAPI/appsettings.json` for:
- MongoDB connection
- JWT settings
- Email settings
- Cloudinary settings

---

## Frontend: Angular

**Location:** `frontend/`

### Features
- User authentication (JWT)
- Dashboard for expense management
- Category selection and filtering
- Analytics and spend ratio
- Profile management
- Data export (CSV, PDF)

### How to Run

```sh
cd frontend
npm install
npm start
```

- App URL: `http://localhost:4200`

### Configuration

Edit `frontend/src/environments/environment.ts` for the backend API URL.

---

## API Endpoints

- `POST /api/auth/signup` — Register
- `POST /api/auth/signin` — Login
- `POST /api/auth/refresh-token` — Refresh JWT
- `GET /api/auth/me` — Get current user
- `GET /api/expense` — List expenses
- `POST /api/expense` — Add expense
- `PUT /api/expense/{id}` — Update expense
- `DELETE /api/expense/{id}` — Delete expense
- `GET /api/category` — List categories
- `POST /api/category` — Add category

---

## Development Notes

- Update CORS and environment settings as needed.
- Use Swagger for backend API testing.
- For deployment, adjust environment variables and CORS.

---

## License

MIT License

---

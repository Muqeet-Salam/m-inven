# Auth and Inventory Management

A full-stack inventory management application with a Vue frontend and an ASP.NET Core Web API. The API provides JWT-based authentication and role-based authorization for users, products, and inventory transactions. Data is stored in MongoDB.

## Stack

- **Backend:** ASP.NET Core Web API on .NET 10
- **Database:** MongoDB / MongoDB Atlas
- **Authentication:** JWT bearer tokens
- **API documentation:** Swagger UI
- **Frontend:** Vue 3, Vue Router, Vite, Axios

## Project Structure

```text
authapi/   ASP.NET Core API, controllers, services, DTOs, and models
frontend/  Vue application
```

## Prerequisites

- .NET 10 SDK
- Node.js and npm
- A running MongoDB instance or a MongoDB Atlas connection string

## Configuration

The API requires MongoDB and JWT settings. Keep secrets out of source control. For local development, configure .NET user secrets from the `authapi` directory:

```bash
cd authapi
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb+srv://<user>:<password>@<cluster>/?retryWrites=true&w=majority"
dotnet user-secrets set "MongoDB:DatabaseName" "authdb"
dotnet user-secrets set "Jwt:Key" "replace-with-a-long-random-signing-key"
dotnet user-secrets set "Jwt:Issuer" "authapi"
```

The same values can be supplied through environment variables:

```text
MongoDB__ConnectionString
MongoDB__DatabaseName
Jwt__Key
Jwt__Issuer
```

The frontend currently expects the API at `http://localhost:5271/api`. Change `frontend/src/services/api.js` if the API runs at a different URL.

## Run Locally

Start the API:

```bash
cd authapi
dotnet restore
dotnet run
```

The API is available at `http://localhost:5271`. Swagger UI is available at `http://localhost:5271/swagger`.

In a second terminal, start the frontend:

```bash
cd frontend
npm install
npm run dev
```

The Vue application is available at `http://localhost:5173`.

## API Endpoints

All API routes are prefixed with `/api`. Protected routes require an HTTP header in this format:

```text
Authorization: Bearer <jwt-token>
```

| Method | Route | Access | Description |
| --- | --- | --- | --- |
| `POST` | `/api/auth/register` | Public | Register a user |
| `POST` | `/api/auth/login` | Public | Log in and receive a JWT |
| `PUT` | `/api/auth/users/{userId}/role` | Admin | Change a user's role |
| `GET` | `/api/user/me` | Authenticated | Get the current user's identity and role |
| `GET` | `/api/user/admin` | Admin | Verify Admin access |
| `GET` | `/api/test` | Public | Check that the API is running |
| `GET` | `/api/test/database` | Public | Check the MongoDB connection |
| `POST` | `/api/products` | Admin, Manager | Create a product |
| `GET` | `/api/products` | Authenticated | List all products |
| `GET` | `/api/products/{name}` | Authenticated | Find a product by name |
| `PUT` | `/api/products/{id}` | Admin, Manager | Update a product |
| `DELETE` | `/api/products?name={name}` | Admin | Delete a product by name |
| `POST` | `/api/inventory/{productId}/stock-in` | Admin, Manager, Staff | Add stock |
| `POST` | `/api/inventory/{productId}/stock-out` | Admin, Manager, Staff | Remove stock |
| `GET` | `/api/inventory/{productId}` | Admin, Manager | List product inventory transactions |

The stock endpoints accept `quantity` and optional `reason` query parameters, for example:

```text
POST /api/inventory/{productId}/stock-in?quantity=10&reason=Restock
```

## Roles

The available roles are `Admin`, `Manager`, `Staff`, and `Viewer`. New registrations are assigned according to the backend's registration service logic; an Admin can change a user's role through the role management endpoint.

## Build

Build the API:

```bash
cd authapi
dotnet build
```

Build the frontend for production:

```bash
cd frontend
npm run build
```

## Development Notes

- Swagger includes a Bearer authentication definition for testing protected endpoints.
- CORS is configured for the Vite development server at `http://localhost:5173`.
- The API stores users, products, and inventory transactions in separate MongoDB collections.

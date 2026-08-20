# Frontend — Local Setup & Development Runbook

How to get the Angular SPA running locally against the backend API.

## Stack

- **Angular 19** (standalone application, no NgModules)
- **TypeScript 5.7**
- **Angular Router** (`provideRouter`)
- **Angular Material + CDK** (azure-blue prebuilt theme)
- **RxJS 7.8** (ships with Angular)
- **SCSS** as the style language
- **`provideHttpClient()`** wired in app config (no API calls yet)

## Prerequisites

- Node.js **≥ 18** (`node --version`)
- npm **≥ 8** (`npm --version`)

## Install

```bash
cd frontend
npm install
```

## Run (development)

```bash
cd frontend
ng serve
```

Opens at **http://localhost:4200**.

## Production build

```bash
cd frontend
ng build
```

Output goes to `frontend/dist/recipe-manager-app/`.

## API configuration

The backend API base URL is configured in:

```
src/environments/environment.ts          # production
src/environments/environment.development.ts  # development (used by ng serve)
```

Both currently point to `http://localhost:5053` (the default backend URL — see `docs/backend-runbook.md`).

File replacements in `angular.json` swap the environment file automatically:
- `ng serve` → `environment.development.ts`
- `ng build` → `environment.ts`

Import and use in services:

```typescript
import { environment } from '../../environments/environment';

const url = `${environment.apiBaseUrl}/api/recipes`;
```

> **Do not** hardcode the API URL in components or services.

## Folder structure

```
src/
├── app/
│   ├── core/          # future: auth, interceptors, app-wide services
│   ├── shared/        # future: reusable UI components, pipes, directives
│   ├── features/      # future: auth, recipes, favorites, admin
│   ├── app.component.ts
│   ├── app.component.html
│   ├── app.component.scss
│   ├── app.config.ts
│   └── app.routes.ts
├── environments/      # environment.ts / environment.development.ts
├── index.html
├── main.ts            # standalone bootstrap
└── styles.scss        # global styles + Material theme imports
```

Empty directories (`core/`, `shared/`, `features/`) contain `.gitkeep` placeholders.

## Angular Material

- Prebuilt theme: `azure-blue` (loaded via `angular.json` styles array)
- Global font: Roboto (loaded via Google Fonts in `index.html`)
- Material Icons available via `MaterialIcons` font (also in `index.html`)
- SCSS setup: `styles.scss` imports `@angular/material` for future custom theming

No Bootstrap, Tailwind, or other CSS frameworks.

## What is NOT implemented yet

- Login / JWT storage / auth guards / HTTP interceptors
- Recipe, favorite, user, or admin screens
- API service layer or HTTP calls
- Angular Material showcase / demo pages
- Reactive Forms (available via `@angular/forms`; wire up when needed)
- NgRx state management (not in scope; add if the project grows)
- SSR / PWA
- Unit / e2e tests (project scaffolded with `--skip-tests`)

## CORS

The backend must allow `http://localhost:4200` as an origin. If CORS is not already configured for that origin, add it in the backend's CORS policy (typically in `Program.cs` or `appsettings.json`). Do **not** use a proxy-only workaround — configure real CORS.

## Next steps (recommended)

1. Add auth interceptor + JWT storage in `core/`
2. Build API service layer in `core/` using `environment.apiBaseUrl`
3. Create feature modules (auth, recipes, favorites, admin) under `features/`
4. Add route guards for protected pages
5. Wire up Reactive Forms for recipe creation/editing
6. Configure backend CORS for `http://localhost:4200` if not already done

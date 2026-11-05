<p align="center">
  <img src="src/raxui-dashboard/wwwroot/assets/raxui-logo.svg" width="64" height="64" alt="RaxUI" />
</p>

<h1 align="center">RaxUI Dashboard</h1>

<p align="center">An admin dashboard built with Blazor and RaxUI, with English, Arabic, and dark mode support.</p>

<p align="center"><strong>.NET 10 · Blazor Interactive Server · RaxUI · RTL</strong></p>

---

## Overview

A demo application bringing store reports, customers, and tasks into one interface, organized around a **feature-based architecture** and reusable components.

| Area | Features |
| --- | --- |
| Dashboards | Store overview, analytics, and CRM pipeline |
| Work management | Customers, Kanban task board, and calendar |
| User workspace | Demo assistant, profile, and settings |

The interface includes search and filters, create and edit forms, detail sheets, and toast notifications, with options for appearance, accent colors, and layout direction.

## Getting Started

Install the **.NET 10 SDK**, then run these commands from the repository root:

```sh
dotnet restore raxui-dashboard.slnx
dotnet run --project src/raxui-dashboard/raxui-dashboard.csproj --launch-profile http
```

Open [localhost:5111](http://localhost:5111). Stop the server with `Ctrl+C`.

## Project Structure

```text
raxui-dashboard.slnx
docker-compose.yml
src/raxui-dashboard/
├── Components/     Pages, layout, shared components, and RaxUI primitives
├── Features/       Components, models, and services per feature
├── Infrastructure/ Demo data sources
├── Models/         Shared reporting metrics
├── Services/       Localization, themes, time, and service registration
├── wwwroot/        Styles, assets, and static files
└── Dockerfile      Production container build
tests/raxui-dashboard.Tests/
```

Library configuration lives in `raxui.json`, with component installation records in `raxui.lock.json`. RaxUI components are included as source files in the project. Application styles are maintained in `wwwroot/app.css`.

## Build and Test

```sh
dotnet build raxui-dashboard.slnx --no-restore
dotnet test tests/raxui-dashboard.Tests/raxui-dashboard.Tests.csproj --no-restore
```

## Development Guides

- [SKILL.md](SKILL.md) — Practical guidance for creating features and pages and using RaxUI. Ask your coding assistant to read it when working on this project.
- [ARCHITECTURE.md](ARCHITECTURE.md) — Application structure, data flow, and service lifetimes.

## Demo Scope

Data, saves, assistant responses, and exports are simulated. There is no database, real authentication, or external AI provider connection. Feature edits reset when a page is recreated; appearance and language preferences are stored in the browser. The default time zone is `Asia/Baghdad`, configurable through `Application:TimeZone` in `appsettings.json`.

## Deployment

Use the root `docker-compose.yml` with Dokploy. Set Compose Path to `./docker-compose.yml`, then add `blazor.raxui.com` in Domains with service `dashboard`, container port `8080`, and HTTPS using Let's Encrypt. Point the domain's DNS A record to your Dokploy server before deploying.

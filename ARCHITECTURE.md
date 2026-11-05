# Dashboard architecture

This is one .NET 10 Blazor Web App. The component library and application features have separate responsibilities; the application remains a demo with deterministic seed data and simulated saves.

## Structure

The solution is at the repository root and the application is under `src/raxui-dashboard`. Application paths below are relative to that folder; `tests/` is relative to the repository root.

| Location | Responsibility |
| --- | --- |
| `Components/Pages` | Route entry points. Customers and Tasks also coordinate their forms, sheets, and selection state. |
| `Features/Customers` | Customer models, validation, operations, summary, form, detail sheet, and the table under `Components/Table`. |
| `Features/Tasks` | Task models, validation, board operations, form, detail sheet, and card content. |
| `Features/Analytics` | Report loading, realtime readings, five report tabs, and the analytics page table. |
| `Features/Assistant` | Chat presentation, turn models, cancellable streaming session, and demo answer routing. |
| `Features/Store` | Store report, period/channel options, order models, and recent orders table. |
| `Features/Crm` | CRM report, pipeline models, opportunities table, and meeting status. |
| `Features/Calendar` | Calendar navigation, event models, daily agenda, and upcoming events. |
| `Features/Profile` | Profile editor, draft validation, and simulated save status. |
| `Features/Settings` | Settings tabs, appearance controls, and page-owned notification choices. |
| `Models/Reporting` | Metric records shared by reporting features. |
| `Components/Shared` | Application components reused across pages, such as icons, submit buttons, and KPI displays. |
| `Components/Layout` | Application shell, navigation, and reconnection UI. |
| `Components/UI` | Source-owned RaxUI primitives; feature-specific behavior belongs in `Features`. |
| `Infrastructure/DemoData` | Read-only sample data, dashboard projections, and the simulated save delay. |
| `Services/Theme` | Circuit-specific theme/language state and browser preference storage. |
| `Services/Localization` | The `Ui` API and translation catalogs grouped by subject. |
| `Services/Time` | Testable local time, backed by `TimeProvider` and configured time zone. |
| `wwwroot/app.css` | Application styling, organized into named sections. Library styles stay under `css/raxui`. |
| `tests/raxui-dashboard.Tests` | Feature operations, validation, service lifetimes, theme/clock behavior, and component rendering. |

`_Imports.razor` is at the project root so routed pages and feature components share the same Razor imports.

## Data flow and ownership

Customer and task pages coordinate feature components through parameters and callbacks. Forms edit draft models; only a valid submission invokes the feature service. Sheets request edit/delete through callbacks, and pages retain the confirmation step. Summary components derive their figures from the current working set.

`Features/Customers/Components/Table/CustomersTable.razor` owns the table markup and cell templates. Its `.razor.cs` partial class owns columns, labels, export feedback, and selection clearing. The page supplies customers and handles view/edit/delete callbacks; dialogs and data mutations stay in the page and customer service.

RaxUI's source-owned `Components/UI/Toast/Toaster` is mounted once in `MainLayout`. Its `Toaster.ToastService` is scoped to the Blazor circuit and injected into presentation components for success, error, and informational feedback. Notify after the operation completes; validation errors remain next to their fields. Toasts replace transient inline status messages for customer/task operations and profile saves. Export and notification-setting feedback explicitly describe their demo limitations. The viewport follows the interface direction, and the library handles accessible announcements, dismissal, timeout, and pausing on hover/focus.

`CustomerService` and `TaskService` are transient, page-owned working sets. This deliberately preserves the previous demo behavior: navigating away and creating a new page instance resets all demo edits, including simulated saves. They are not persistence stores. Replacing the demo data adapter with durable storage belongs behind these services, rather than inside Razor components.

The task service uses `TaskColumn` domain records. The task page adapts them to RaxUI's `KanbanTypes.Column` at the UI boundary, including drag/drop updates. Application operations do not depend on the Kanban component.

The remaining route files render a feature root component. Each feature keeps its components, models, and services together. Reporting roots own their selected range/filter/tab state; their services load data through the demo adapter. Tables and report tabs receive models through parameters. Calendar selection stays in its root component, while agenda and upcoming-event components receive data and emit selection callbacks.

`AssistantSession` owns chat turns, streaming state, and cancellation. Its component subscribes to changes and disposes the session on navigation. `AssistantAnswerService` still generates deterministic demo responses from dashboard data; it does not call an external AI provider. `ProfileService` owns its validated draft and save timestamp. `SettingsService` owns notification choices; persistent appearance preferences remain in `ThemeService`.

Feature services are transient. Their mutable state belongs to the feature instance, so navigation preserves the existing reset behavior. `DashboardData` is a singleton because it contains deterministic seed/projection data, not mutable per-user state. Do not put editable customer lists or user preferences in it. Razor presentation components do not depend directly on this adapter. Feature models live with their features; only shared metric records live in `Models/Reporting`. Saves and notification choices remain demonstrations without durable persistence.

## Theme, translation, and time

`ThemeService` is scoped to the Blazor circuit. `MainLayout` initializes it after the interactive render, subscribes to changes, and cascades the current `Ui`. Settings injects the service directly and has no dependency on a layout instance. The existing browser storage key and theme application before first paint are preserved.

Theme buttons and settings controls carry `data-dash-theme-*` attributes. The head-loaded theme script applies and saves their changes synchronously in the browser; their Blazor callbacks read the latest browser state without reapplying an older selection. Command palette actions still use `ThemeService.SetAsync`. Cosmetic changes retain the current `Ui` instance; changing the interface language replaces it and updates translated content through the server circuit.

Store, CRM, and Analytics load their in-memory reports during `OnInitializedAsync`, so the initial HTML includes report data before interactivity connects. Chart drawing and Analytics' live timer still start only after interactive rendering.

Translations preserve `L["English key"]`, Arabic culture formatting, and English fallback. Catalogs are merged in their original order; later values retain the original dictionary's override behavior for repeated keys.

`AppClock` uses `TimeProvider` and `Application:TimeZone` (default `Asia/Baghdad`). Task defaults/overdue checks, new customer dates, and profile save timestamps use this clock. Historical dashboard/chart/calendar sample data keeps its fixed reference dates through `DemoSchedule` so the seed data remains internally consistent. Analytics live readings and assistant streaming delays use the injected `TimeProvider`.

## Development and verification

From the repository root:

```sh
dotnet restore raxui-dashboard.slnx
dotnet build raxui-dashboard.slnx --no-restore
dotnet test tests/raxui-dashboard.Tests/raxui-dashboard.Tests.csproj --no-restore
dotnet run --project src/raxui-dashboard/raxui-dashboard.csproj --launch-profile http
```

The application and tests are in separate folders; the solution includes both projects. Rendering tests use Blazor's `HtmlRenderer`, so no browser installation is required for the automated suite.

For UI changes, also verify customer identity-to-sheet navigation, create/edit/confirmed-delete, task radio choices and column moves, keyboard Kanban movement, Arabic/dark mode persistence, and mobile dialog/sheet layout. Keep routes and application CSS class contracts stable when extracting further features.

No database, authentication system, generic repository, or additional production application project is introduced by this structure.

## Container deployment

The root docker-compose.yml builds src/raxui-dashboard/Dockerfile. Dokploy terminates TLS and routes to port 8080. The container runs as the non-root app user, and Data Protection keys persist in a named volume. The /health endpoint reports application health. Forwarded headers are enabled only in Compose; the container has no published host ports. See the Deployment section in [README.md](README.md) for domain configuration.

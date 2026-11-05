---
name: raxui-dashboard
description: Develop and maintain this RaxUI Dashboard repository, including Blazor features, routed pages, RaxUI components, forms, tables, localization, themes, and toast feedback. Use when implementing or refactoring application code in this project.
---

# RaxUI Dashboard Development

This skill applies to this repository: a .NET 10 Blazor Web App using Interactive Server and source-owned RaxUI components. Read [ARCHITECTURE.md](ARCHITECTURE.md) for ownership and data behavior, then inspect the closest existing feature before implementing changes.

Application paths in this guide are relative to `src/raxui-dashboard`. Run solution build/test commands from the repository root, and RaxUI CLI commands from the application folder.

## Where Code Belongs

| Responsibility | Location |
| --- | --- |
| Route entry points | `Components/Pages` |
| Feature UI and logic | `Features/<Feature>/Components`, `Models`, `Services` |
| Application components reused across features | `Components/Shared` |
| Application shell and navigation | `Components/Layout/MainLayout.razor` |
| RaxUI primitives | `Components/UI/<Component>` |
| Service registration | `Services/ServiceCollectionExtensions.cs` |
| Translations | `Services/Localization/Ui.*.cs` |
| Demo data sources | `Infrastructure/DemoData` |
| Application styling | `wwwroot/app.css` |

## Creating Features and Pages

1. Create only the folders needed under `Features/<Feature>`, using PascalCase names. Put view and draft models in `Models`, and data operations and validation in `Services`.
2. Keep route files small: declare `@page` and render the feature root component. Place `PageTitle` and `h1` in that root component, following existing pages.
3. Keep search, filters, the selected tab, and dialog state in the coordinating component. Pass data down through `[Parameter]` and emit events through `EventCallback<T>`.
4. Register services in `AddDashboardApplication`. Add namespaces to `_Imports.razor` or the consuming file. Update `MainLayout` navigation and its translations when the page needs a navigation link.

Example route file, after importing the feature namespace:

```razor
@page "/reports"

<ReportsDashboard />
```

Most existing pages follow this pattern. Customers and Tasks still coordinate dialogs and presentation operations inside their route files. Move those responsibilities only when the task calls for it.

## Using RaxUI

- Inspect the actual component API in `Components/UI`, including its `[Parameter]` declarations and companion JavaScript, before using it. Do not assume property names from another library.
- Reuse `Button`, `Dialog`, `Sheet`, `ConfirmDialog`, `Field`, `Select`, `DataTable`, `Kanban`, and `Toaster` where appropriate. Use `Components/Shared/Lucide.razor` for existing icons.
- Library configuration lives in `raxui.json`; installation records and file hashes live in `raxui.lock.json`. Styles are imported by `wwwroot/css/raxui/raxui.css`, which `Components/App.razor` loads.
- To install a missing component, use `raxui add <component>` from the project root when the CLI is available, respecting the configured registry version. This project is already initialized; do not rerun `raxui init` to add a component.
- When using a local RaxUI source, inspect the component manifest, dependencies, files, and styles. Verify file hashes and update installation records to reflect the actual source without overwriting existing local changes.
- Keep customer, task, and other business behavior in features. Change a primitive for a general component fix; put page-specific styling in `app.css`.
- Files under `wwwroot/css/raxui/components/*.css` are generated library styles. Use a scoped application class in `app.css` for page customization. For a general library styling fix, work from its source and generation process.

## Forms, Tables, and Overlays

- Use a separate draft such as `CustomerDraft` with DataAnnotations. Bind it to `EditForm` with `DataAnnotationsValidator` and display errors through `FieldError`. Services also validate before mutating data.
- Submit through `OnValidSubmit` and display pending state with `SubmitButton`. Guard against repeated submissions and reset pending state in `finally`.
- Use `ConfirmDialog` before deletion. Its handler keeps the dialog open and displays errors when `OnConfirm` fails; do not return successfully after a failed deletion.
- Use `Sheet` for details and `Dialog` for creation and editing, following existing patterns. Preserve `Open`/`OpenChanged` binding and keyboard dismissal.
- Group related table files when their size justifies it, as in `Features/Customers/Components/Table`: markup and Razor templates in `.razor`; columns, parameters, and helpers in `.razor.cs`, using a `partial class` and matching namespace.
- Column options, translations, and cell templates belong to presentation; loading, saving, and deletion belong to services. Preserve selection, sorting, and filters across renders, and clear selection after operations that require it.
- For tasks, the service uses `TaskColumn`; conversion to `KanbanTypes.Column<TaskCard>` happens at the UI boundary.

## Toast Feedback

`MainLayout` mounts one `Toaster`, and `Toaster.ToastService` is registered as scoped. Inject the service into the presentation component performing the operation:

```razor
@inject Toaster.ToastService Toasts
```

```csharp
await CustomerStore.SaveAsync(draft, editingId);
Toasts.Show(L["Customer updated"], "success", draft.Name.Trim());
```

Show success after the operation completes. Use `danger` for failures and `info` for information. Keep field validation errors next to their fields. Do not mount a toaster on every page or register its service as a singleton. Export and demo-setting messages must describe what actually happened.

## Localization, Themes, and Styling

- Receive `Ui L` through `[CascadingParameter]` and use `L["English key"]`. Add keys to the appropriate catalog; register new catalogs in `Ui.CreateStrings()`.
- Preserve translation merging through `result[item.Key] = item.Value`; some repeated keys depend on section precedence.
- Use `L.Culture` for dates and numbers, and `L.Money` for currency. Isolate email addresses, identifiers, and mixed-direction values with `dir="ltr"` or `bdi` when needed.
- Use logical CSS properties such as `margin-inline` and `padding-inline`. Follow existing theme tokens and avoid hardcoded background or text colors that break dark mode.
- Put application styles in the relevant section of `wwwroot/app.css`. The current convention does not use page-specific `.razor.css` files; do not automatically add one for every component.
- `ThemeService` manages theme and language per circuit and stores preferences in the browser. Its first initialization runs after interactive rendering in `MainLayout`; do not access browser storage or the DOM during prerendering.

## Data and Service Lifetimes

- This is a demo: there is no database or real authentication, the assistant simulates responses and streaming, and export currently downloads no file. Do not describe these operations as live integrations.
- Feature services are transient, preserving resets when a page is recreated. `DashboardData` is a singleton for demo source data only; `ThemeService` and toasts are scoped. Do not store mutable user state in a singleton.
- Use `AppClock` for current dates and form defaults, and `DemoSchedule` for consistent demo reports and calendar data. Use `TimeProvider` for testable timing.
- Cancel timers and streaming when leaving a component, and unsubscribe from events, as Analytics and Assistant do. Run rendering-related JavaScript calls after interactivity is available.

## Verification and Delivery

Run the checks appropriate to the change from the repository root:

```sh
dotnet build raxui-dashboard.slnx --no-restore
dotnet test tests/raxui-dashboard.Tests/raxui-dashboard.Tests.csproj --no-restore
dotnet run --project src/raxui-dashboard/raxui-dashboard.csproj --launch-profile http
```

Run `dotnet restore raxui-dashboard.slnx` first when needed. Rendering tests use `HtmlRenderer` and require no browser. When interactions change, verify the affected behavior in a browser: the primary operation, validation, confirmation, Arabic and RTL, dark mode, and mobile layout as relevant to the task.

If the application is already running, use a separate port and output directory for verification, then stop only the preview instance you created. Report what changed, what was verified, and any remaining limitations. Update `ARCHITECTURE.md` when ownership or operating instructions change.

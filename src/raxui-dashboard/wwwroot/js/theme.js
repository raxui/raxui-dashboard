// Classic script loaded in <head>: applies the saved theme before first paint, then lets MainLayout read and change it.
window.dashTheme = (() => {
  const key = "rax-dashboard-theme";
  const html = document.documentElement;

  const readSaved = () => {
    let saved = null;
    try { saved = JSON.parse(localStorage.getItem(key)); } catch { }
    return {
      appearance: saved?.appearance ?? html.dataset.raxAppearance ?? "glass",
      accent: saved?.accent ?? html.dataset.raxAccent ?? "indigo",
      dark: saved?.dark ?? matchMedia("(prefers-color-scheme: dark)").matches,
      rtl: saved?.rtl ?? false,
    };
  };

  let current = readSaved();
  const load = () => ({ ...current });

  const apply = (theme, save) => {
    current = { ...theme };
    html.dataset.raxAppearance = theme.appearance;
    html.dataset.raxAccent = theme.accent;
    html.dataset.bsTheme = theme.dark ? "dark" : "light";
    html.dir = theme.rtl ? "rtl" : "ltr";
    html.lang = theme.rtl ? "ar" : "en";
    // Bootstrap utilities are physical (ms-*, text-start...), so RTL needs its RTL build.
    const bootstrap = document.getElementById("bootstrap-css");
    const href = theme.rtl ? bootstrap?.dataset.rtl : bootstrap?.dataset.ltr;
    if (bootstrap && href && bootstrap.getAttribute("href") !== href) bootstrap.setAttribute("href", href);
    if (save) try { localStorage.setItem(key, JSON.stringify(theme)); } catch { }
  };

  // Apply cosmetic preferences before a Blazor event makes a network round trip.
  // The existing component callbacks then read this state to keep C# in sync.
  document.addEventListener("click", event => {
    const control = event.target.closest("[data-dash-theme-toggle], [data-dash-theme-value]");
    if (!control || control.disabled || control.getAttribute("aria-disabled") === "true") return;
    const toggle = control.dataset.dashThemeToggle;
    const field = toggle || control.dataset.dashThemeField;
    if (!["dark", "rtl", "appearance", "accent"].includes(field)) return;
    apply({ ...current, [field]: toggle ? !current[field] : control.dataset.dashThemeValue }, true);
  }, true);

  document.addEventListener("change", event => {
    const control = event.target;
    const field = control.dataset?.dashThemeField;
    if (!["dark", "rtl", "appearance", "accent"].includes(field) || control.disabled) return;
    apply({ ...current, [field]: control.type === "checkbox" ? control.checked : control.value }, true);
  }, true);

  apply(load(), false);
  return { load, apply };
})();

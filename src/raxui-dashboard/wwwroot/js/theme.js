// Classic script loaded in <head>: applies the saved theme before first paint, then lets MainLayout read and change it.
window.dashTheme = (() => {
  const key = "rax-dashboard-theme";
  const html = document.documentElement;

  const load = () => {
    let saved = null;
    try { saved = JSON.parse(localStorage.getItem(key)); } catch { }
    return {
      appearance: saved?.appearance ?? html.dataset.raxAppearance ?? "glass",
      accent: saved?.accent ?? html.dataset.raxAccent ?? "indigo",
      dark: saved?.dark ?? matchMedia("(prefers-color-scheme: dark)").matches,
      rtl: saved?.rtl ?? false,
    };
  };

  const apply = (theme, save) => {
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

  apply(load(), false);
  return { load, apply };
})();

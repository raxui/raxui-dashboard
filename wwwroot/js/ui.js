// Small DOM helpers for pages (imported as a module).

// Keeps the selected tab visible when a tab list scrolls sideways on narrow screens.
export function revealSelected(listSelector) {
  const tab = document.querySelector(`${listSelector} [aria-selected="true"]`);
  const reduce = matchMedia("(prefers-reduced-motion: reduce)").matches;
  tab?.scrollIntoView({ block: "nearest", inline: "nearest", behavior: reduce ? "auto" : "smooth" });
}

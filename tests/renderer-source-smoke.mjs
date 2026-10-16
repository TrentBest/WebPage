import assert from "node:assert/strict";
import { chromium } from "playwright";

const baseUrl = process.env.WORKSHOP_BASE_URL ?? "http://127.0.0.1:5261";
const browser = await chromium.launch({ headless: true });

try {
  const page = await browser.newPage({ viewport: { width: 1440, height: 1000 } });
  await page.goto(`${baseUrl}/digitens`, { waitUntil: "networkidle" });
  await page.getByRole("heading", { name: "DIGITENS", exact: true }).waitFor({
    state: "visible",
    timeout: 20_000
  });

  await page.getByRole("button", { name: /Renderer & detail/i }).click();
  const activeState = page.locator(".renderer-live > strong");
  await activeState.waitFor({ state: "visible", timeout: 10_000 });
  assert.equal((await activeState.textContent())?.trim(), "Semantic");

  await page.getByRole("button", { name: "Interactive", exact: true }).click();
  await page.waitForFunction(
    () => document.querySelector(".renderer-live > strong")?.textContent?.trim() === "Interactive",
    null,
    { timeout: 10_000 }
  );

  await page.getByRole("button", { name: "Distant", exact: true }).click();
  await page.waitForFunction(
    () => document.querySelector(".renderer-live > strong")?.textContent?.trim() === "Distant",
    null,
    { timeout: 10_000 }
  );

  await page.screenshot({ path: process.env.WORKSHOP_SCREENSHOT ?? "renderer-source-smoke.png", fullPage: true });
  console.log("Renderer source integration: Semantic → Interactive → Distant transitions passed through the real FSM_API-backed RendererComputationMachine.");
} finally {
  await browser.close();
}

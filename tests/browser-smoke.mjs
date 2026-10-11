import { chromium } from "playwright";

const baseUrl = process.env.WORKSHOP_BASE_URL ?? "http://127.0.0.1:5260";
const outputDir = process.env.WORKSHOP_SCREENSHOT_DIR ?? "TestResults/Browser";
const browserErrors = [];

const browser = await chromium.launch({ headless: true });
try {
  const page = await browser.newPage({ viewport: { width: 1440, height: 900 }, deviceScaleFactor: 1 });
  page.on("pageerror", error => browserErrors.push(error.message));

  await page.goto(baseUrl, { waitUntil: "domcontentloaded", timeout: 30_000 });
  await page.getByText("IF AN IMAGE IS WORTH A THOUSAND WORDS...", { exact: true }).waitFor({ state: "visible", timeout: 30_000 });
  await page.getByText("HOW MANY WORDS IS A LIVING IMAGE WORTH?", { exact: true }).waitFor({ state: "visible", timeout: 30_000 });
  await page.locator(".enter-workshop").waitFor({ state: "visible", timeout: 45_000 });
  await page.screenshot({ path: `${outputDir}/01-gateway.png`, fullPage: true });

  await page.getByRole("button", { name: /enter the singularity workshop/i }).click();
  const moniker = page.locator(".hello-moniker");
  await moniker.waitFor({ state: "visible", timeout: 180_000 });
  await page.screenshot({ path: `${outputDir}/02-moniker.png`, fullPage: true });

  const hub = page.locator(".manifest-hub");
  await hub.waitFor({ state: "visible", timeout: 240_000 });
  const tabs = page.locator(".manifest-hub-tabs");
  const monikerTab = tabs.getByRole("button", { name: "THE WORKSHOP", exact: true });
  const anyAppTab = tabs.getByRole("button", { name: "ANYAPP", exact: true });
  const supportTab = tabs.getByRole("button", { name: "SUPPORT", exact: true });

  await monikerTab.waitFor({ state: "visible", timeout: 10_000 });
  await anyAppTab.waitFor({ state: "visible", timeout: 10_000 });
  await supportTab.waitFor({ state: "visible", timeout: 10_000 });
  await moniker.waitFor({ state: "visible", timeout: 10_000 });
  if (await monikerTab.getAttribute("aria-pressed") !== "true") {
    throw new Error("The Workshop moniker must own the initial hub view; no destination is selected automatically.");
  }
  if (await page.getByRole("heading", { name: "AnyApp", exact: true }).count() !== 0) {
    throw new Error("AnyApp content must remain hidden until the visitor explicitly selects its tab.");
  }
  if (await page.getByText("WELCOME", { exact: true }).count() !== 0) {
    throw new Error("The hub should present the living moniker, not a generic welcome landing page.");
  }
  await page.screenshot({ path: `${outputDir}/03-hub-moniker.png`, fullPage: true });

  await anyAppTab.click();
  await page.getByRole("heading", { name: "ANYAPP", exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  if (await moniker.count() !== 0) {
    throw new Error("Selecting AnyApp must replace the moniker in the primary panel.");
  }
  await page.screenshot({ path: `${outputDir}/04-hub-anyapp.png`, fullPage: true });

  await supportTab.click();
  await page.getByRole("heading", { name: "SUPPORT THE WORKSHOP.", exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  if (await page.getByRole("heading", { name: "ANYAPP", exact: true }).count() !== 0) {
    throw new Error("Selecting Support must replace AnyApp content.");
  }
  await page.screenshot({ path: `${outputDir}/05-hub-support.png`, fullPage: true });

  // Return to the living moniker and check the hub at a narrow mobile viewport.
  await monikerTab.click();
  await moniker.waitFor({ state: "visible", timeout: 10_000 });
  await page.setViewportSize({ width: 390, height: 844 });
  await monikerTab.waitFor({ state: "visible", timeout: 10_000 });
  await page.screenshot({ path: `${outputDir}/06-hub-mobile.png`, fullPage: true });

  if (browserErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected:\n${browserErrors.join("\n")}`);
  }

  console.log("Browser journey passed: gateway -> moniker -> manifest-driven hub (desktop and mobile viewport).");
} finally {
  await browser.close();
}

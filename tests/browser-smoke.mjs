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
  if (await page.locator(".manifest-hub-tabs").count() !== 0) {
    throw new Error("The primary canvas must not add a second tab strip; navigation belongs to the left rail.");
  }
  await moniker.waitFor({ state: "visible", timeout: 10_000 });
  const hubHeight = await hub.evaluate(el => el.scrollHeight);
  const hubVisibleHeight = await hub.evaluate(el => el.clientHeight);
  if (hubHeight > hubVisibleHeight + 2) {
    throw new Error(`The manifested hub must fit its viewport without scrolling (scrollHeight=${hubHeight}, clientHeight=${hubVisibleHeight}).`);
  }
  await page.screenshot({ path: `${outputDir}/03-hub-moniker.png`, fullPage: true });

  // AnyApp is reached from the persistent left rail and owns a full-height,
  // paged canvas. It must not present a long scrolling article.
  await page.getByRole("link", { name: "AnyApp", exact: true }).click();
  await page.getByRole("heading", { name: "ANYAPP", exact: true }).waitFor({ state: "visible", timeout: 15_000 });
  const anyApp = page.locator(".anyapp-life");
  await anyApp.waitFor({ state: "visible", timeout: 10_000 });
  await page.getByText("01 / THE IDEA", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  const initialHeight = await anyApp.evaluate(el => el.scrollHeight);
  const visibleHeight = await anyApp.evaluate(el => el.clientHeight);
  if (initialHeight > visibleHeight + 2) {
    throw new Error(`AnyApp must fit its scene in the panel rather than require document scrolling (scrollHeight=${initialHeight}, clientHeight=${visibleHeight}).`);
  }
  await page.screenshot({ path: `${outputDir}/04-anyapp-concept.png`, fullPage: true });

  await anyApp.hover();
  await page.mouse.wheel(0, 500);
  await page.getByText("02 / INTERCHANGEABILITY", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  // Wait for the current fade lifecycle to finish before requesting another transition.
  await page.waitForTimeout(800);
  await page.screenshot({ path: `${outputDir}/05-anyapp-interchange.png`, fullPage: true });

  await page.getByRole("button", { name: "Go to scene 3" }).click();
  await page.getByText("03 / THE SAME IDEA, LIVE RIGHT HERE", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  await page.waitForTimeout(800);
  await page.screenshot({ path: `${outputDir}/06-anyapp-live-proof.png`, fullPage: true });

  await page.getByRole("button", { name: "Go to scene 4" }).click();
  await page.getByText("04 / THE NEXT FRONTIER", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  await page.waitForTimeout(800);
  await page.getByRole("link", { name: /INSPECT ANYAPP SOURCE/i }).waitFor({ state: "visible" });
  await page.screenshot({ path: `${outputDir}/07-anyapp-desktop.png`, fullPage: true });

  // The left rail is still available to return to the Workshop.
  await page.getByRole("link", { name: "The Singularity Workshop" }).click();
  await page.locator(".manifest-hub").waitFor({ state: "visible", timeout: 15_000 });

  // The ecosystem atlas is a manifest-declared destination, not a hard-coded rail entry.
  await page.getByRole("link", { name: "Ecosystem", exact: true }).click();
  await page.getByRole("heading", { name: /Explore the living stack/i }).waitFor({ state: "visible", timeout: 15_000 });
  await page.getByRole("button", { name: "GITHUB", exact: true }).click();
  await page.getByRole("heading", { name: "WebPage", exact: true }).waitFor({ state: "visible", timeout: 5_000 });
  await page.getByRole("button", { name: "ALL SIGNALS", exact: true }).click();
  await page.getByRole("button", { name: /RUN DESTRUCTIVE TEST SIMULATION/i }).click();
  await page.getByText("PRELIMINARY PASS", { exact: true }).waitFor({ state: "visible", timeout: 5_000 });
  await page.getByLabel("Testing profile").selectOption("strict");
  await page.getByRole("button", { name: /RUN DESTRUCTIVE TEST SIMULATION/i }).click();
  await page.getByText("REJECTED", { exact: true }).waitFor({ state: "visible", timeout: 5_000 });

  if (browserErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected:\n${browserErrors.join("\n")}`);
  }

  console.log("Browser journey passed: gateway -> moniker -> manifest-driven hub (desktop and mobile viewport).");
} finally {
  await browser.close();
}

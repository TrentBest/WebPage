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
  await page.locator(".manifest-hub-tabs").getByText("ANYAPP", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  if (await page.getByText("WELCOME", { exact: true }).count() !== 0) {
    throw new Error("The hub should present manifest navigation, not a generic welcome landing page.");
  }
  if (await page.getByText("A public window into composable software.", { exact: true }).count() !== 0) {
    throw new Error("The hub should not insert the retired marketing brief.");
  }
  await page.screenshot({ path: `${outputDir}/03-hub.png`, fullPage: true });

  // Re-check the finished hub at a narrow mobile viewport. This catches responsive
  // regressions without restarting the time-dependent opening animation.
  await page.setViewportSize({ width: 390, height: 844 });
  await hub.waitFor({ state: "visible", timeout: 10_000 });
  await page.locator(".manifest-hub-tabs").getByText("ANYAPP", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  await page.screenshot({ path: `${outputDir}/04-hub-mobile.png`, fullPage: true });

  if (browserErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected:\n${browserErrors.join("\n")}`);
  }

  console.log("Browser journey passed: gateway -> moniker -> manifest-driven hub (desktop and mobile viewport).");
} finally {
  await browser.close();
}

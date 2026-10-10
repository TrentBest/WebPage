import { chromium } from "playwright";

const baseUrl = process.env.WORKSHOP_BASE_URL ?? "http://127.0.0.1:5260";
const outputDir = process.env.WORKSHOP_SCREENSHOT_DIR ?? "TestResults/Browser";
const browserErrors = [];

const browser = await chromium.launch({ headless: true });
try {
  const page = await browser.newPage({ viewport: { width: 1440, height: 900 }, deviceScaleFactor: 1 });
  page.on("pageerror", error => browserErrors.push(error.message));

  await page.goto(baseUrl, { waitUntil: "domcontentloaded", timeout: 30_000 });
  await page.locator(".enter-workshop").waitFor({ state: "visible", timeout: 45_000 });
  await page.screenshot({ path: `${outputDir}/01-gateway.png`, fullPage: true });

  await page.getByRole("button", { name: /enter the singularity workshop/i }).click();
  const moniker = page.locator(".hello-moniker");
  await moniker.waitFor({ state: "visible", timeout: 180_000 });
  await page.screenshot({ path: `${outputDir}/02-moniker.png`, fullPage: true });

  const hubTitle = page.locator(".manifest-hub-title");
  await hubTitle.waitFor({ state: "visible", timeout: 240_000 });
  if ((await hubTitle.innerText()).trim() !== "WELCOME") {
    throw new Error("Expected the manifest-driven hub to show WELCOME.");
  }
  await page.getByText("A public window into composable software.", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  await page.screenshot({ path: `${outputDir}/03-hub.png`, fullPage: true });

  if (browserErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected:\n${browserErrors.join("\n")}`);
  }

  console.log("Browser journey passed: gateway -> moniker -> manifest-driven hub.");
} finally {
  await browser.close();
}

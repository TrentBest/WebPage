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

  const hubLinks = await page.locator(".workshop-nav-links a").allTextContents();
  for (const required of ["Ecosystem", "Schedule Consultation", "Institutions"]) {
    if (!hubLinks.some(label => label.trim() === required)) {
      throw new Error(`Expected manifest-driven hub destination '${required}'.`);
    }
  }
  if (hubLinks.some(label => label.trim() === "AnyApp")) {
    throw new Error("AnyApp must not appear as a Workshop hub destination.");
  }

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

  await page.getByRole("link", { name: /RETURN TO HUB/i }).click();
  await page.getByRole("link", { name: "Schedule Consultation", exact: true }).click();
  const schedule = page.locator(".schedule-experience");
  await schedule.waitFor({ state: "visible", timeout: 15_000 });
  const scheduleFits = await schedule.evaluate(el => el.scrollHeight <= el.clientHeight + 2);
  if (!scheduleFits) throw new Error("The consultation experience must fit its viewport without scrolling.");
  await page.getByRole("heading", { name: /Let's find the/i }).waitFor({ state: "visible" });
  await page.screenshot({ path: `${outputDir}/04-schedule.png`, fullPage: true });

  await page.getByRole("link", { name: /RETURN TO HUB/i }).click();
  await page.getByRole("link", { name: "Institutions", exact: true }).click();
  const institutions = page.locator(".institution-brief");
  await institutions.waitFor({ state: "visible", timeout: 15_000 });
  const institutionsFits = await institutions.evaluate(el => el.scrollHeight <= el.clientHeight + 2);
  if (!institutionsFits) throw new Error("The institutional briefing must fit its viewport without scrolling.");
  await page.getByRole("heading", { name: /Behavior|Then, into|Now, the|Demonstrate/i }).waitFor({ state: "visible" });
  await page.getByRole("button", { name: "EVOLUTION", exact: true }).click();
  await page.getByRole("heading", { name: /Then, into Unity/i }).waitFor({ state: "visible" });
  await page.getByRole("button", { name: "NEXT GATE", exact: true }).click();
  await page.getByRole("heading", { name: /Demonstrate/i }).waitFor({ state: "visible" });
  await page.screenshot({ path: `${outputDir}/05-institutions.png`, fullPage: true });

  if (browserErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected:\n${browserErrors.join("\n")}`);
  }

  console.log("Browser journey passed: gateway -> moniker -> manifest-driven hub (desktop and mobile viewport).");
} finally {
  await browser.close();
}

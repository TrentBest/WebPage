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
  const moniker = page.locator(".workshop-running-hub .living-experience-panel .hello-moniker");
  await moniker.waitFor({ state: "visible", timeout: 180_000 });
  const monikerNode = await moniker.elementHandle();
  if (!monikerNode) {
    throw new Error("The landing moniker must exist before hub arrival.");
  }
  await page.screenshot({ path: `${outputDir}/02-moniker.png`, fullPage: true });

  const hub = page.locator(".manifest-hub");
  await hub.waitFor({ state: "visible", timeout: 240_000 });
  if (await page.locator(".manifest-hub-tabs").count() !== 0) {
    throw new Error("The primary canvas must not add a second tab strip; navigation belongs to the left rail.");
  }
  if (!(await monikerNode.evaluate(el => el.isConnected))) {
    throw new Error("Hub arrival must not unmount or replace the living moniker.");
  }
  await moniker.waitFor({ state: "visible", timeout: 10_000 });
  const hubHeight = await hub.evaluate(el => el.scrollHeight);
  const hubVisibleHeight = await hub.evaluate(el => el.clientHeight);
  if (hubHeight > hubVisibleHeight + 2) {
    throw new Error(`The manifested hub must fit its viewport without scrolling (scrollHeight=${hubHeight}, clientHeight=${hubVisibleHeight}).`);
  }
  await page.screenshot({ path: `${outputDir}/03-hub-moniker.png`, fullPage: true });

  const initialLinks = (await page.locator(".workshop-nav-links a").allTextContents()).map(label => label.trim());
  const expectedLinks = ["Welcome", "Demonstrations", "Movement Lab", "Replay Lab", "Economy", "Digitens", "Singularity Architecture", "ProtocolAi Terminal"];
  if (initialLinks.length !== expectedLinks.length || expectedLinks.some((label, i) => initialLinks[i] !== label)) {
    throw new Error(`The hub navigation must expose the expected core destinations plus manifest-declared gallery entries; found: ${initialLinks.join(", ")}.`);
  }

  await page.getByRole("link", { name: "Welcome", exact: true }).click();
  await page.locator(".workshop-nav-shell.collapsed").waitFor({ state: "visible" });
  if (await page.locator(".workshop-nav-links").count() !== 0) {
    throw new Error("Selecting Welcome must retract the navigation to the expander only.");
  }
  await page.locator(".workshop-running-surface.destination-selected .living-experience-panel > .first-contact-stage").waitFor({ state: "hidden" });
  const sidebarWidth = await page.locator(".workshop-sidebar").evaluate(el => el.getBoundingClientRect().width);
  if (sidebarWidth > 60) {
    throw new Error(`The collapsed navigation should leave only the expander rail; width was ${sidebarWidth}px.`);
  }

  await page.getByRole("heading", { name: "Make a system. Change its state. Watch it respond." }).waitFor({ state: "visible" });
  await page.getByRole("button", { name: "SEND START SIGNAL" }).click();
  // FSM_API advances the current-state name on one tick; the new state's OnEnter
  // callback occurs on the next tick. Assert state first, then verify actual lifecycle callbacks.
  await page.getByText("Running", { exact: true }).first().waitFor({ state: "visible" });
  const tick = page.getByRole("button", { name: "TICK RUNTIME" });
  await tick.click(); // enters Running and executes its first update
  await page.getByText("OnEnter → Running", { exact: false }).waitFor({ state: "visible" });
  await tick.click();
  await tick.click(); // third Running update requests Complete
  await tick.click(); // enters Complete on the following update
  await page.getByText("OnEnter → Complete", { exact: false }).waitFor({ state: "visible" });

  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "About Us", exact: true }).waitFor({ state: "visible" });
  const expandedLinks = (await page.locator(".workshop-nav-links a").allTextContents()).map(label => label.trim());
  if (expandedLinks.some(label => ["Ecosystem", "Schedule Consultation", "Institutions"].includes(label))) {
    throw new Error("Removed destination tabs must not reappear in the hub; manifest-declared gallery entries remain valid.");
  }
  await page.screenshot({ path: `${outputDir}/03-welcome.png`, fullPage: true });

  // Exercise the additional destinations after the original Welcome/FSM journey.
  // That keeps the pre-existing state-transition proof independent of iframe navigation.
  await page.getByRole("link", { name: "Digitens", exact: true }).click();
  const digitens = page.frameLocator('iframe[title="Digitens: digital citizens of the Singularity World"]');
  await digitens.getByRole("heading", { name: "DIGITENS", exact: true }).waitFor({ state: "visible", timeout: 15_000 });
  await digitens.getByRole("heading", { name: "The Workshop stays at the heart of the city.", exact: true }).waitFor({ state: "visible", timeout: 15_000 });

  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "Economy", exact: true }).click();
  const casino = page.frameLocator('iframe[title="Singularity Casino living economy experience"]');
  await casino.getByRole("heading", { name: "SINGULARITY CASINO", exact: true }).waitFor({ state: "visible", timeout: 15_000 });

  // The new destination must work through the existing Workshop navigation, not just by direct URL.
  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "Movement Lab", exact: true }).click();
  await page.getByRole("heading", { name: "Movement Lab", exact: true }).waitFor({ state: "visible", timeout: 15_000 });
  const hurdle = page.getByRole("button", { name: /hurdle-01/i });
  await hurdle.focus();
  await page.keyboard.press("Enter");
  await page.getByText("(3, 0.5, 0)", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  await page.getByText("(0.2, 1, 2)", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });

  // Replay Lab is intentionally a local presentation fixture, but its controls must work.
  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "Replay Lab", exact: true }).click();
  await page.getByRole("heading", { name: "Replay Lab", exact: true }).waitFor({ state: "visible", timeout: 15_000 });
  await page.getByText("ILLUSTRATIVE DEMO DATA", { exact: true }).waitFor({ state: "visible" });
  const scrubber = page.locator("#replay-scrubber");
  await scrubber.evaluate(el => {
    el.value = "51";
    el.dispatchEvent(new Event("input", { bubbles: true }));
  });
  await page.getByText("051", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  await page.getByText("Illustrative state marker", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  await page.getByRole("button", { name: /play timeline/i }).click();
  await page.getByRole("button", { name: /pause/i }).waitFor({ state: "visible", timeout: 5_000 });
  await page.waitForFunction(() => Number(document.querySelector(".tick-readout strong")?.textContent) > 51, null, { timeout: 5_000 });
  await page.getByRole("button", { name: /pause/i }).click();
  await page.getByText(/PAUSED — ILLUSTRATIVE FIXTURE ONLY/).waitFor({ state: "visible", timeout: 5_000 });

  await page.setViewportSize({ width: 390, height: 844 });
  await page.getByRole("heading", { name: "Replay Lab", exact: true }).waitFor({ state: "visible" });
  const mobileScrubberWidth = await scrubber.evaluate(el => el.getBoundingClientRect().width);
  if (mobileScrubberWidth < 120) {
    throw new Error(`Replay Lab scrubber should remain usable at a narrow viewport; width was ${mobileScrubberWidth}px.`);
  }
  await page.screenshot({ path: `${outputDir}/04-replay-lab-mobile.png`, fullPage: true });

  if (browserErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected:\n${browserErrors.join("\n")}`);
  }

  console.log("Browser journey passed: gateway -> moniker -> manifest-driven hub (desktop and mobile viewport).");
} finally {
  await browser.close();
}

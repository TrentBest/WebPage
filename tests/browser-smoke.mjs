import { chromium } from "playwright";

const baseUrl = process.env.WORKSHOP_BASE_URL ?? "http://127.0.0.1:5260";
const outputDir = process.env.WORKSHOP_SCREENSHOT_DIR ?? "TestResults/Browser";
const browserErrors = [];

const browser = await chromium.launch({ headless: true });
try {
  const page = await browser.newPage({ viewport: { width: 1440, height: 900 }, deviceScaleFactor: 1 });
  page.on("pageerror", error => browserErrors.push(error.message));

  await page.goto(baseUrl, { waitUntil: "domcontentloaded", timeout: 30_000 });
  await page.getByRole("link", { name: "Return to The Singularity Workshop home" }).waitFor({ state: "visible", timeout: 15_000 });
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
  const expectedLinks = ["Welcome", "Economy", "Digitens"];
  if (initialLinks.length !== expectedLinks.length || expectedLinks.some((label, i) => initialLinks[i] !== label)) {
    throw new Error(`The hub destinations must be Welcome, Economy, and Digitens; found: ${initialLinks.join(", ")}.`);
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
  if (expandedLinks.some(label => ["Ecosystem", "Schedule Consultation", "Institutions", "Demonstrations"].includes(label))) {
    throw new Error("Removed destination tabs must not reappear in the hub.");
  }
  await page.screenshot({ path: `${outputDir}/03-welcome.png`, fullPage: true });

  // Exercise the additional destinations after the original Welcome/FSM journey.
  // That keeps the pre-existing state-transition proof independent of iframe navigation.
  await page.getByRole("link", { name: "Digitens", exact: true }).click();
  const digitens = page.frameLocator('iframe[title="Digitens: digital citizens of the Singularity World"]');
  await digitens.getByRole("heading", { name: "DIGITENS", exact: true }).waitFor({ state: "visible", timeout: 15_000 });
  await digitens.getByRole("link", { name: "Return to The Singularity Workshop home" }).waitFor({ state: "visible", timeout: 15_000 });
  await digitens.getByRole("heading", { name: "The Workshop stays at the heart of the city.", exact: true }).waitFor({ state: "visible", timeout: 15_000 });

  // Verify the real Visualize proof asset and the city model view, not only the route shell.
  await digitens.getByRole("button", { name: /Renderer & detail/i }).click();
  await digitens.getByText(/REAL VISUALIZE OUTPUT/).waitFor({ state: "visible", timeout: 15_000 });
  const proofLoaded = await digitens.locator(".visualize-proof img").evaluate(image =>
    image instanceof HTMLImageElement && image.complete && image.naturalWidth > 0);
  if (!proofLoaded) {
    throw new Error("The Visualize proof image did not load in the Digitens renderer topic.");
  }
  await page.screenshot({ path: `${outputDir}/04-digitens-renderer.png`, fullPage: true });

  await digitens.getByRole("button", { name: /Singularity City/i }).click();
  await digitens.getByText("MAYOR'S MANSION", { exact: true }).waitFor({ state: "visible", timeout: 15_000 });
  await digitens.getByText(/LABELS COME FROM THE EXISTING C# CITY MODEL/).waitFor({ state: "visible", timeout: 15_000 });
  await page.screenshot({ path: `${outputDir}/05-singularity-city.png`, fullPage: true });

  // The first company-building step is a real local draft, not a fake city write.
  await digitens.getByRole("button", { name: /Start building/i }).click();
  await digitens.getByLabel("Company name").fill("Aurora Foundry");
  await digitens.getByLabel("What will it do?").fill("Builds civic rendering tools for Singularity City.");
  await digitens.getByLabel("Office footprint").selectOption({ label: "Team floor" });
  await digitens.getByRole("button", { name: "Prepare blueprint", exact: true }).click();
  await digitens.getByRole("heading", { name: "Aurora Foundry", exact: true }).waitFor({ state: "visible", timeout: 15_000 });
  await digitens.getByText(/LOCAL DRAFT ONLY/).waitFor({ state: "visible", timeout: 15_000 });
  await page.screenshot({ path: `${outputDir}/06-company-blueprint.png`, fullPage: true });
  await digitens.getByRole("button", { name: /Explore Singularity City/i }).click();
  await digitens.getByText(/LABELS COME FROM THE EXISTING C# CITY MODEL/).waitFor({ state: "visible", timeout: 15_000 });

  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "Economy", exact: true }).click();
  const casino = page.frameLocator('iframe[title="Singularity Casino living economy experience"]');
  await casino.getByRole("heading", { name: "SINGULARITY CASINO", exact: true }).waitFor({ state: "visible", timeout: 15_000 });

  // The City Lab is a separate, navigable renderer test: all three skins must reuse one geometry model.
  const cityLab = await browser.newPage({ viewport: { width: 1440, height: 1000 }, deviceScaleFactor: 1 });
  cityLab.on("pageerror", error => browserErrors.push(error.message));
  await cityLab.goto(`${baseUrl}/city-lab`, { waitUntil: "domcontentloaded", timeout: 30_000 });
  await cityLab.getByRole("heading", { name: "Singularity City Lab" }).waitFor({ state: "visible", timeout: 30_000 });
  await cityLab.locator("#density-profile").selectOption("bronx");
  await cityLab.getByText("13,483 residents/km²", { exact: true }).waitFor({ state: "visible" });
  await cityLab.getByText("1,472,654 residents", { exact: true }).waitFor({ state: "visible" });
  await cityLab.locator("#density-profile").selectOption("manhattan");
  await cityLab.getByText("28,874 residents/km²", { exact: true }).waitFor({ state: "visible" });
  const structureCount = await cityLab.locator(".readout-card").first().locator("strong").innerText();
  await cityLab.getByRole("button", { name: /02 \/ METRO/ }).click();
  await cityLab.locator('[data-skin="metro"]').waitFor({ state: "visible" });
  if (await cityLab.locator(".readout-card").first().locator("strong").innerText() !== structureCount) {
    throw new Error("Switching to Metro must preserve the exact same city geometry/data.");
  }
  await cityLab.getByRole("button", { name: /03 \/ HARMONY/ }).click();
  await cityLab.locator('[data-skin="solarpunk"]').waitFor({ state: "visible" });
  if (await cityLab.locator(".readout-card").first().locator("strong").innerText() !== structureCount) {
    throw new Error("Switching to Harmony must preserve the exact same city geometry/data.");
  }
  await cityLab.locator("#density-input").focus();
  await cityLab.locator("#density-input").press("End");
  await cityLab.getByText("50,000 residents/km²", { exact: true }).waitFor({ state: "visible" });
  if (await cityLab.locator("#density-profile").inputValue() !== "custom") {
    throw new Error("Moving the density slider must switch the profile selector to Custom.");
  }
  const denseStructureCount = await cityLab.locator(".readout-card").first().locator("strong").innerText();
  if (Number(denseStructureCount.replaceAll(",", "")) <= Number(structureCount.replaceAll(",", ""))) {
    throw new Error("Increasing density must generate more structures in the deterministic city model.");
  }
  await cityLab.screenshot({ path: `${outputDir}/07-city-lab-harmony.png`, fullPage: true });
  await cityLab.close();

  if (browserErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected:\n${browserErrors.join("\n")}`);
  }

  console.log("Browser journey passed: gateway -> moniker -> manifest-driven hub (desktop and mobile viewport).");
} finally {
  await browser.close();
}

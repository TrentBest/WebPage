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
  const expectedLinks = ["Welcome", "Demonstrations", "Movement Lab", "Replay Lab", "Economy", "Digitens", "Singularity Architecture", "ProtocolAi Terminal", "About Us"];
  if (initialLinks.length !== expectedLinks.length || expectedLinks.some((label, i) => initialLinks[i] !== label)) {
    throw new Error(`The hub navigation must expose core destinations plus manifest-declared gallery entries; found: ${initialLinks.join(", ")}.`);
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
  await page.getByRole("button", { name: "REVEAL THE FSM" }).click();
  await page.getByRole("button", { name: "SEND START SIGNAL" }).waitFor({ state: "visible" });
  await page.getByRole("button", { name: "SEND START SIGNAL" }).click();
  // FSM_API advances the current-state name on one tick; the new state's OnEnter
  // callback occurs on the next tick. Assert state first, then verify actual lifecycle callbacks.
  await page.getByText("Running", { exact: true }).first().waitFor({ state: "visible" });
  const tick = page.getByRole("button", { name: "TICK RUNTIME" });
  await tick.click(); // first Running update
  await page.getByText("OnEnter → Running", { exact: false }).waitFor({ state: "visible" });
  await tick.click(); // second Running update requests Complete
  await tick.click(); // enters Complete on the following update
  await page.getByText("OnEnter → Complete", { exact: false }).waitFor({ state: "visible" });

  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "About Us", exact: true }).waitFor({ state: "visible" });
  const expandedLinks = (await page.locator(".workshop-nav-links a").allTextContents()).map(label => label.trim());
  if (expandedLinks.some(label => ["Ecosystem", "Schedule Consultation", "Institutions"].includes(label))) {
    throw new Error("Removed destination tabs must not reappear in the hub.");
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

  // Movement Lab exposes geometry inspection without claiming a simulated attempt.
  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "Movement Lab", exact: true }).click();
  await page.getByRole("heading", { name: "Movement Lab", exact: true }).waitFor({ state: "visible", timeout: 15_000 });
  const hurdle = page.getByRole("button", { name: /hurdle-01/i });
  await hurdle.focus();
  await page.keyboard.press("Enter");
  await page.getByText("(3, 0.5, 0)", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  await page.getByText("(0.2, 1, 2)", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  const schematic = await page.locator(".course-svg").evaluate(svg => ({
    viewBox: svg.getAttribute("viewBox"),
    approach: ["x", "y", "width", "height"].map(name => svg.querySelector(".approach-zone").getAttribute(name)),
    hurdle: ["x", "y", "width", "height"].map(name => svg.querySelector(".hurdle-zone").getAttribute(name)),
    landing: ["x", "y", "width", "height"].map(name => svg.querySelector(".landing-zone").getAttribute(name)),
    description: svg.querySelector("#course-desc")?.textContent ?? ""
  }));
  const expectedSchematic = {
    viewBox: "0 0 760 300",
    approach: ["100", "175", "180", "90"],
    hurdle: ["451", "130", "18", "90"],
    landing: ["550", "211", "180", "18"]
  };
  for (const key of Object.keys(expectedSchematic)) {
    if (JSON.stringify(schematic[key]) !== JSON.stringify(expectedSchematic[key])) {
      throw new Error(`Movement Lab schematic geometry changed unexpectedly for ${key}: ${JSON.stringify(schematic[key])}`);
    }
  }
  if (!schematic.description.includes("same 90 SVG units per meter on both axes")) {
    throw new Error("Movement Lab must disclose its shared schematic scale and authored-bound representation.");
  }

  // Check the actual Movement Lab layout at a narrow viewport, not just the desktop render.
  await page.setViewportSize({ width: 390, height: 844 });
  await page.waitForTimeout(100);
  const mobileLayout = await page.evaluate(() => ({
    viewportWidth: window.innerWidth,
    documentWidth: document.documentElement.scrollWidth,
    courseColumns: getComputedStyle(document.querySelector(".course-layout")).gridTemplateColumns,
    evidenceColumns: getComputedStyle(document.querySelector(".evidence-layout")).gridTemplateColumns,
    selectedElementVisible: (() => {
      const button = document.querySelector(".element-button.selected");
      if (!button) return false;
      const rect = button.getBoundingClientRect();
      return rect.width > 0 && rect.left >= 0 && rect.right <= window.innerWidth;
    })()
  }));
  if (mobileLayout.documentWidth > mobileLayout.viewportWidth + 1) {
    throw new Error(`Movement Lab must not create horizontal page overflow on mobile: ${JSON.stringify(mobileLayout)}`);
  }
  if (!mobileLayout.selectedElementVisible) {
    throw new Error(`Movement Lab selected region must remain visible on mobile: ${JSON.stringify(mobileLayout)}`);
  }
  if (mobileLayout.courseColumns.split(" ").length !== 1 || mobileLayout.evidenceColumns.split(" ").length !== 1) {
    throw new Error(`Movement Lab course and evidence panels must stack on mobile: ${JSON.stringify(mobileLayout)}`);
  }
  await page.screenshot({ path: `${outputDir}/04-movement-lab-mobile.png`, fullPage: true });
  await page.setViewportSize({ width: 1440, height: 900 });

  // Replay Lab uses the current development fixture: slider, event ledger, and transport.
  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "Replay Lab", exact: true }).click();
  await page.getByRole("heading", { name: "Replay Lab", exact: true }).waitFor({ state: "visible", timeout: 15_000 });
  await page.getByText("DEMO DATA — NOT YET CONNECTED TO PHYSICS WORLD REPLAY", { exact: true }).waitFor({ state: "visible" });
  const scrubber = page.locator(".timeline-slider");
  await scrubber.evaluate(el => {
    el.value = "51";
    el.dispatchEvent(new Event("input", { bubbles: true }));
  });
  await page.getByText("051", { exact: true }).waitFor({ state: "visible", timeout: 10_000 });
  await page.getByText("The illustrative path bends toward the orbital field.", { exact: true }).first().waitFor({ state: "visible", timeout: 10_000 });
  await page.getByRole("button", { name: /play/i }).click();
  await page.getByRole("button", { name: /pause/i }).waitFor({ state: "visible", timeout: 5_000 });
  await page.waitForFunction(() => Number(document.querySelector(".tick-readout strong")?.textContent) > 51, null, { timeout: 5_000 });
  await page.getByRole("button", { name: /pause/i }).click();
  await page.getByRole("button", { name: /play/i }).waitFor({ state: "visible", timeout: 5_000 });
  await page.setViewportSize({ width: 390, height: 844 });
  const mobileScrubberWidth = await scrubber.evaluate(el => el.getBoundingClientRect().width);
  if (mobileScrubberWidth < 120) {
    throw new Error(`Replay Lab scrubber should remain usable at a narrow viewport; width was ${mobileScrubberWidth}px.`);
  }
  await page.screenshot({ path: `${outputDir}/04-replay-lab-mobile.png`, fullPage: true });

  // Verify the user's human-mediated clipboard workflow; this terminal never calls an LLM.
  await page.setViewportSize({ width: 1440, height: 900 });
  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "ProtocolAi Terminal", exact: true }).click();
  await page.locator(".ai-exchange-grid .ai-exchange-block").nth(0).waitFor({ state: "visible", timeout: 15_000 });
  await page.locator(".ai-exchange-grid .ai-exchange-block").nth(1).waitFor({ state: "visible", timeout: 15_000 });
  const responsePreview = page.locator("#ai-llm-response");
  await responsePreview.waitFor({ state: "visible", timeout: 10_000 });
  if (await responsePreview.isEditable()) {
    throw new Error("ProtocolAi response preview must not be directly editable.");
  }
  await page.context().grantPermissions(["clipboard-read", "clipboard-write"]);
  await page.getByRole("button", { name: "Copy to Clipboard", exact: true }).click();
  const copiedExchange = await page.evaluate(() => navigator.clipboard.readText());
  if (!copiedExchange.includes("WORKSHOP PROTOCOLAI TERMINAL EXCHANGE") ||
      !copiedExchange.includes("PROTOCOL") || !copiedExchange.includes("GRAMMAR")) {
    throw new Error("ProtocolAi Copy to Clipboard must carry the composed exchange context.");
  }
  await page.evaluate(() => navigator.clipboard.writeText("browser smoke response fixture"));
  await page.getByRole("button", { name: "Paste from Clipboard", exact: true }).click();
  await page.waitForFunction(() => document.querySelector("#ai-llm-response")?.value === "browser smoke response fixture", null, { timeout: 10_000 });
  await page.getByText(/NO HOST PROCESSOR ATTACHED; NO ACTUATION OCCURRED/).waitFor({ state: "visible", timeout: 10_000 });

  if (browserErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected:\n${browserErrors.join("\n")}`);
  }

  console.log("Browser journey passed: gateway/moniker/hub, FSM lifecycle, Movement Lab keyboard inspection, Replay Lab transport/mobile layout, and ProtocolAi clipboard preview.");
} finally {
  await browser.close();
}

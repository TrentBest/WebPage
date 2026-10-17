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
  const expectedLinks = ["Welcome", "About Us"];
  if (initialLinks.length !== expectedLinks.length || expectedLinks.some((label, i) => initialLinks[i] !== label)) {
    throw new Error(`The hub navigation must expose core destinations plus manifest-declared gallery entries; found: ${initialLinks.join(", ")}.`);
  }

  await page.getByRole("link", { name: "Welcome", exact: true }).click();
  await page.locator(".workshop-nav-shell.collapsed").waitFor({ state: "visible" });
  if (await page.locator(".workshop-nav-links").count() !== 0) {
    throw new Error("Selecting Welcome must retract the navigation to the expander only.");
  }
  await page.locator(".workshop-running-surface.destination-selected .living-experience-panel > .first-contact-stage").waitFor({ state: "hidden" });
  // The rail deliberately animates between expanded and collapsed widths. Wait for
  // the real layout to reach its target before asserting the final geometry; sampling
  // mid-transition makes this smoke test intermittently fail at fractional widths.
  await page.waitForFunction(() => {
    const sidebar = document.querySelector(".workshop-sidebar");
    return sidebar && sidebar.getBoundingClientRect().width <= 60;
  }, null, { timeout: 5_000 });
  const sidebarWidth = await page.locator(".workshop-sidebar").evaluate(el => el.getBoundingClientRect().width);
  if (sidebarWidth > 60) {
    throw new Error(`The collapsed navigation should leave only the expander rail; width was ${sidebarWidth}px.`);
  }

  await page.getByRole("heading", { name: "Welcome, friend." }).waitFor({ state: "visible" });
  await page.locator(".workshop-map").waitFor({ state: "visible" });
  const floorplanVoice = page.getByRole("button", { name: "VOICE ON", exact: true });
  await floorplanVoice.waitFor({ state: "visible" });
  await page.getByRole("button", { name: "Observatory: assemble solar systems", exact: true }).hover();
  await page.locator(".inspector-copy h2").getByText("The Observatory", { exact: true }).waitFor({ state: "visible" });
  await page.getByRole("button", { name: "Forge: construct MicroBundles and Experiences", exact: true }).hover();
  await page.locator(".inspector-copy h2").getByText("The Forge", { exact: true }).waitFor({ state: "visible" });
  await page.getByText(/general MicroBundle\/Experience authoring tool is not implemented yet/i).waitFor({ state: "visible" });
  await page.getByRole("button", { name: "Laboratory: live FSM_API demonstration station", exact: true }).hover();
  await page.locator(".inspector-copy h2").getByText("The Singularity Laboratory", { exact: true }).waitFor({ state: "visible" });
  await floorplanVoice.click(); // Mute browser speech so the remaining browser journey is quiet.
  await page.getByRole("button", { name: "VOICE OFF", exact: true }).waitFor({ state: "visible" });
  const floorplanDimensions = await page.evaluate(() => ({
    viewport: window.innerHeight,
    document: document.documentElement.scrollHeight
  }));
  if (floorplanDimensions.document > floorplanDimensions.viewport + 2) {
    throw new Error("The Welcome floor plan must fit the viewport without page scrolling: " + JSON.stringify(floorplanDimensions));
  }
  await page.getByRole("button", { name: "ENTER FSM_API STATION ↗" }).click();
  await page.getByRole("heading", { name: "Make a system. Change its state. Watch it respond." }).waitFor({ state: "visible" });
  await page.getByRole("button", { name: "REVEAL THE FSM" }).click();
  await page.getByRole("button", { name: "SEND START SIGNAL" }).waitFor({ state: "visible" });
  await page.getByRole("button", { name: "SEND START SIGNAL" }).click();
  // FSM_API advances the current-state name on one tick; the new state's OnEnter
  // callback occurs on the next tick. Assert state first, then verify actual lifecycle callbacks.
  // Each Welcome lab stamps its state and controls with the same instance identity.
  // Use that identity directly instead of guessing component boundaries in generated HTML.
  const welcomeLabHandle = await page.waitForFunction(() => {
    const isActuallyVisible = element => {
      for (let node = element; node instanceof HTMLElement; node = node.parentElement) {
        const style = getComputedStyle(node);
        if (style.display === "none" || style.visibility === "hidden" || Number(style.opacity) === 0)
          return false;
      }
      return element.getClientRects().length > 0;
    };
    for (const state of document.querySelectorAll(".welcome-fsm-current-state")) {
      if (state.textContent?.trim() !== "Running" || !isActuallyVisible(state))
        continue;
      const labId = state.getAttribute("data-welcome-lab");
      if (!labId)
        continue;
      const tick = [...document.querySelectorAll("button.welcome-fsm-action")]
        .find(button => button.getAttribute("data-welcome-lab") === labId
          && button.textContent?.includes("TICK RUNTIME"));
      if (tick && isActuallyVisible(tick) && !tick.disabled)
        return labId;
    }
    return false;
  }, null, { timeout: 15_000 });
  const welcomeLabId = await welcomeLabHandle.jsonValue();
  const tick = page.locator(`button.welcome-fsm-action[data-welcome-lab="${welcomeLabId}"]`)
    .filter({ hasText: "TICK RUNTIME" });
  await tick.click(); // enters Running and delivers its OnEnter callback
  await page.getByText("OnEnter → Running", { exact: false }).waitFor({ state: "visible" });
  await tick.click(); // first Running update
  await tick.click(); // second Running update selects Complete
  await tick.click(); // deliver OnEnter after the terminal state has been selected
  await page.getByText("OnEnter → Complete", { exact: false }).waitFor({ state: "visible" });

  await page.getByRole("button", { name: "Close the Laboratory station", exact: true }).click();
  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "About Us", exact: true }).waitFor({ state: "visible" });
  const expandedLinks = (await page.locator(".workshop-nav-links a").allTextContents()).map(label => label.trim());
  if (expandedLinks.length !== 2 || expandedLinks[0] !== "Welcome" || expandedLinks[1] !== "About Us") {
    throw new Error("The first hub view should expose only Welcome and the always-available About Us route; found: " + expandedLinks.join(", ") + ".");
  }
  await page.screenshot({ path: outputDir + "/03-welcome.png", fullPage: true });

  // About Us remains directly addressable for existing external and Unity package links.
  await page.goto(baseUrl + "/about-us", { waitUntil: "domcontentloaded" });
  await page.getByText("We are not building one giant application.", { exact: true }).waitFor({ state: "visible", timeout: 15000 });

  // Direct URLs remain available; entering a deeper experience may reveal related destinations.
  await page.goto(baseUrl + "/digitens", { waitUntil: "networkidle" });
  await page.getByRole("heading", { name: "DIGITENS", exact: true }).waitFor({ state: "visible", timeout: 15000 });
  await page.getByRole("button", { name: /expand workshop navigation/i }).click();
  await page.getByRole("link", { name: "Digitens", exact: true }).waitFor({ state: "visible" });
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

  // Singularity Studios must expose a downloadable, versioned stage-data artifact.
  // The cameras share one scene identity and repeated props reference one canonical asset definition.
  await page.goto(`${baseUrl}/singularity-studios`, { waitUntil: "domcontentloaded", timeout: 30_000 });
  await page.locator(".studios-header h1").waitFor({ state: "visible", timeout: 15_000 });
  const stageDataLink = page.getByRole("link", { name: /inspect \/ download the versioned stage data/i });
  await stageDataLink.waitFor({ state: "visible", timeout: 10_000 });
  await page.locator(".studios-audio audio[controls]").waitFor({ state: "visible", timeout: 10_000 });
  const stageDataResponse = await page.request.get(`${baseUrl}/Workshop/Forge/StreamingAssets/Experiences/singularity-studios-stage.json`);
  if (!stageDataResponse.ok()) {
    throw new Error(`Singularity Studios stage data must be served by the WebPage host; HTTP ${stageDataResponse.status()}.`);
  }
  const stageData = await stageDataResponse.json();
  const planImage = page.locator(".studios-stage-plan");
  await planImage.waitFor({ state: "visible", timeout: 15_000 });
  const planResponse = await page.request.get(`${baseUrl}/Workshop/Forge/StreamingAssets/Experiences/singularity-studios-stage-plan.svg`);
  if (!planResponse.ok()) {
    throw new Error(`The Visualize-generated studio plan must be served by WebPage; HTTP ${planResponse.status()}.`);
  }
  const planSvg = await planResponse.text();
  const visualizerGeneratedPlan = planSvg.includes("Singularity Studios production plan") && planSvg.includes("draw-");
  const staticFallbackPlan = planSvg.includes('data-render-status="static-fallback"') &&
    planSvg.includes("SOURCE-INTEGRATION BUILDS REGENERATE THROUGH VISUALIZE");
  if (!planSvg.includes("<svg") || (!visualizerGeneratedPlan && !staticFallbackPlan)) {
    throw new Error("The studio plan must be either the explicitly labelled static fallback or the Visualize-generated geometry/frame/SVG output.");
  }
  if (stageData.coordinateFrame?.handedness !== "right-handed" ||
      stageData.coordinateFrame.axes.x !== "stage-width" ||
      stageData.coordinateFrame.axes.y !== "elevation" ||
      stageData.coordinateFrame.axes.z !== "stage-depth" ||
      stageData.coordinateFrame.planProjection.verticalAxis !== "z") {
    throw new Error("Singularity Studios must declare its world coordinate frame and plan projection explicitly.");
  }
  const propDefinition = stageData.assetDefinitions.find(asset => asset.id === "prop.service-crate");
  const propPlacements = stageData.scene.instances.filter(instance => instance.assetId === "prop.service-crate");
  if (stageData.schemaVersion !== "0.1.0" || !propDefinition || propPlacements.length !== 2) {
    throw new Error("Singularity Studios must preserve one versioned prop definition referenced by multiple scene placements.");
  }
  if (stageData.cameras.length !== 2 || stageData.cameras.some(camera => camera.sceneId !== stageData.scene.id)) {
    throw new Error("Singularity Studios cameras must observe the same canonical scene instead of duplicating world state.");
  }
  if (!propDefinition.planFootprint || propPlacements.some(instance => !instance.transform.position)) {
    throw new Error("Studio plan rendering must use explicit illustrative footprints and stable scene-instance transforms.");
  }
  if (stageData.stage.dimensionsStatus !== "illustrative-planning-target" ||
      !stageData.productionNotes.some(note => note.includes("not implemented")) ||
      !stageData.productionNotes.some(note => note.includes("illustrative footprints"))) {
    throw new Error("Singularity Studios must label planning dimensions and unsupported production capabilities honestly.");
  }

  if (browserErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected:\n${browserErrors.join("\n")}`);
  }

  console.log("Browser journey passed: gateway/moniker/hub, Welcome floorplan with distinct Laboratory and Forge roles, FSM lifecycle station, Movement Lab keyboard inspection, Replay Lab transport/mobile layout, ProtocolAi clipboard preview, and Singularity Studios Visualize-generated plan and reusable stage-data proof.");
} finally {
  await browser.close();
}

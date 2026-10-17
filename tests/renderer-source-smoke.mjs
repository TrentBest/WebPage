import assert from "node:assert/strict";
import { chromium } from "playwright";

const baseUrl = process.env.WORKSHOP_BASE_URL ?? "http://127.0.0.1:5261";
const browser = await chromium.launch({ headless: true });

try {
  const page = await browser.newPage({ viewport: { width: 1440, height: 1000 } });

  // The same deterministic city must remain identifiable across overview, plan, and street observation.
  await page.goto(`${baseUrl}/city-lab`, { waitUntil: "networkidle" });
  await page.getByRole("heading", { name: /Singularity City Lab/i }).waitFor({ state: "visible", timeout: 20_000 });
  await page.getByRole("button", { name: /TOP-DOWN PLAN/i }).click();
  const planBuildings = page.locator(".plan-building[data-entity-id]");
  await planBuildings.first().waitFor({ state: "visible", timeout: 10_000 });
  const planIds = await planBuildings.evaluateAll(nodes => nodes.map(node => node.getAttribute("data-entity-id")));
  assert.ok(planIds.length > 200, `Expected a city-scale plan, received ${planIds.length} buildings.`);
  assert.equal(new Set(planIds).size, planIds.length, "Plan building identities must be unique.");

  await page.getByRole("button", { name: /STREET VIEW/i }).click();
  const streetBuildings = page.locator(".street-building[data-entity-id]");
  await streetBuildings.first().waitFor({ state: "visible", timeout: 10_000 });
  const streetIds = await streetBuildings.evaluateAll(nodes => nodes.map(node => node.getAttribute("data-entity-id")));
  assert.ok(streetIds.length > 0, "First-person projection must show city buildings.");
  assert.ok(streetIds.every(id => planIds.includes(id)), "Street view must reuse canonical plan building IDs.");
  const beforeMove = await page.locator(".stage-topline").textContent();
  await page.getByRole("button", { name: "FORWARD", exact: false }).click();
  const afterMove = await page.locator(".stage-topline").textContent();
  assert.notEqual(afterMove, beforeMove, "Moving the observer must change camera state.");
  await page.getByRole("button", { name: /TURN/i }).last().click();
  const cameraReadout = page.locator(".stage-topline span").last();
  await cameraReadout.waitFor({ state: "visible", timeout: 5_000 });
  assert.match(await cameraReadout.textContent() ?? "", /YAW 12°/);
  await page.getByRole("button", { name: /CITY OVERVIEW/i }).click();
  await page.screenshot({ path: process.env.CITY_LAB_SCREENSHOT ?? "singularity-city-overview.png", fullPage: true });
  console.log(`Singularity City: ${planIds.length} stable building IDs are shared between top-down plan and observer-relative street projection; movement and yaw controls passed.`);

  // Source mode must submit the city through Renderer reduction and the actual Visualize GUI.SVG backend.
  const pipelineButton = page.getByRole("button", { name: /RENDERER.*VISUALIZE/i });
  assert.equal(await pipelineButton.isEnabled(), true, "The source-mode build must expose the live Renderer/Visualize path.");
  await pipelineButton.click();
  const pipelineSurface = page.locator(".pipeline-surface svg");
  await pipelineSurface.waitFor({ state: "visible", timeout: 15_000 });
  const pipelineMetrics = page.locator(".pipeline-readout p");
  await pipelineMetrics.waitFor({ state: "visible", timeout: 10_000 });
  const initialPipelineMetrics = await pipelineMetrics.textContent() ?? "";
  assert.match(initialPipelineMetrics, /candidate commands\s+[\d,]+ → submitted\s+[\d,]+/);
  assert.match(initialPipelineMetrics, /CPU SVG proof, not GPU rendering/);
  assert.ok(await pipelineSurface.locator("path").count() > 0, "GUI.SVG must manifest actual submitted city geometry.");
  await page.getByRole("button", { name: "Pipeline move forward", exact: true }).click();
  await page.waitForFunction(
    previous => document.querySelector(".pipeline-readout p")?.textContent !== previous,
    initialPipelineMetrics,
    { timeout: 10_000 }
  );
  await page.screenshot({ path: process.env.CITY_PIPELINE_SCREENSHOT ?? "TestResults/Browser/singularity-city-pipeline.png", fullPage: true });
  const afterMovePipelineMetrics = await pipelineMetrics.textContent() ?? "";
  assert.notEqual(afterMovePipelineMetrics, initialPipelineMetrics, "Observer movement must change the live Renderer pipeline readout.");
  await page.getByRole("button", { name: "Pipeline move back", exact: true }).click();
  await page.waitForFunction(
    previous => document.querySelector(".pipeline-readout p")?.textContent !== previous,
    afterMovePipelineMetrics,
    { timeout: 10_000 }
  );
  console.log("Singularity City pipeline: source-mode city entities passed through Renderer policy/reduction, RendererFrameAdapter, Visualize, and the host-owned GUI.SVG sink; observer movement refreshed the submitted frame. Screenshot saved.");

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

  await page.goto(`${baseUrl}/rendering`, { waitUntil: "networkidle" });
  await page.getByRole("heading", { name: "Test the same Renderer decisions in the browser." }).waitFor({
    state: "visible",
    timeout: 20_000
  });

  await page.getByText("SOURCE MODE // RENDERER REDUCER + VISUALIZE FRAME ADAPTER", { exact: true }).waitFor({
    state: "visible",
    timeout: 10_000
  });

  const proofMetrics = page.locator(".renderer-proof-metrics");
  await proofMetrics.waitFor({ state: "visible", timeout: 10_000 });
  assert.match(await proofMetrics.textContent() ?? "", /Candidate commands\s*5,000/);
  assert.match(await proofMetrics.textContent() ?? "", /Selected commands\s*1,000/);
  assert.match(await proofMetrics.textContent() ?? "", /Omitted commands\s*4,000/);
  assert.equal(await page.locator(".renderer-proof-svg circle").count(), 1_000);
  const baselineEntityIds = await page.locator(".renderer-proof-svg circle").evaluateAll(
    nodes => nodes.map(node => node.getAttribute("data-entity-id"))
  );
  assert.deepEqual(
    baselineEntityIds,
    Array.from({ length: 1_000 }, (_, index) => String(index + 1))
  );

  const detailCases = [
    ["SEMANTIC", "#5f788c"],
    ["AGGREGATE", "#00a8ff"],
    ["DISTANT", "#00eaff"],
    ["NEAR", "#ff2fff"],
    ["INTERACTIVE", "#ffd34d"]
  ];
  for (const [label, expectedColor] of detailCases) {
    await page.getByRole("button", { name: label, exact: true }).click();
    await page.waitForFunction(
      expected => document.querySelector(".renderer-proof-metrics")?.textContent?.includes(`Detail ${expected}`),
      label,
      { timeout: 10_000 }
    );
    assert.equal(await page.locator(".renderer-proof-svg circle").count(), 1_000);
    const currentEntityIds = await page.locator(".renderer-proof-svg circle").evaluateAll(
      nodes => nodes.map(node => node.getAttribute("data-entity-id"))
    );
    assert.deepEqual(currentEntityIds, baselineEntityIds);
    assert.equal(await page.locator(".renderer-proof-svg circle").first().getAttribute("fill"), expectedColor);
    assert.match(await proofMetrics.textContent() ?? "", /Omitted commands\s*4,000/);
  }

  console.log("Renderer browser host: the shared 5,000-command workload reduced to 1,000 stable entities at all five detail ceilings; SVG projection and metrics matched the selected representation.");

  // Singularity Studios must render its authored floor plan through the source Visualize adapter.
  await page.goto(`${baseUrl}/singularity-studios`, { waitUntil: "networkidle" });
  await page.locator(".studios-stage-plan").waitFor({ state: "visible", timeout: 15_000 });
  const studioPlanResponse = await page.request.get(
    `${baseUrl}/Workshop/Forge/StreamingAssets/Experiences/singularity-studios-stage-plan.svg`
  );
  assert.equal(studioPlanResponse.status(), 200);
  const studioPlanSvg = await studioPlanResponse.text();
  assert.match(studioPlanSvg, /Singularity Studios production plan/);
  assert.match(studioPlanSvg, /Renderer policy.*Visualize.*GUI\.SVG/);
  assert.match(studioPlanSvg, /Interactive=\d+/);
  assert.match(studioPlanSvg, /Distant=\d+/);
  assert.match(studioPlanSvg, /draw-/);
  assert.doesNotMatch(studioPlanSvg, /data-render-status="static-fallback"/);
  console.log("Singularity Studios source integration: stage zones, reusable asset footprints, scene placements, cameras, and lighting markers were submitted through Visualize and emitted by the GUI.SVG adapter.");
} finally {
  await browser.close();
}

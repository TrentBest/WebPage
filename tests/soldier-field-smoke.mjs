import { chromium } from "playwright";

const baseUrl = process.env.WORKSHOP_BASE_URL ?? "http://127.0.0.1:5260";
const browser = await chromium.launch({ headless: true });
try {
  const page = await browser.newPage({ acceptDownloads: true, viewport: { width: 1440, height: 900 } });
  const pageErrors = [];
  page.on("pageerror", error => pageErrors.push(error.message));

  await page.goto(`${baseUrl}/experiences/soldier-field/index.html`, {
    waitUntil: "domcontentloaded",
    timeout: 30_000
  });
  await page.getByRole("heading", { name: "Soldier Field — Event Horizon Scaling Lab" }).waitFor({
    state: "visible",
    timeout: 15_000
  });

  const select = page.locator("#part-select");
  await select.locator("option").nth(37).waitFor({ state: "attached", timeout: 15_000 });
  const optionCount = await select.locator("option").count();
  if (optionCount !== 38) {
    throw new Error(`Expected 38 semantic part records, found ${optionCount}.`);
  }

  await select.selectOption("torso.sternum-plate");
  const fields = page.locator("#part-fields");
  await fields.getByText("torso.sternum-plate", { exact: true }).waitFor({ state: "attached" });
  await fields.getByText("Nominal dimensions (m)", { exact: true }).waitFor({ state: "attached" });
  await page.locator("#part-issues li").first().waitFor({ state: "attached" });

  const downloadPromise = page.waitForEvent("download");
  await page.getByRole("button", { name: "Download derived cut sheet (JSON)" }).click();
  const download = await downloadPromise;
  if (download.suggestedFilename() !== "torso.sternum-plate-cut-sheet.json") {
    throw new Error(`Unexpected cut-sheet filename: ${download.suggestedFilename()}.`);
  }

  if (pageErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected: ${pageErrors.join("\n")}`);
  }
  await page.locator("#demo-select option").nth(2).waitFor({ state: "attached", timeout: 15_000 });
  if (await page.locator("#demo-select option").count() !== 3) {
    throw new Error("Expected three data-driven research demonstrations.");
  }
  await page.locator("#demo-select").selectOption("plate-impact");
  await page.locator("#demo-description").waitFor({ state: "visible" });
  if (!(await page.locator("#demo-description").textContent()).includes("Current selection basis: most-hit candidate")) {
    throw new Error("The tablet did not explain the most-hit plate selection premise.");
  }
  if (await page.locator("#demo-cost").textContent() !== "120 cr") {
    throw new Error("Plate-impact demonstration did not show its catalog price.");
  }
  const requestButtonHitTest = await page.locator("#request-demo").evaluate(el => {
    const rect = el.getBoundingClientRect();
    const top = document.elementFromPoint(rect.left + rect.width / 2, rect.top + rect.height / 2);
    return { tag: top?.tagName, id: top?.id, className: typeof top?.className === "string" ? top.className : "", html: top?.outerHTML?.slice(0, 240) };
  });
  console.log("Research order button hit-test:", JSON.stringify(requestButtonHitTest));
  await page.locator("#request-demo").click();
  await page.locator("#complete-setup").click();
  await page.locator("#fire-demo").click();
  await page.locator("#research-log").getByText("PROTOTYPE DISCHARGE.", { exact: false }).waitFor({ state: "attached" });
  await page.getByText("Head scientist: “As you can see, sire, we still struggle with dissipating the heat…”", { exact: true }).waitFor({ state: "attached" });
  await page.locator("#invest-cooling").click();
  await page.locator("#demo-select").selectOption("plate-impact");
  await page.locator("#request-demo").click();
  await page.locator("#complete-setup").click();
  await page.locator("#fire-demo").click();
  await page.locator("#research-log").getByText("Nearby trooper, dryly:", { exact: false }).waitFor({ state: "attached" });
  if (pageErrors.length > 0) {
    throw new Error(`Browser JavaScript errors detected: ${pageErrors.join("\n")}`);
  }
  console.log("Soldier Field smoke passed: 38 part records, cut-sheet download, priced demonstration setup, impact/heat response, cooling investment, and follow-on trooper chatter.");
} finally {
  await browser.close();
}

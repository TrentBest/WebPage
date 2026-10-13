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
  console.log("Soldier Field smoke passed: 38 records loaded, a part selected, and a derived cut sheet downloaded.");
} finally {
  await browser.close();
}

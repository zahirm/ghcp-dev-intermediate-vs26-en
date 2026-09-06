# GH-300 Copilot Demo Kit — .NET 8 edition

A compact, self-contained kit for demonstrating GitHub Copilot capabilities during
GH-300 preparation.

- Ported to **.NET 8 (C#)** and tuned for **Visual Studio 2026**.
- Also works in **VS Code** with the C# Dev Kit.
- The two source modules are **intentionally imperfect** — an off-by-one bug, a logged
  card number, SQL injection, no docs, and (almost) no tests — so that Copilot has real
  work to do during `/fix`, `/tests`, review, refactor, and modernization demos.

---

## 📁 Solution layout

```
CopilotDemoKit.sln
├─ src/
│  ├─ ShoppingCart/        ShoppingCart\Cart.cs            (port of cart.js)
│  ├─ InventoryLegacy/     InventoryLegacy\InventoryLegacy.cs (port of inventory_legacy.py)
│  └─ DemoApp/             Program.cs — driver that exercises both modules
├─ tests/
│  └─ ShoppingCart.Tests/  intentionally minimal — the "no tests" gap for the /tests demo
├─ .github/
│  ├─ copilot-instructions.md
│  ├─ instructions/csharp.instructions.md
│  └─ prompts/{security-review,unit-tests}.prompt.md
├─ .vscode/{settings.json,mcp.json}
├─ admin/content-exclusion-example.yml
└─ global.json  (pins the .NET 8 SDK)
```

---

## ✅ Prerequisites

- .NET 8 SDK (`dotnet --list-sdks` should show an `8.0.x`; `global.json` pins it).
- Visual Studio 2026 with the GitHub Copilot and Copilot Chat components, **or** VS Code + C# Dev Kit.

---

## 🛠️ Build & run

```powershell
dotnet build
dotnet run --project src/DemoApp
dotnet test
```

> The demo app wraps `Checkout` in a try/catch so the off-by-one bug surfaces as a
> caught exception instead of crashing the walkthrough.

---

## 📦 Files in this kit

| Path | What it demonstrates |
| --- | --- |
| `src/ShoppingCart/Cart.cs` | Off-by-one bug, logged card number, money as `double`, no tests — inline suggestions, `/fix`, review, tests |
| `src/InventoryLegacy/InventoryLegacy.cs` | SQL injection, no types/docs — modernization, refactor, security, docs |
| `.github/copilot-instructions.md` | Repository-wide custom instructions |
| `.github/instructions/csharp.instructions.md` | Path-scoped instructions via `applyTo` |
| `.github/prompts/*.prompt.md` | Reusable prompt files for consistent responses |
| `.vscode/settings.json` | VS Code Copilot enablement per language (in Visual Studio, use **Tools > Options > GitHub > Copilot**) |
| `.vscode/mcp.json` | MCP servers for agent mode (Visual Studio also discovers `.mcp.json` / `.vs\mcp.json`) |
| `admin/content-exclusion-example.yml` | Content exclusion syntax |

---

## 🅰️ Part A — Use GitHub Copilot in the IDE

### 1️⃣ Demo 1 — Enable and scope Copilot (2 min)

- Open **Tools > Options > GitHub > Copilot** and walk the Copilot settings (global enablement, completions, Next Edit Suggestions).
- Use the **Copilot badge** in the top-right of the IDE to show/toggle Copilot state for the current session.
- Open a file type where you don't want completions and point out that enablement is controlled here, not per-file.

> **Talking point:** in Visual Studio, enablement is managed through **Tools > Options > GitHub > Copilot** and the Copilot badge — not a `.vscode/settings.json` file (that map is a VS Code mechanism).

---

### 2️⃣ Demo 2 — Inline suggestions and Next Edit Suggestions (3 min)

- In `src/ShoppingCart/Cart.cs`, put the cursor at the end of the `ShoppingCartModule` class (after `Cart.cs:58`, before the closing `}` on `Cart.cs:59`) and type:

  ```csharp
  // Returns the cart total in integer cents, including tax
  public static int TotalWithTax(Cart cart, double taxRate)
  ```

- Accept with `Tab`, cycle alternatives with `Alt+.` / `Alt+,`, and accept word-by-word with `Ctrl+->` (line-by-line with `Ctrl+Down`).
- Ensure Next Edit Suggestions is on (**Tools > Options > GitHub > Copilot > Copilot Completions > Enable Next Edit Suggestions**), then rename the `total` variable in `Subtotal` (declared on `Cart.cs:40`, used on `Cart.cs:43` and `Cart.cs:45`) to `runningTotal`, and `Tab` to navigate/accept the follow-on edits so all three references update together.

> **Talking point:** ghost-text completions, partial accept, and edit-aware NES are different features; NES uses `Tab` to jump to and accept the next predicted edit.

---

### 3️⃣ Demo 3 — Inline chat and the chat panel (4 min)

- Select the `Subtotal` method (`Cart.cs:38-46`), right-click > **Ask Copilot** (or use inline chat), and ask:

  > Why does this throw an error on the last iteration? Fix it and keep the same signature.

  (The off-by-one is the `i <= cart.Items.Count` loop bound on `Cart.cs:41`.)

- Then open the chat window (**View > GitHub Copilot Chat**) and run, in order:
  - `/explain` on the selection
  - `/fix` on `Checkout` (`Cart.cs:53-58`)
  - `/tests` for `Cart.cs`
  - `@workspace where is the discount percentage applied?` (`ApplyDiscount` on `Cart.cs:48-50`, called from `Checkout` on `Cart.cs:55`)
  - `#Cart.cs summarise the public API in a table`

> **Talking point:** the chat building blocks in Visual Studio are slash commands (`/explain`, `/fix`, `/tests`, `/doc`, `/optimize`), the `@workspace` participant for solution context (plus `@github` on Enterprise), and `#` references for files, methods, and classes (for example `#Cart.cs`, `#Subtotal`). Call out the limits: chat has a bounded context window and per-plan rate limits, and you can compact the conversation to free up space.

---

### 4️⃣ Demo 4 — Multi-file edits in Agent mode (4 min)

- Open the chat window, switch the mode dropdown to **Agent**, and add both `src/ShoppingCart/Cart.cs` and `src/InventoryLegacy/InventoryLegacy.cs` as context (the `+` button).
- Ask:

  > Money must be handled as integer cents across both files. Update the code and any callers,
  > and keep the public method names unchanged.

  Money currently uses `double` at: `Cart.cs:9` (`CartItem.Price`), `Cart.cs:16` (`Cart.Discount`), `Cart.cs:26-27` (`CheckoutResult.OrderId`/`Total`), and `InventoryLegacy.cs:52-54` (`price_with_tax`).

- Review the proposed diffs per file, accept some, discard others, then undo.

> **Talking point:** Visual Studio agent mode drives multi-file, diff-first, reversible edits — it plans the change, edits across files, and asks before running commands.

---

### 5️⃣ Demo 5 — Agent mode (5 min)

- Switch the chat to Agent, then ask:

  > Add an xUnit test suite for src/InventoryLegacy, create the tests project layout if needed,
  > run `dotnet test`, and fix anything that fails.

  Good methods to target: `get_stock` (`InventoryLegacy.cs:13-21`), `adjust` (`InventoryLegacy.cs:23-33`), `reorder_report` (`InventoryLegacy.cs:35-50`), and `price_with_tax` (`InventoryLegacy.cs:52-54`).

- Let it plan, edit files, and request terminal approval. Point out the tool-approval prompt.

> **Talking point:** agent mode chooses its own files and tools, iterates on failures, and
> asks before running commands.

---

### 6️⃣ Demo 6 — MCP in agent mode (3 min)

- Open the solution's `.mcp.json` (Visual Studio discovers `%USERPROFILE%\.mcp.json`, `<SolutionDir>\.mcp.json`, `<SolutionDir>\.vs\mcp.json`, and `<SolutionDir>\.vscode\mcp.json`).
- In the chat window, switch to **Agent**, open the **tools** (wrench) icon, and enable the MCP-provided tools (tools are disabled by default per server).
- Ask:

  > Using the GitHub tools, list the open issues on this repository and draft a fix plan for
  > the oldest one.

- Show the tools picker listing the MCP-provided tools.

> **Talking point:** MCP extends agent mode with external systems using one open protocol; servers are configured per solution or per user, each tool is enabled explicitly, and every tool call is approval-gated.

---

### 7️⃣ Demo 7 — Agent sessions and delegation (3 min)

- On github.com, open the Agents panel (or assign an issue to Copilot) and delegate:

  > Fix the off-by-one bug in Subtotal() and open a pull request.

- Show the session log, the branch it creates, and the draft PR.
- Mention sub-agents: the session delegates focused units of work so the main context window is not consumed by side quests.

> **Talking point:** delegated sessions run on GitHub-hosted compute; you review the PR like
> any other contributor.

---

### 8️⃣ Demo 8 — Copilot CLI (5 min)

- In a terminal at the repo root, start an interactive session:

  ```
  copilot                       # interactive session
  ```

- Inside the session, explore the commands:
  - `/help`, `/model`, `/clear`, `/session`, `/exit`
- Then try a few asks:
  - `explain what src/InventoryLegacy/InventoryLegacy.cs does and list its risks`
  - `write a PowerShell script that backs up every .cs file into backups/ with a timestamp, then run it`
  - `rename src/ShoppingCart/Cart.cs to src/ShoppingCart/ShoppingCart.cs and update the namespace references`
- Or run non-interactively:

  ```
  copilot -p "generate a .gitignore for a .NET and Node project"
  ```

> **Talking point:** the CLI brings Copilot to the terminal for people who do not live in an
> IDE — it can explain commands, generate scripts, and act on files, and it asks for approval
> before changing anything.

---

## 🅱️ Part B — Code review, collaboration, and organization settings

### 9️⃣ Demo 9 — Code review and custom review standards (4 min)

- In `src/ShoppingCart/Cart.cs`, select `Checkout` (`Cart.cs:53-58`) > right-click > **Copilot Actions > Review Selection** to get inline review comments (or use the sparkle button in the **Git Changes** window to review local changes). Expect a flag on the logged card number (`Cart.cs:56`).
- Open `.github/copilot-instructions.md`, show the Review standards section, and re-run the review to demonstrate that the guidance changes the comments.
- On github.com, request a review from Copilot on a pull request.

> **Talking point:** review standards are instructions-file driven, so review output is
> consistent across the team.

---

### 🔟 Demo 10 — PR summaries, Spaces, and Spark (3 min)

- On an open PR, use Copilot > Generate summary and show the walkthrough it writes.
- Open a Copilot Space, attach this repo plus the study guide link, and ask a question that needs both.
- In Spark, describe a tiny inventory dashboard and let it build and deploy the app.

> **Talking point:** Spaces are curated, shareable context; Spark goes from prompt to running app.

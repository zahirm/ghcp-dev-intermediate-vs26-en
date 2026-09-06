# Solution Overview — GH-300 Copilot Demo Kit (.NET 8)

This document explains what the solution is, how its pieces fit together, and where
the **intentional defects** live. The whole kit exists to give GitHub Copilot real
work to do during demos (`/fix`, `/tests`, review, refactor, modernize).

---

## 1. The big picture

There are three source projects and one test project. `DemoApp` is the entry point;
it drives the two "imperfect" library modules so their planted bugs surface at runtime.

<svg xmlns="http://www.w3.org/2000/svg" width="760" height="340" viewBox="0 0 760 340" font-family="'Segoe UI', system-ui, sans-serif">
  <defs>
	<marker id="arrow" markerWidth="8" markerHeight="8" refX="6" refY="3" orient="auto">
	  <path d="M0,0 L6,3 L0,6 Z" fill="#94a3b8" />
	</marker>
	<marker id="arrowV" markerWidth="8" markerHeight="8" refX="6" refY="3" orient="auto">
	  <path d="M0,0 L6,3 L0,6 Z" fill="#7A56C2" />
	</marker>
	<filter id="shadow" x="-20%" y="-20%" width="140%" height="140%">
	  <feDropShadow dx="0" dy="2" stdDeviation="3" flood-color="#0f172a" flood-opacity="0.15" />
	</filter>
  </defs>
  <!-- connectors (behind nodes) -->
  <line x1="380" y1="100" x2="200" y2="168" stroke="#94a3b8" stroke-width="2" marker-end="url(#arrow)" />
  <line x1="380" y1="100" x2="560" y2="168" stroke="#94a3b8" stroke-width="2" marker-end="url(#arrow)" />
  <line x1="160" y1="300" x2="160" y2="236" stroke="#7A56C2" stroke-width="2" stroke-dasharray="6,5" marker-end="url(#arrowV)" />
  <!-- DemoApp -->
  <rect x="290" y="40" width="180" height="60" rx="12" fill="#4C7EF3" filter="url(#shadow)" />
  <text x="380" y="68" fill="#fff" text-anchor="middle" font-size="15" font-weight="600">DemoApp</text>
  <text x="380" y="87" fill="#dbe6ff" text-anchor="middle" font-size="12">Program.cs · driver</text>
  <!-- ShoppingCart -->
  <rect x="70" y="170" width="180" height="64" rx="12" fill="#2FA95B" filter="url(#shadow)" />
  <text x="160" y="198" fill="#fff" text-anchor="middle" font-size="15" font-weight="600">ShoppingCart</text>
  <text x="160" y="217" fill="#dcf3e5" text-anchor="middle" font-size="12">Cart.cs</text>
  <!-- InventoryLegacy -->
  <rect x="470" y="170" width="180" height="64" rx="12" fill="#C9772E" filter="url(#shadow)" />
  <text x="560" y="198" fill="#fff" text-anchor="middle" font-size="15" font-weight="600">InventoryLegacy</text>
  <text x="560" y="217" fill="#fae6d3" text-anchor="middle" font-size="12">InventoryLegacy.cs</text>
  <!-- Tests -->
  <rect x="70" y="300" width="180" height="32" rx="10" fill="#7A56C2" filter="url(#shadow)" />
  <text x="160" y="321" fill="#fff" text-anchor="middle" font-size="13" font-weight="600">Tests · xUnit</text>
</svg>

| Project | File | Responsibility |
| --- | --- | --- |
| `DemoApp` | `Program.cs` | Console driver that runs both modules end-to-end |
| `ShoppingCart` | `Cart.cs` | Cart totals, discount, checkout |
| `InventoryLegacy` | `InventoryLegacy.cs` | SQLite-backed stock read/adjust/report |
| `ShoppingCart.Tests` | `UnitTest1.cs` | Deliberately minimal test coverage |

---

## 2. ShoppingCart module

The cart holds a list of items (price + quantity) and a discount. Checkout computes a
discounted total and returns an order.

<svg xmlns="http://www.w3.org/2000/svg" width="760" height="140" viewBox="0 0 760 140" font-family="'Segoe UI', system-ui, sans-serif">
  <defs>
	<marker id="arrowG" markerWidth="8" markerHeight="8" refX="6" refY="3" orient="auto">
	  <path d="M0,0 L6,3 L0,6 Z" fill="#2FA95B" />
	</marker>
	<filter id="shadowG" x="-20%" y="-20%" width="140%" height="140%">
	  <feDropShadow dx="0" dy="2" stdDeviation="2.5" flood-color="#0f172a" flood-opacity="0.12" />
	</filter>
  </defs>
  <g fill="#0f172a" font-size="13" text-anchor="middle">
	<rect x="30"  y="48" width="140" height="48" rx="10" fill="#eafaf0" stroke="#2FA95B" stroke-width="1.5" filter="url(#shadowG)"/>
	<text x="100" y="77">AddItem</text>
	<rect x="215" y="48" width="140" height="48" rx="10" fill="#eafaf0" stroke="#2FA95B" stroke-width="1.5" filter="url(#shadowG)"/>
	<text x="285" y="77">Subtotal</text>
	<rect x="400" y="48" width="140" height="48" rx="10" fill="#eafaf0" stroke="#2FA95B" stroke-width="1.5" filter="url(#shadowG)"/>
	<text x="470" y="77">ApplyDiscount</text>
	<rect x="585" y="48" width="140" height="48" rx="10" fill="#eafaf0" stroke="#2FA95B" stroke-width="1.5" filter="url(#shadowG)"/>
	<text x="655" y="77">Checkout</text>
  </g>
  <g stroke="#2FA95B" stroke-width="2">
	<line x1="170" y1="72" x2="207" y2="72" marker-end="url(#arrowG)"/>
	<line x1="355" y1="72" x2="392" y2="72" marker-end="url(#arrowG)"/>
	<line x1="540" y1="72" x2="577" y2="72" marker-end="url(#arrowG)"/>
  </g>
</svg>

**Planted defects (on purpose):**

- **Off-by-one bug** — `Subtotal` loops `i <= Items.Count`, so it reads one past the
  end of the list and throws `ArgumentOutOfRangeException`.
- **Leaked card number** — `Checkout` writes `user.CardNumber` to the console.
- **Money as `double`** — prices should be integer cents, not floating point.
- **Missing XML docs** on public members.

`DemoApp` wraps `Checkout` in a `try/catch` so the off-by-one surfaces as a *caught*
exception instead of crashing the walkthrough.

---

## 3. InventoryLegacy module

A SQLite-backed inventory in a deliberately dated style — no docs, no types on some
inputs, and string-built SQL.

<svg xmlns="http://www.w3.org/2000/svg" width="760" height="260" viewBox="0 0 760 260" font-family="'Segoe UI', system-ui, sans-serif">
  <defs>
	<marker id="arrowO" markerWidth="8" markerHeight="8" refX="6" refY="3" orient="auto">
	  <path d="M0,0 L6,3 L0,6 Z" fill="#C9772E" />
	</marker>
	<filter id="shadowO" x="-20%" y="-20%" width="140%" height="140%">
	  <feDropShadow dx="0" dy="2" stdDeviation="2.5" flood-color="#0f172a" flood-opacity="0.12" />
	</filter>
  </defs>
  <!-- connectors first (straight fan, no overlaps) -->
  <g stroke="#C9772E" stroke-width="2">
	<line x1="210" y1="42"  x2="470" y2="110" marker-end="url(#arrowO)"/>
	<line x1="210" y1="98"  x2="470" y2="122" marker-end="url(#arrowO)"/>
	<line x1="210" y1="154" x2="470" y2="134" marker-end="url(#arrowO)"/>
	<line x1="210" y1="210" x2="470" y2="146" marker-end="url(#arrowO)"/>
  </g>
  <!-- methods -->
  <g fill="#0f172a" font-size="13" text-anchor="middle">
	<rect x="30" y="20"  width="180" height="44" rx="10" fill="#fdf3e9" stroke="#C9772E" stroke-width="1.5" filter="url(#shadowO)"/>
	<text x="120" y="47">get_stock</text>
	<rect x="30" y="76"  width="180" height="44" rx="10" fill="#fdf3e9" stroke="#C9772E" stroke-width="1.5" filter="url(#shadowO)"/>
	<text x="120" y="103">adjust</text>
	<rect x="30" y="132" width="180" height="44" rx="10" fill="#fdf3e9" stroke="#C9772E" stroke-width="1.5" filter="url(#shadowO)"/>
	<text x="120" y="159">bulk_import</text>
	<rect x="30" y="188" width="180" height="44" rx="10" fill="#fdf3e9" stroke="#C9772E" stroke-width="1.5" filter="url(#shadowO)"/>
	<text x="120" y="215">reorder_report</text>
  </g>
  <!-- db cylinder -->
  <g filter="url(#shadowO)">
	<path d="M485 104 h150 v52 a75 12 0 0 1 -150 0 z" fill="#C9772E"/>
	<ellipse cx="560" cy="104" rx="75" ry="12" fill="#d98a45"/>
  </g>
  <text x="560" y="134" fill="#fff" text-anchor="middle" font-size="14" font-weight="600">SQLite</text>
  <text x="560" y="152" fill="#fae6d3" text-anchor="middle" font-size="11">stock table</text>
</svg>

**Planted defects (on purpose):**

- **SQL injection** — `get_stock`, `adjust`, and `bulk_import` build SQL by string
  concatenation / `string.Format` instead of using `SqliteParameter`.
- **No XML docs**, dated naming (`get_stock`, `c`, `r`, `n`).
- **Money as `double`** in `price_with_tax`.

The `stock` table has three columns: `sku`, `qty`, `reorder_point`.

---

## 4. Runtime flow (what happens on `dotnet run`)

<svg xmlns="http://www.w3.org/2000/svg" width="760" height="250" viewBox="0 0 760 250" font-family="'Segoe UI', system-ui, sans-serif">
  <defs>
	<marker id="arrowB" markerWidth="8" markerHeight="8" refX="6" refY="3" orient="auto">
	  <path d="M0,0 L6,3 L0,6 Z" fill="#4C7EF3" />
	</marker>
	<filter id="shadowB" x="-20%" y="-20%" width="140%" height="140%">
	  <feDropShadow dx="0" dy="2" stdDeviation="2.5" flood-color="#0f172a" flood-opacity="0.12" />
	</filter>
  </defs>

  <!-- connectors between step boxes -->
  <g stroke="#4C7EF3" stroke-width="2">
	<line x1="380" y1="72" x2="380" y2="88"  marker-end="url(#arrowB)"/>
	<line x1="380" y1="152" x2="380" y2="168" marker-end="url(#arrowB)"/>
  </g>
  <g font-size="13" fill="#0f172a">
	<rect x="60" y="20" width="640" height="52" rx="12" fill="#eef3fe" stroke="#4C7EF3" stroke-width="1.5" filter="url(#shadowB)"/>
	<circle cx="92" cy="46" r="14" fill="#4C7EF3"/>
	<text x="92" y="51" fill="#fff" text-anchor="middle" font-weight="600">1</text>
	<text x="120" y="51">Build a Cart, add two items, call Checkout &#8212; off-by-one throws, caught &amp; printed</text>
	<rect x="60" y="100" width="640" height="52" rx="12" fill="#eef3fe" stroke="#4C7EF3" stroke-width="1.5" filter="url(#shadowB)"/>
	<circle cx="92" cy="126" r="14" fill="#4C7EF3"/>
	<text x="92" y="131" fill="#fff" text-anchor="middle" font-weight="600">2</text>
	<text x="120" y="131">Open in-memory SQLite, create the stock table</text>
	<rect x="60" y="180" width="640" height="52" rx="12" fill="#eef3fe" stroke="#4C7EF3" stroke-width="1.5" filter="url(#shadowB)"/>
	<circle cx="92" cy="206" r="14" fill="#4C7EF3"/>
	<text x="92" y="211" fill="#fff" text-anchor="middle" font-weight="600">3</text>
	<text x="120" y="211">bulk_import rows, then get_stock / adjust / reorder_report / price_with_tax</text>
  </g>
</svg>

---

## 5. How to build and run

```powershell
dotnet build
dotnet run --project src/DemoApp
dotnet test
```

---

## 6. Defect cheat sheet (Copilot demo targets)

| Where | Defect | Fix it with |
| --- | --- | --- |
| `Cart.Subtotal` | Off-by-one (`<=`) | `/fix`, inline chat |
| `Cart.Checkout` | Logs card number | Security review |
| `Cart` / `CartItem` | Money as `double` | Copilot Edits (cents) |
| `InventoryLegacy` | SQL injection | Refactor to `SqliteParameter` |
| both modules | Missing XML docs | `/doc`, modernize |
| `ShoppingCart.Tests` | Almost no tests | `/tests`, agent mode |

> Every issue above is intentional — the kit is a training ground, not production code.

// DemoApp - a tiny driver that exercises the deliberately-flawed demo modules.
// This exists so agent-mode / run demos have an entrypoint to execute.
// Expect it to reveal the planted defects (e.g., Subtotal throws, card number is logged).

using Microsoft.Data.Sqlite;
using ShoppingCart;

Console.WriteLine("== ShoppingCart demo ==");

var cart = new Cart { Discount = 0.10 };
ShoppingCartModule.AddItem(cart, new CartItem { Price = 9.99, Qty = 2 });
ShoppingCartModule.AddItem(cart, new CartItem { Price = 4.50, Qty = 1 });

try
{
    var result = ShoppingCartModule.Checkout(cart, new User { CardNumber = "4111111111111111" });
    Console.WriteLine($"order {result.OrderId} total {result.Total}");
}
catch (Exception ex)
{
    // The off-by-one bug in Subtotal surfaces here on the last iteration.
    Console.WriteLine($"Checkout failed: {ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("== InventoryLegacy demo ==");

using var conn = new SqliteConnection("Data Source=:memory:");
conn.Open();

var create = conn.CreateCommand();
create.CommandText = "CREATE TABLE stock (sku TEXT, qty INTEGER, reorder_point INTEGER)";
create.ExecuteNonQuery();

var inventory = new InventoryLegacy.InventoryLegacy();
inventory.bulk_import(conn, new object[][]
{
    new object[] { "A-1", 3, 5 },
    new object[] { "B-2", 12, 4 },
    new object[] { "C-3", 0, 2 },
});

Console.WriteLine($"stock A-1: {inventory.get_stock(conn, "A-1")}");
Console.WriteLine($"adjust A-1 by -1: {inventory.adjust(conn, "A-1", -1)}");

Console.WriteLine("reorder report (threshold 10):");
foreach (var line in inventory.reorder_report(conn, 10))
{
    Console.WriteLine($"  {line}");
}

Console.WriteLine($"price_with_tax(100, 0.2): {inventory.price_with_tax(100, 0.2)}");

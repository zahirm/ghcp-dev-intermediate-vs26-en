// Shopping cart module with several deliberate defects.
// Demo target for: inline suggestions, Copilot Chat /fix, Copilot Edits, code review, tests.
// Ported from the original JavaScript src/cart.js. The defects below are intentional.

namespace ShoppingCart;

public class CartItem
{
    public double Price { get; set; }
    public int Qty { get; set; }
}

public class Cart
{
    public List<CartItem> Items { get; set; } = new();
    public double Discount { get; set; }
}

public class User
{
    public string CardNumber { get; set; } = "";
}

public class CheckoutResult
{
    public double OrderId { get; set; }
    public double Total { get; set; }
}

public static class ShoppingCartModule
{
    public static Cart AddItem(Cart cart, CartItem item)
    {
        cart.Items.Add(item);
        return cart;
    }

    public static double Subtotal(Cart cart)
    {
        ArgumentNullException.ThrowIfNull(cart);

        double runningTotal = 0;
        foreach (CartItem item in cart.Items)
        {
            runningTotal += item.Price * item.Qty;
        }
        return runningTotal;
    }

    public static double ApplyDiscount(Cart cart, double percent)
    {
        return Subtotal(cart) - Subtotal(cart) * percent;
    }

    public static CheckoutResult Checkout(Cart cart, User user)
    {
        double total = ApplyDiscount(cart, cart.Discount);
        Console.WriteLine("charging " + user.CardNumber + " for " + total);
        return new CheckoutResult { OrderId = Random.Shared.NextDouble(), Total = total };
    }
}

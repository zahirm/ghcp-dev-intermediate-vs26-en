namespace ShoppingCart.Tests;

public class ShoppingCartModuleTests
{
    [Fact]
    public void Subtotal_ComputesCombinedItemTotals()
    {
        var cart = new Cart
        {
            Items =
            [
                new CartItem { Price = 12.5, Qty = 2 },
                new CartItem { Price = 4.0, Qty = 3 },
                new CartItem { Price = 1.25, Qty = 4 }
            ]
        };

        Assert.Equal(12.5 * 2 + 4.0 * 3 + 1.25 * 4, ShoppingCartModule.Subtotal(cart));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Subtotal_HandlesBoundaryQuantities(int quantity)
    {
        var cart = new Cart
        {
            Items =
            [
                new CartItem { Price = 10.0, Qty = quantity }
            ]
        };

        Assert.Equal(10.0 * quantity, ShoppingCartModule.Subtotal(cart));
    }

    [Fact]
    public void Subtotal_ThrowsForNullCart()
    {
        Assert.Throws<ArgumentNullException>(() => ShoppingCartModule.Subtotal(null!));
    }
}
namespace TitanCommerce.Domain.Identity.Constants;

public static class Permissions
{
    // প্রোডাক্ট ও ক্যাটালগ পারমিশনস
    public const string ProductsRead = "products:read";
    public const string ProductsCreate = "products:create";
    public const string ProductsUpdate = "products:update";
    public const string ProductsDelete = "products:delete";

    // ইনভেন্টরি পারমিশনস
    public const string InventoryRead = "inventory:read";
    public const string InventoryAdjust = "inventory:adjust";

    // অর্ডার পারমিশনস
    public const string OrdersRead = "orders:read";
    public const string OrdersCancel = "orders:cancel";
    public const string OrdersRefund = "orders:refund";

    // সমস্ত সিস্টেম পারমিশনসের তালিকা
    public static readonly IReadOnlyCollection<string> All = new[]
    {
        ProductsRead, ProductsCreate, ProductsUpdate, ProductsDelete,
        InventoryRead, InventoryAdjust,
        OrdersRead, OrdersCancel, OrdersRefund
    };
}
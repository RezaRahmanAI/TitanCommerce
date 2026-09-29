using TitanCommerce.Domain.Identity.Enums;

namespace TitanCommerce.Domain.Identity.Constants;

public static class RolePermissions
{
    private static readonly Dictionary<UserRole, HashSet<string>> Map = new()
    {
        [UserRole.Customer] = new HashSet<string>
        {
            Permissions.ProductsRead,
            Permissions.OrdersRead
        },
        [UserRole.Seller] = new HashSet<string>
        {
            Permissions.ProductsRead,
            Permissions.ProductsCreate,
            Permissions.ProductsUpdate,
            Permissions.InventoryRead,
            Permissions.InventoryAdjust,
            Permissions.OrdersRead
        },
        [UserRole.SupportAgent] = new HashSet<string>
        {
            Permissions.ProductsRead,
            Permissions.OrdersRead,
            Permissions.OrdersCancel
        },
        [UserRole.Admin] = new HashSet<string>(Permissions.All)
    };

    public static IReadOnlyCollection<string> GetPermissionsForRole(UserRole role) =>
        Map.TryGetValue(role, out var permissions)
            ? permissions
            : Array.Empty<string>();
}

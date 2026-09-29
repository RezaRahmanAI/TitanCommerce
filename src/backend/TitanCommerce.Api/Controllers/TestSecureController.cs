using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TitanCommerce.Application.Common.Security;
using TitanCommerce.Domain.Identity.Constants;

namespace TitanCommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class TestSecureController : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        return Ok(new
        {
            message = "This endpoint is accessible by any authenticated user.",
            user = User.Identity?.Name ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        });
    }

    [HttpDelete("product-delete-test")]
    [HasPermissionAtribute(Permissions.ProductsDelete)] 
    public IActionResult DeleteProductTest()
    {
        return Ok(new { message = "Product deleted successfully (Permission: products:delete verified)!" });
    }
}
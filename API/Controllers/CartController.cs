using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CartController(ICartService cartService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
    {
        var cart = await cartService.GetCartAsync(id);

        return Ok(cart ?? new ShoppingCart { Id = id });
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        var updatedCart = await cartService.SetCartAsync(cart);
        if (updatedCart == null)
            return BadRequest("Problem updating cart");

        return Ok(updatedCart);
    }

    [HttpDelete]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(string id)
    {
        var isDeleted = await cartService.DeleteCartAsync(id);
        if (!isDeleted)
            return BadRequest("Problem deleting cart");

        return Ok();
    }
}

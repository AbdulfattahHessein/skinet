using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specification;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class OrdersController(IUnitOfWork unit, ICartService cartService) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
    {
        var email = User.GetEmail();

        var cart = await cartService.GetCartAsync(orderDto.CartId);

        if (cart == null)
            return BadRequest("Cart not found");

        if (cart.PaymentIntentId is null)
            return BadRequest("no payment intent for this order");

        var spec = new ProductSpecification<Product>(
            p => cart.Items.Select(i => i.ProductId).Contains(p.Id),
            p => p
        );

        var products = await unit.Repository<Product>().ListAsync(spec);

        var productItems = products
            .Select(p => new OrderItem
            {
                ItemOrdered = new()
                {
                    ProductId = p.Id,
                    PictureUrl = p.PictureUrl,
                    ProductName = p.Name,
                },
                Price = p.Price,
                Quantity = cart.Items.First(i => i.ProductId == p.Id).Quantity,
            })
            .ToList();

        var deliveryMethod = await unit.Repository<DeliveryMethod>()
            .GetByIdAsync(orderDto.DeliveryMethodId);

        if (deliveryMethod == null)
            return BadRequest("Delivery method not found");

        var order = new Order
        {
            OrderItems = productItems,
            DeliveryMethod = deliveryMethod,
            BuyerEmail = email,
            ShippingAddress = orderDto.ShippingAddress,
            PaymentIntentId = cart.PaymentIntentId,
            Subtotal = productItems.Sum(p => p.Price * p.Quantity),
            PaymentSummary = orderDto.PaymentSummary,
        };

        unit.Repository<Order>().Add(order);

        if (await unit.Complete())
        {
            return order;
        }

        return BadRequest("Failed to create order");
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Order>>> GetOrdersForUser()
    {
        var spec = new OrderSpecification(User.GetEmail());

        var orders = await unit.Repository<Order>().ListAsync(spec);

        return Ok(orders.Select(o => o.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrdersForUserById(int id)
    {
        var spec = new OrderSpecification(User.GetEmail(), id);

        var order = await unit.Repository<Order>().GetEntityWithSpec(spec);

        if (order is null)
        {
            return NotFound();
        }
        return order.ToDto();
    }
}

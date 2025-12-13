using System;
using Core.Entities;
using Core.Entities.OrderAggregate;

namespace API.DTOs;

public class OrderDto
{
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public required string BuyerEmail { get; set; }
    public required decimal ShippingPrice { get; set; }
    public required ShippingAddress ShippingAddress { get; set; }
    public required DeliveryMethod DeliveryMethod { get; set; }
    public required PaymentSummary PaymentSummary { get; set; }
    public ICollection<OrderItemDto> OrderItems { get; set; } = [];
    public decimal Subtotal { get; set; }
    public required string Status { get; set; }
    public required string PaymentIntentId { get; set; }
    public required decimal Total { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string PictureUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

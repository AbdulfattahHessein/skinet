using System;
using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions;

public static class OrderMappingExtension
{

    extension(Order order)
    {
       public OrderDto ToDto()
        {
            return new OrderDto
            {
                BuyerEmail = order.BuyerEmail,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                ShippingAddress = order.ShippingAddress,
                DeliveryMethod = order.DeliveryMethod,
                PaymentIntentId = order.PaymentIntentId,
                Subtotal = order.Subtotal,
                PaymentSummary = order.PaymentSummary,
                OrderItems = [.. order.OrderItems.Select(x => x.ToDto() )],
                ShippingPrice = order.DeliveryMethod.Price,
                Total = order.Total
                
            };
        }
        
    }

    extension(OrderItem item)
    {
        public OrderItemDto ToDto()
        {
            return new OrderItemDto
            {
                ProductId = item.ItemOrdered.ProductId,
                ProductName = item.ItemOrdered.ProductName,
                PictureUrl = item.ItemOrdered.PictureUrl,
                Quantity = item.Quantity,
                Price = item.Price,
            
            };
        }
    }
}

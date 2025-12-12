using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(
    IConfiguration configuration,
    ICartService cartService,
    IGenericRepository<Core.Entities.Product> productRepository,
    IGenericRepository<DeliveryMethod> deliveryMethodRepository
) : IPaymentService
{
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];

        var cart = await cartService.GetCartAsync(cartId);
        if (cart == null)
            return null;

        var shippingPrice = 0m;

        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await deliveryMethodRepository.GetByIdAsync(
                cart.DeliveryMethodId.Value
            );

            if (deliveryMethod == null)
                return null;

            shippingPrice = deliveryMethod.Price;
        }

        foreach (var item in cart.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                return null;

            if (item.Price != product.Price)
                item.Price = product.Price;
        }

        var service = new PaymentIntentService();

        PaymentIntent? intent = null;

        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var paymentIntent = new PaymentIntentCreateOptions
            {
                Amount =
                    (long)cart.Items.Sum(i => i.Quantity * i.Price * 100)
                    + (long)(shippingPrice * 100),

                Currency = "usd",
                PaymentMethodTypes = ["card"],
            };
            intent = await service.CreateAsync(paymentIntent);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret;
        }
        else
        {
            var paymentIntentUpdateOptions = new PaymentIntentUpdateOptions
            {
                Amount =
                    (long)cart.Items.Sum(i => i.Quantity * i.Price * 100)
                    + (long)(shippingPrice * 100),
            };

            intent = await service.UpdateAsync(cart.PaymentIntentId, paymentIntentUpdateOptions);
        }

        await cartService.SetCartAsync(cart);

        return cart;
    }
}

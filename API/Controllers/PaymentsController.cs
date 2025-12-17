using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;

public class PaymentsController(
    IPaymentService paymentService,
    IUnitOfWork unit,
    ILogger<PaymentsController> logger,
    IConfiguration config,
    IHubContext<NotificationHub> hub
) : BaseApiController
{
    private readonly string _wSecret = config["StripeSettings:WhSecret"]!;

    [HttpPost("{cartId}")]
    [Authorize]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);
        if (cart == null)
            return BadRequest("Problem with your cart");

        return Ok(cart);
    }

    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
        var methods = await unit.Repository<DeliveryMethod>().ListAllAsync();
        return Ok(methods);
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        try
        {
            var stripEvent = ConstructStripeEvent(json);
            if (stripEvent.Data.Object is not PaymentIntent paymentIntent)
                return BadRequest("invalid event data");

            await HandlePaymentIntentSucceeded(paymentIntent);

            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe webhook error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Webhook Error");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "unexpected error occurred");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred"
            );
        }
    }

    private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
    {
        if (intent.Status == "succeeded")
        {
            var spec = new OrderSpecification(intent.Id, true);

            var order =
                await unit.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpec(spec)
                ?? throw new StripeException("order not fount");

            if ((long)order.Total * 100 != intent.Amount)
            {
                order.Status = OrderStatus.PaymentMismatch;
            }
            else
            {
                order.Status = OrderStatus.PaymentReceived;
            }

            await unit.Complete();

            // SignalR

            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
            if (!string.IsNullOrEmpty(connectionId))
                await hub
                    .Clients.Client(connectionId)
                    .SendAsync("OrderCompletedNotification", order.ToDto());
        }
    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
            return EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _wSecret,
                300,
                false
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            throw new StripeException("invalid signature");
        }
    }
}

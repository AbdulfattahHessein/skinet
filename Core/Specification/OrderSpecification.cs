using Core.Entities.OrderAggregate;

namespace Core.Specification;

public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(string email)
        : base(x => x.BuyerEmail == email)
    {
        AddInclude(x => x.DeliveryMethod);
        AddInclude(x => x.OrderItems);
        AddOrderByDescending(x => x.OrderDate);
    }

    public OrderSpecification(string email, int id)
        : base(x => x.BuyerEmail == email && x.Id == id)
    {
        AddInclude($"{nameof(Order.DeliveryMethod)}");
        AddInclude($"{nameof(Order.OrderItems)}");
    }

    public OrderSpecification(string paymentIntentId, bool isPaymentIntent)
        : base(x => x.PaymentIntentId == paymentIntentId)
    {
        AddInclude($"{nameof(Order.DeliveryMethod)}");
        AddInclude($"{nameof(Order.OrderItems)}");
    }
}

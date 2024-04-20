using FluentValidation;
using StoreApplication.Application.FoodOrder;

namespace StoreApplication.Api.Validators;

public class CreateValidatorPlaceOrder : AbstractValidator<PlaceOrderCommand>
{
    public CreateValidatorPlaceOrder()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");

        RuleForEach(x => x.OrderDetails).ChildRules(orderDetail =>
        {
            orderDetail.RuleFor(d => d.ProductId).NotEmpty().WithMessage("Product ID is required.");
            orderDetail.RuleFor(d => d.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}

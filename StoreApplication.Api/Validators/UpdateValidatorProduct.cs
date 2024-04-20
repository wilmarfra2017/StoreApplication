﻿using FluentValidation;
using StoreApplication.Application.Products;

namespace StoreApplication.Api.Validators
{
    public class UpdateValidatorProduct : AbstractValidator<UpdateProductCommand>
    {

        public UpdateValidatorProduct() 
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name must not be empty");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description must not be empty");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
            RuleFor(x => x.Stock).GreaterThan(0).WithMessage("Stock must be greater than 0.");
        }
    }
}

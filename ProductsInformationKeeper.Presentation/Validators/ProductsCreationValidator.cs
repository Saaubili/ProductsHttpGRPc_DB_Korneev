using Application.Models;
using FluentValidation;

namespace Presentation.Validators
{
    public class ProductsCreationValidator: AbstractValidator<ApplicationProductModel>
    {
        public ProductsCreationValidator()
        {
            RuleFor(p => p.ProductName)
               .NotEmpty().WithMessage("Название продукта не может быть пустым")
               .MaximumLength(200).WithMessage("Название продукта не может быть длиннее 200 символов");

            RuleFor(p => p.ProductPrice)
                .GreaterThan(-1).WithMessage("Цена продукта должна быть больше или равна 0");

            RuleFor(p => p.ProductWeight)
                .GreaterThan(0).WithMessage("Вес продукта должен быть больше нуля");

            RuleFor(p => p.ProductType)
                .IsInEnum().WithMessage("Недопустимый вид товара");

            RuleFor(p => p.WarehouseNumber)
                .GreaterThan(0).WithMessage("Номер склада должен быть больше нуля");
        }
    }
}

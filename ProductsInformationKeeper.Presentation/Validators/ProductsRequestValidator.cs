using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators
{
    public class ProductsRequestValidator : AbstractValidator<ProductsRequest>
    {
        public ProductsRequestValidator()
        {
            RuleFor(p => p.productName)
                .MaximumLength(200).WithMessage("Название продукта не может быть длиннее 200 символов");

            RuleFor(p => p.priceFrom)
                .GreaterThanOrEqualTo(0f).WithMessage("Начальная цена не может быть отрицательной")
                .When(p => p.priceFrom.HasValue);

            RuleFor(p => p.priceTo)
                .GreaterThanOrEqualTo(0f).WithMessage("Конечная цена не может быть отрицательной")
                .When(p => p.priceTo.HasValue);

            RuleFor(p => p.priceFrom)
                .LessThanOrEqualTo(p => p.priceTo)
                .When(p => p.priceFrom.HasValue && p.priceTo.HasValue)
                .WithMessage("Начальная цена не может быть больше конечной");

            RuleFor(p => p.weightFrom)
                .GreaterThanOrEqualTo(0f).WithMessage("Начальный вес не может быть отрицательным")
                .When(p => p.weightFrom.HasValue);

            RuleFor(p => p.weightTo)
                .GreaterThanOrEqualTo(0f).WithMessage("Конечный вес не может быть отрицательным")
                .When(p => p.weightTo.HasValue);

            RuleFor(p => p.weightFrom)
                .LessThanOrEqualTo(p => p.weightTo)
                .When(p => p.weightFrom.HasValue && p.weightTo.HasValue)
                .WithMessage("Начальный вес не может быть больше конечного");

            RuleFor(p => p.type)
                .IsInEnum().WithMessage("Недопустимый вид товара");

            RuleFor(p => p.warehouseNumber)
                .GreaterThan(0).WithMessage("Номер склада должен быть больше нуля")
                .When(p => p.warehouseNumber.HasValue);

            RuleFor(p => p.createdFrom)
                .LessThanOrEqualTo(p => p.createdTo)
                .When(p => p.createdFrom.HasValue && p.createdTo.HasValue)
                .WithMessage("Начальная дата не может быть позже конечной");

            RuleFor(p => p.pageNumber)
                .NotNull().WithMessage("Номер страницы обязателен")
                .GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть не меньше 1");

            RuleFor(p => p.pageSize)
                .NotNull().WithMessage("Размер страницы обязателен")
                .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100");
        }
    }
}
using FluentValidation;

namespace InnoShop.Products.Application.Features.Products.Queries
{
    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product ID must be provided.");
        }
    }
}

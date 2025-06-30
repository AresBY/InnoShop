using FluentValidation.TestHelper;
using InnoShop.Products.Application.Features.Products.Queries;

namespace InnoShop.Products.Tests.Application.Features.Products.Validation
{
    public class GetProductByIdQueryValidatorTests
    {
        private readonly GetProductByIdQueryValidator _validator;

        public GetProductByIdQueryValidatorTests()
        {
            _validator = new GetProductByIdQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var model = new GetProductByIdQuery { Id = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Product ID must be provided.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Id_Is_Valid()
        {
            var model = new GetProductByIdQuery { Id = Guid.NewGuid() };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}

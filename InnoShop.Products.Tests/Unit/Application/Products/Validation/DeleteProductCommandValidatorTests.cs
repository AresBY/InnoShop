using FluentValidation.TestHelper;
using InnoShop.Products.Application.Features.Products.Commands;

namespace InnoShop.Products.Tests.Application.Features.Products.Validation
{
    public class DeleteProductCommandValidatorTests
    {
        private readonly DeleteProductCommandValidator _validator;

        public DeleteProductCommandValidatorTests()
        {
            _validator = new DeleteProductCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var model = new DeleteProductCommand { Id = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Product ID must be provided.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Id_Is_Valid()
        {
            var model = new DeleteProductCommand { Id = Guid.NewGuid() };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}

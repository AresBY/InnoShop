using FluentValidation.TestHelper;
using InnoShop.Products.Application.Features.Products.Commands;

namespace InnoShop.Products.Tests.Application.Features.Products.Validation
{
    public class UpdateProductCommandValidatorTests
    {
        private readonly UpdateProductCommandValidator _validator;

        public UpdateProductCommandValidatorTests()
        {
            _validator = new UpdateProductCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var model = new UpdateProductCommand { Id = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("Product ID must be provided.");
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var model = new UpdateProductCommand { Name = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Product name is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Name_Too_Long()
        {
            var model = new UpdateProductCommand { Name = new string('a', 101) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Product name must be at most 100 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Empty()
        {
            var model = new UpdateProductCommand { Description = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Too_Long()
        {
            var model = new UpdateProductCommand { Description = new string('a', 1001) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description must be at most 1000 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_Price_Negative()
        {
            var model = new UpdateProductCommand { Price = -1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Price)
                .WithErrorMessage("Price must be non-negative.");
        }

        [Fact]
        public void Should_Have_Error_When_CreatedByUserId_Is_Empty()
        {
            var model = new UpdateProductCommand { CreatedByUserId = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CreatedByUserId)
                .WithErrorMessage("Creator user ID is required.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
        {
            var model = new UpdateProductCommand
            {
                Id = Guid.NewGuid(),
                Name = "Valid Name",
                Description = "Valid Description",
                Price = 10,
                CreatedByUserId = Guid.NewGuid()
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

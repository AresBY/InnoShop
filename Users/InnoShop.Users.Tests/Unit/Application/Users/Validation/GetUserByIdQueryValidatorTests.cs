using FluentValidation.TestHelper;
using InnoShop.Users.Application.Features.Users.Queries;

namespace InnoShop.Users.Tests.Unit.Application.Users.Validation
{
    public class GetUserByIdQueryValidatorTests
    {
        private readonly GetUserByIdQueryValidator _validator;

        public GetUserByIdQueryValidatorTests()
        {
            _validator = new GetUserByIdQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var query = new GetUserByIdQuery { Id = Guid.Empty };
            var result = _validator.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("User Id is required.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Id_Is_Not_Empty()
        {
            var query = new GetUserByIdQuery { Id = Guid.NewGuid() };
            var result = _validator.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}

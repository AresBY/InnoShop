using FluentValidation.TestHelper;
using InnoShop.Users.Application.Features.Users.Commands;

namespace InnoShop.Users.Tests.Unit.Application.Users.Validation
{
    public class DeleteUserCommandValidatorTests
    {
        private readonly DeleteUserCommandValidator _validator;

        public DeleteUserCommandValidatorTests()
        {
            _validator = new DeleteUserCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var command = new DeleteUserCommand { Id = Guid.Empty };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("User Id is required.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Id_Is_Not_Empty()
        {
            var command = new DeleteUserCommand { Id = Guid.NewGuid() };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}

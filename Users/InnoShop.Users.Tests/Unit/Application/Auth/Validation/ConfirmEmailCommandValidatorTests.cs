using FluentValidation.TestHelper;
using InnoShop.Users.Application.Features.Auth.Commands;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Validation
{
    public class ConfirmEmailCommandValidatorTests
    {
        private readonly ConfirmEmailCommandValidator _validator;

        public ConfirmEmailCommandValidatorTests()
        {
            _validator = new ConfirmEmailCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new ConfirmEmailCommand { Email = "", Token = "token" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new ConfirmEmailCommand { Email = "invalid-email", Token = "token" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Token_Is_Empty()
        {
            var command = new ConfirmEmailCommand { Email = "user@example.com", Token = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Token);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Valid()
        {
            var command = new ConfirmEmailCommand { Email = "user@example.com", Token = "valid-token" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
            result.ShouldNotHaveValidationErrorFor(x => x.Token);
        }
    }
}

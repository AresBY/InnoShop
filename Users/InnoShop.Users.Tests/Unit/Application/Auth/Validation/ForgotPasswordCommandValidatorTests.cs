using FluentValidation.TestHelper;
using InnoShop.Users.Application.Features.Auth.Commands;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Validation
{
    public class ForgotPasswordCommandValidatorTests
    {
        private readonly ForgotPasswordCommandValidator _validator;

        public ForgotPasswordCommandValidatorTests()
        {
            _validator = new ForgotPasswordCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new ForgotPasswordCommand { Email = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new ForgotPasswordCommand { Email = "invalid-email" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {
            var command = new ForgotPasswordCommand { Email = "user@example.com" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}

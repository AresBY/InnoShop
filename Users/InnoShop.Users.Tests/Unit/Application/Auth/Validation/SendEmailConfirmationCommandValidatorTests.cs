using FluentValidation.TestHelper;
using InnoShop.Users.Application.Features.Auth.Commands;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Validation
{
    public class SendEmailConfirmationCommandValidatorTests
    {
        private readonly SendEmailConfirmationCommandValidator _validator;

        public SendEmailConfirmationCommandValidatorTests()
        {
            _validator = new SendEmailConfirmationCommandValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Email_Is_NullOrEmpty(string email)
        {
            var command = new SendEmailConfirmationCommand { Email = email };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new SendEmailConfirmationCommand { Email = "invalidemail" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_Is_Valid()
        {
            var command = new SendEmailConfirmationCommand { Email = "test@example.com" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}

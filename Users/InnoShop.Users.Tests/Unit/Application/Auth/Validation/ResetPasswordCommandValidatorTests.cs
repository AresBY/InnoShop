using FluentValidation.TestHelper;
using InnoShop.Users.Application.Features.Auth.Commands;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Validation
{
    public class ResetPasswordCommandValidatorTests
    {
        private readonly ResetPasswordCommandValidator _validator;

        public ResetPasswordCommandValidatorTests()
        {
            _validator = new ResetPasswordCommandValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Email_Is_NullOrEmpty(string email)
        {
            var command = new ResetPasswordCommand { Email = email };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new ResetPasswordCommand { Email = "invalidemail" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Token_Is_NullOrEmpty(string token)
        {
            var command = new ResetPasswordCommand { Token = token };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Token);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        public void Should_Have_Error_When_NewPassword_Is_Invalid(string password)
        {
            var command = new ResetPasswordCommand { NewPassword = password };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid()
        {
            var command = new ResetPasswordCommand
            {
                Email = "test@example.com",
                Token = "valid-token",
                NewPassword = "newstrongpassword"
            };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
            result.ShouldNotHaveValidationErrorFor(x => x.Token);
            result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
        }
    }
}

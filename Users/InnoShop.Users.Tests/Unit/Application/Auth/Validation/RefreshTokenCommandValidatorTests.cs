using FluentValidation.TestHelper;
using InnoShop.Users.Application.Features.Auth.Commands;

namespace InnoShop.Users.Tests.Unit.Application.Auth.Validation
{
    public class RefreshTokenCommandValidatorTests
    {
        private readonly RefreshTokenCommandValidator _validator;

        public RefreshTokenCommandValidatorTests()
        {
            _validator = new RefreshTokenCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_RefreshToken_Is_Empty()
        {
            var command = new RefreshTokenCommand { RefreshToken = "" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
        }

        [Fact]
        public void Should_Not_Have_Error_When_RefreshToken_Is_Not_Empty()
        {
            var command = new RefreshTokenCommand { RefreshToken = "valid_refresh_token" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
        }
    }
}

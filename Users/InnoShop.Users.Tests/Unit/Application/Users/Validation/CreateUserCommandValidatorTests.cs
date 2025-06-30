using FluentValidation.TestHelper;
using InnoShop.Users.Application.Features.Users.Commands;

namespace InnoShop.Users.Tests.Unit.Application.Users.Validation
{
    public class CreateUserCommandValidatorTests
    {
        private readonly CreateUserCommandValidator _validator;

        public CreateUserCommandValidatorTests()
        {
            _validator = new CreateUserCommandValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Name_Is_NullOrEmpty(string name)
        {
            var command = new CreateUserCommand { Name = name, Email = "test@example.com", Password = "password" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Email_Is_NullOrEmpty(string email)
        {
            var command = new CreateUserCommand { Name = "Test", Email = email, Password = "password" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new CreateUserCommand { Name = "Test", Email = "invalidemail", Password = "password" };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("12345")] // less than 6 chars
        public void Should_Have_Error_When_Password_Is_Invalid(string password)
        {
            var command = new CreateUserCommand { Name = "Test", Email = "test@example.com", Password = password };
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var command = new CreateUserCommand { Name = "Test", Email = "test@example.com", Password = "password" };
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
}

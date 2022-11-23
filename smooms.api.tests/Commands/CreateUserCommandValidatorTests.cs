using FluentAssertions;
using FluentValidation;
using smooms.api.Commands;
using smooms.api.tests.Utils;

namespace smooms.api.tests.Commands;

public class CreateUserCommandValidatorTests : DbTest
{
    public CreateUserCommandValidatorTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Theory]
    [ClassData(typeof(InvalidEmailGenerator))]
    public async Task Validate_InvalidEmail_IsNotValid(string invalidEmail)
    {
        // Arrange
        var command = new CreateUserCommand("displayName", invalidEmail, "password");
        var validator = new CreateUserCommandValidator();
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateUserCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("no")]
    [InlineData("wayTooLongUsernameThatIsNotValidBecauseItIsLongerThanTheMaximumLengthOfTheDatabaseFieldThatIsDefinedInTheModel")]
    public async Task Validate_InvalidDisplayName_IsNotValid(string invalidUsername)
    {
        // Arrange
        var command = new CreateUserCommand(invalidUsername, "valid.email@web.de", "password");
        var validator = new CreateUserCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateUserCommand.DisplayName));
    }

    [Theory]
    [InlineData("")]
    public async Task Validate_InvalidPassword_IsNotValid(string invalidPassword)
    {
        // Arrange
        var command = new CreateUserCommand("displayName", "valid.email@web.de", invalidPassword);
        var validator = new CreateUserCommandValidator();

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateUserCommand.Password));
    }
}
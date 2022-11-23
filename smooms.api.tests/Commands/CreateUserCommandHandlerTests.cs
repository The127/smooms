using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Randomizer;
using smooms.api.Commands;
using smooms.api.Models;
using smooms.api.Services;
using smooms.api.tests.Utils;

namespace smooms.api.tests.Commands;

public class CreateUserCommandHandlerTests : DbTest
{
    public CreateUserCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Handle_ValidInputs_CreatesUser()
    {
        // Arrange
        var securityService = new Mock<ISecurityService>();
        var passwordBytes = Encoding.UTF8.GetBytes("hashedPassword");
        securityService.Setup(x => x.HashPassword(It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(passwordBytes);
        var command = new CreateUserCommand("displayName", "valid.email@email.com", "password");
        var handler = new CreateUserCommandHandler(DbContextFactory, securityService.Object);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.UserId.Value.Should().NotBeEmpty();
    }

    [Theory]
    [ClassData(typeof(InvalidEmailGenerator))]
    public async Task Handle_InvalidsEmail_ThrowsDbUpdateException(string invalidEmail)
    {
        // Arrange
        var securityService = new Mock<ISecurityService>();
        var command = new CreateUserCommand("displayName", invalidEmail, "password");
        var handler = new CreateUserCommandHandler(DbContextFactory, securityService.Object);
        
        // Act
        var action = async () => await handler.Handle(command);
        
        // Assert
        await action.Should().ThrowAsync<DbUpdateException>();
    }
    
    [Fact]
    public async Task Handle_TooLongEmail_ThrowsDbUpdateException()
    {
        // Arrange
        var securityService = new Mock<ISecurityService>();
        var command = new CreateUserCommand("displayName", 
            new RandomAlphanumericStringGenerator().GenerateValue(UserConfiguration.EmailMaxLength+1), 
            "password");
        var handler = new CreateUserCommandHandler(DbContextFactory, securityService.Object);
        
        // Act
        var action = async () => await handler.Handle(command);
        
        // Assert
        await action.Should().ThrowAsync<DbUpdateException>();
    }
    
    [Fact]
    public async Task Handle_TooLongUsername_ThrowsDbUpdateException()
    {
        // Arrange
        var securityService = new Mock<ISecurityService>();
        var command = new CreateUserCommand(
            new RandomAlphanumericStringGenerator().GenerateValue(UserConfiguration.UserNameMaxLength+1), 
            "valid.email@web.de", 
            "password");
        var handler = new CreateUserCommandHandler(DbContextFactory, securityService.Object);
        
        // Act
        var action = async () => await handler.Handle(command);
        
        // Assert
        await action.Should().ThrowAsync<DbUpdateException>();
    }
    
    [Fact]
    public async Task Handle_TooShortUsername_ThrowsDbUpdateException()
    {
        // Arrange
        var securityService = new Mock<ISecurityService>();
        var command = new CreateUserCommand(
            new RandomAlphanumericStringGenerator().GenerateValue(UserConfiguration.UserNameMinLength-1), 
            "email", 
            "password");
        var handler = new CreateUserCommandHandler(DbContextFactory, securityService.Object);
        
        // Act
        var action = async () => await handler.Handle(command);
        
        // Assert
        await action.Should().ThrowAsync<DbUpdateException>();
    }
}
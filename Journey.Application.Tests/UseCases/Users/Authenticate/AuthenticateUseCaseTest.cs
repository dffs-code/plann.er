using Journey.Application.UseCases.Users.AuthenticateUser;
using Journey.Application.UseCases.Users.Common;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UnitTests.UseCases.Users.AuthenticateUser
{
    public class AuthenticateUserUseCaseTests
    {
        [Fact]
        public void Execute_ShouldAuthenticateUser_WithCorrectCredentials()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_Authenticate")
                .Options;

            var salt = PasswordHandler.GenerateSalt();
            var hashedPassword = PasswordHandler.HashPassword("password123", salt);

            using (var context = new JourneyDbContext(options))
            {
                context.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Username = "testuser",
                    Password = hashedPassword,
                    Salt = salt,
                    Email = "testuser@example.com"
                });
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new AuthenticateUserUseCase(context);

                // Act
                var result = useCase.Execute("testuser", "password123");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("testuser", result.Username);
                Assert.Equal("testuser@example.com", result.Email);
            }
        }

        [Fact]
        public void Execute_ShouldThrowException_WithIncorrectUsername()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_Authenticate")
                .Options;

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new AuthenticateUserUseCase(context);

                // Act & Assert
                var exception = Assert.Throws<ErrorOnValidationException>(() => useCase.Execute("wronguser", "password123"));
                Assert.Contains(ResourceErrorMessages.WRONG_USERNAME_OR_PASSWORD, exception.GetErrorMessages());
            }
        }

        [Fact]
        public void Execute_ShouldThrowException_WithIncorrectPassword()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_Authenticate")
                .Options;

            var salt = PasswordHandler.GenerateSalt();
            var hashedPassword = PasswordHandler.HashPassword("password123", salt);

            using (var context = new JourneyDbContext(options))
            {
                context.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Username = "testuser",
                    Password = hashedPassword,
                    Salt = salt,
                    Email = "testuser@example.com"
                });
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new AuthenticateUserUseCase(context);

                // Act & Assert
                var exception = Assert.Throws<ErrorOnValidationException>(() => useCase.Execute("testuser", "wrongpassword"));
                Assert.Contains(ResourceErrorMessages.WRONG_USERNAME_OR_PASSWORD, exception.GetErrorMessages());
            }
        }

        [Fact]
        public void Execute_ShouldAuthenticateUser_WithMultipleUsers()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_Authenticate")
                .Options;

            var salt1 = PasswordHandler.GenerateSalt();
            var hashedPassword1 = PasswordHandler.HashPassword("password123", salt1);

            var salt2 = PasswordHandler.GenerateSalt();
            var hashedPassword2 = PasswordHandler.HashPassword("password456", salt2);

            using (var context = new JourneyDbContext(options))
            {
                context.Users.AddRange(
                    new User { Id = Guid.NewGuid(), Username = "testuser1", Password = hashedPassword1, Salt = salt1, Email = "testuser1@example.com" },
                    new User { Id = Guid.NewGuid(), Username = "testuser2", Password = hashedPassword2, Salt = salt2, Email = "testuser2@example.com" }
                );
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new AuthenticateUserUseCase(context);

                // Act
                var result = useCase.Execute("testuser1", "password123");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("testuser1", result.Username);
                Assert.Equal("testuser1@example.com", result.Email);
            }
        }
    }
}

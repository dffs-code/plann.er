using Journey.Application.UseCases.Users.Common;
using Journey.Application.UseCases.Users.Register;
using Journey.Communication.Requests;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;


namespace Journey.Application.Tests.UseCases.Users.Register
{
    public class RegisterUserUseCaseTest
    {
        [Fact]
        public void Execute_ShouldRegisterUser_WhenValidRequest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_RegisterValid")
                .Options;

            var request = new RequestRegisterUserJson
            {
                Username = "NewUser",
                Password = "Password123",
                Email = "newuser@example.com"
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new RegisterUserUseCase(context);

                // Act
                var result = useCase.Execute(request);

                // Assert
                Assert.NotNull(result);
                Assert.NotEqual(Guid.Empty, result.Id);
                Assert.Equal("NewUser", result.Username);
                Assert.Equal("newuser@example.com", result.Email);

                var userInDb = context.Users.FirstOrDefault(u => u.Username == "NewUser");
                Assert.NotNull(userInDb);
                Assert.Equal("NewUser", userInDb.Username);
                Assert.Equal("newuser@example.com", userInDb.Email);
                Assert.NotEmpty(userInDb.Password); // Password should be hashed and not empty
                Assert.NotEmpty(userInDb.Salt);     // Salt should be generated and not empty
            }
        }

        [Fact]
        public void Execute_ShouldThrowErrorOnValidationException_WhenInvalidRequest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_RegisterInvalid")
                .Options;

            var request = new RequestRegisterUserJson
            {
                Username = "",    // Invalid username
                Password = "",    // Invalid password
                Email = "invalid-email" // Invalid email
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new RegisterUserUseCase(context);

                // Act
                var exception = Assert.Throws<ErrorOnValidationException>(() => useCase.Execute(request));

                // Assert
                Assert.NotNull(exception);
                Assert.Contains(ResourceErrorMessages.USERNAME_EMPTY, exception.GetErrorMessages());
                Assert.Contains(ResourceErrorMessages.PASSWORD_EMPTY, exception.GetErrorMessages());
                Assert.Contains(ResourceErrorMessages.INVALID_EMAIL, exception.GetErrorMessages());
            }
        }

        [Fact]
        public void Execute_ShouldGenerateSaltAndHashPassword()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_GenerateSaltAndHash")
                .Options;

            var request = new RequestRegisterUserJson
            {
                Username = "UserWithHash",
                Password = "Password123",
                Email = "userwithhash@example.com"
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new RegisterUserUseCase(context);

                // Act
                var result = useCase.Execute(request);

                // Assert
                Assert.NotNull(result);

                var userInDb = context.Users.FirstOrDefault(u => u.Username == "UserWithHash");
                Assert.NotNull(userInDb);
                Assert.NotEmpty(userInDb.Password); // Password should be hashed and not empty
                Assert.NotEmpty(userInDb.Salt);     // Salt should be generated and not empty

                // Optional: Validate that the password is hashed and not equal to the plain password
                var plainPassword = "Password123";
                var hashedPassword = PasswordHandler.HashPassword(plainPassword, userInDb.Salt);
                Assert.NotEqual(plainPassword, hashedPassword); // Ensure the password is hashed
            }
        }

    }
}
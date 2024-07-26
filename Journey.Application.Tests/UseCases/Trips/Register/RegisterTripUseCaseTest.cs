using Journey.Application.UseCases.Trips.Register;
using Journey.Communication.Requests;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure.Entities;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Journey.Exception;

namespace Journey.Application.UnitTests.UseCases.Trips.Register
{
    public class RegisterTripUseCaseTest
    {
        [Fact]
        public void Execute_ShouldRegisterTripSuccessfully()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_RegisterTrip")
                .Options;

            var request = new RequestRegisterTripJson
            {
                Name = "Business Trip",
                StartDate = new DateTime(2024, 8, 1),
                EndDate = new DateTime(2024, 8, 5),
                Country = "USA",
                City = "New York",
                FullAddress = "123 Main St, New York, NY"
            };

            var userId = Guid.NewGuid().ToString();

            using (var context = new JourneyDbContext(options))
            {
                context.Users.Add(new User { Id = Guid.Parse(userId), Username = "User1", Email = "user1@example.com" });
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new RegisterTripUseCase(context);

                // Act
                var result = useCase.Execute(request, userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(request.Name, result.Name);
                Assert.Equal(request.StartDate, result.StartDate);
                Assert.Equal(request.EndDate, result.EndDate);
                Assert.Equal(request.Country, result.Country);
                Assert.Equal(request.City, result.City);
                Assert.Equal(request.FullAddress, result.FullAddress);
                Assert.Equal(userId, result.UserId);
                Assert.NotNull(result.User);
                Assert.Equal("User1", result.User.Username);
                Assert.Equal("user1@example.com", result.User.Email);
            }
        }

        [Fact]
        public void Execute_ShouldThrowErrorOnValidationException_WhenInvalidRequest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_RegisterTripInvalid")
                .Options;

            var request = new RequestRegisterTripJson
            {
                Name = "", // Invalid name
                StartDate = new DateTime(2024, 8, 1),
                EndDate = new DateTime(2024, 8, 5),
                Country = "", // Invalid country
                City = "New York",
                FullAddress = ""
            };

            var userId = Guid.NewGuid().ToString();

            using (var context = new JourneyDbContext(options))
            {
                context.Users.Add(new User { Id = Guid.Parse(userId), Username = "User1", Email = "user1@example.com" });
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new RegisterTripUseCase(context);

                // Act & Assert
                var exception = Assert.Throws<ErrorOnValidationException>(() => useCase.Execute(request, userId));

                // Assert
                Assert.NotNull(exception);
                Assert.Contains(ResourceErrorMessages.NAME_EMPTY, exception.GetErrorMessages());
                Assert.Contains(ResourceErrorMessages.EMPTY_COUNTRY, exception.GetErrorMessages());
                Assert.Contains(ResourceErrorMessages.EMPTY_ADDRESS, exception.GetErrorMessages());
            }
        }

        [Fact]
        public void Execute_ShouldHandleTripsWithManyUsers()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_RegisterTripManyUsers")
                .Options;

            var request = new RequestRegisterTripJson
            {
                Name = "Business Trip",
                StartDate = new DateTime(2024, 8, 1),
                EndDate = new DateTime(2024, 8, 5),
                Country = "USA",
                City = "New York",
                FullAddress = "123 Main St, New York, NY"
            };

            var userId = Guid.NewGuid().ToString();

            using (var context = new JourneyDbContext(options))
            {
                for (int i = 0; i < 1000; i++)
                {
                    context.Users.Add(new User { Id = Guid.NewGuid(), Username = $"User{i}", Email = $"user{i}@example.com" });
                }
                context.Users.Add(new User { Id = Guid.Parse(userId), Username = "User1", Email = "user1@example.com" });
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new RegisterTripUseCase(context);

                // Act
                var result = useCase.Execute(request, userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(request.Name, result.Name);
                Assert.Equal(request.StartDate, result.StartDate);
                Assert.Equal(request.EndDate, result.EndDate);
                Assert.Equal(request.Country, result.Country);
                Assert.Equal(request.City, result.City);
                Assert.Equal(request.FullAddress, result.FullAddress);
                Assert.Equal(userId, result.UserId);
                Assert.NotNull(result.User);
                Assert.Equal("User1", result.User.Username);
                Assert.Equal("user1@example.com", result.User.Email);
            }
        }
    }
}

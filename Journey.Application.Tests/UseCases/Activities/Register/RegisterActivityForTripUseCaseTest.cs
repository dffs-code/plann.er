using Journey.Application.UseCases.Activities.Register;
using Journey.Communication.Requests;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UnitTests.UseCases.Activities.Register
{
    public class RegisterActivityForTripUseCaseTest
    {
        [Fact]
        public void Execute_ShouldAddActivity_WhenTripExistsAndRequestIsValid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_AddActivity")
                .Options;

            Guid tripId;
            using (var context = new JourneyDbContext(options))
            {
                var user = new User { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com" };
                context.Users.Add(user);
                var trip = new Trip
                {
                    Id = Guid.NewGuid(),
                    Name = "Trip1",
                    StartDate = new DateTime(2024, 8, 1),
                    EndDate = new DateTime(2024, 8, 5),
                    Country = "USA",
                    City = "New York",
                    FullAddress = "123 Main St, New York, NY",
                    User = user
                };
                context.Trips.Add(trip);
                context.SaveChanges();
                tripId = trip.Id;
            }

            var request = new RequestRegisterActivityJson
            {
                Name = "Activity1",
                Date = new DateTime(2024, 8, 2)
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new RegisterActivityForTripUseCase(context);

                // Act
                var result = useCase.Execute(tripId, request);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Activity1", result.Name);
                Assert.Equal(new DateTime(2024, 8, 2), result.Date);
            }
        }

        [Fact]
        public void Execute_ShouldThrowNotFoundException_WhenTripDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_TripNotFound")
                .Options;

            var request = new RequestRegisterActivityJson
            {
                Name = "Activity",
                Date = new DateTime(2024, 8, 2)
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new RegisterActivityForTripUseCase(context);
                var nonExistentTripId = Guid.NewGuid();

                // Act & Assert
                var exception = Assert.Throws<NotFoundException>(() => useCase.Execute(nonExistentTripId, request));
                Assert.Equal(ResourceErrorMessages.TRIP_NOT_FOUND, exception.Message);
            }
        }

        [Fact]
        public void Execute_ShouldThrowErrorOnValidationException_WhenActivityDateIsOutsideTripPeriod()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_InvalidDate")
                .Options;

            Guid tripId;
            using (var context = new JourneyDbContext(options))
            {
                var user = new User { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com" };
                context.Users.Add(user);
                var trip = new Trip
                {
                    Id = Guid.NewGuid(),
                    Name = "Trip1",
                    StartDate = new DateTime(2024, 8, 1),
                    EndDate = new DateTime(2024, 8, 5),
                    Country = "USA",
                    City = "New York",
                    FullAddress = "123 Main St, New York, NY",
                    User = user
                };
                context.Trips.Add(trip);
                context.SaveChanges();
                tripId = trip.Id;
            }

            var request = new RequestRegisterActivityJson
            {
                Name = "Activity",
                Date = new DateTime(2024, 8, 6)
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new RegisterActivityForTripUseCase(context);

                // Act & Assert
                var exception = Assert.Throws<ErrorOnValidationException>(() => useCase.Execute(tripId, request));
                Assert.Contains(ResourceErrorMessages.DATE_NOT_WITHIN_TRAVEL_PERIOD, exception.GetErrorMessages());
            }
        }

        [Fact]
        public void Execute_ShouldThrowErrorOnValidationException_WhenRequestIsInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_InvalidRequest")
                .Options;

            Guid tripId;
            using (var context = new JourneyDbContext(options))
            {
                var user = new User { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com" };
                context.Users.Add(user);
                var trip = new Trip
                {
                    Id = Guid.NewGuid(),
                    Name = "Trip1",
                    StartDate = new DateTime(2024, 8, 1),
                    EndDate = new DateTime(2024, 8, 5),
                    Country = "USA",
                    City = "New York",
                    FullAddress = "123 Main St, New York, NY",
                    User = user
                };
                context.Trips.Add(trip);
                context.SaveChanges();
                tripId = trip.Id;
            }

            var invalidRequest = new RequestRegisterActivityJson
            {
                Name = "", // Invalid because name is empty
                Date = new DateTime(2024, 8, 2)
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new RegisterActivityForTripUseCase(context);

                // Act & Assert
                var exception = Assert.Throws<ErrorOnValidationException>(() => useCase.Execute(tripId, invalidRequest));
                Assert.Contains(ResourceErrorMessages.NAME_EMPTY, exception.GetErrorMessages());
            }
        }
    }
}

using Journey.Application.UseCases.Trips.Update;
using Journey.Communication.Requests;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UnitTests.UseCases.Trips.Update
{
    public class UpdateTripUseCaseTest
    {
        [Fact]
        public void Execute_ShouldUpdateTrip_WhenTripExistsAndUserIsAuthorized()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_UpdateTrip")
                .Options;

            Guid tripId;
            Guid userId;
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
                userId = user.Id;
            }

            var request = new RequestRegisterTripJson
            {
                Name = "Updated Trip",
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2024, 9, 5),
                Country = "Canada",
                City = "Toronto",
                FullAddress = "456 Queen St, Toronto, ON"
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new UpdateTripUseCase(context);

                // Act
                var result = useCase.Execute(tripId, request, userId.ToString());

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Updated Trip", result.Name);
                Assert.Equal("Canada", result.Country);
                Assert.Equal("Toronto", result.City);
                Assert.Equal("456 Queen St, Toronto, ON", result.FullAddress);
            }
        }

        [Fact]
        public void Execute_ShouldThrowNotFoundException_WhenTripDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_TripNotFound")
                .Options;

            var request = new RequestRegisterTripJson
            {
                Name = "Trip",
                StartDate = new DateTime(2024, 8, 1),
                EndDate = new DateTime(2024, 8, 5),
                Country = "USA",
                City = "New York",
                FullAddress = "123 Main St, New York, NY"
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new UpdateTripUseCase(context);
                var nonExistentTripId = Guid.NewGuid();
                var userId = Guid.NewGuid().ToString();

                // Act & Assert
                var exception = Assert.Throws<NotFoundException>(() => useCase.Execute(nonExistentTripId, request, userId));
                Assert.Equal(ResourceErrorMessages.TRIP_NOT_FOUND, exception.Message);
            }
        }

        [Fact]
        public void Execute_ShouldThrowErrorOnValidationException_WhenUserIsUnauthorized()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_UserUnauthorized")
                .Options;

            Guid tripId;
            using (var context = new JourneyDbContext(options))
            {
                var user1 = new User { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com" };
                var user2 = new User { Id = Guid.NewGuid(), Username = "User2", Email = "user2@example.com" };
                context.Users.AddRange(user1, user2);
                var trip = new Trip
                {
                    Id = Guid.NewGuid(),
                    Name = "Trip1",
                    StartDate = new DateTime(2024, 8, 1),
                    EndDate = new DateTime(2024, 8, 5),
                    Country = "USA",
                    City = "New York",
                    FullAddress = "123 Main St, New York, NY",
                    User = user1
                };
                context.Trips.Add(trip);
                context.SaveChanges();
                tripId = trip.Id;
            }

            var request = new RequestRegisterTripJson
            {
                Name = "Updated Trip",
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2024, 9, 5),
                Country = "Canada",
                City = "Toronto",
                FullAddress = "456 Queen St, Toronto, ON"
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new UpdateTripUseCase(context);
                var unauthorizedUserId = Guid.NewGuid().ToString();

                // Act & Assert
                var exception = Assert.Throws<ErrorOnValidationException>(() => useCase.Execute(tripId, request, unauthorizedUserId));
                Assert.Contains(ResourceErrorMessages.USER_UNAUTHORIZED, exception.GetErrorMessages());
            }
        }

        [Fact]
        public void Execute_ShouldUpdateTripWithNoActivities_WhenTripHasNoActivities()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_NoActivities")
                .Options;

            Guid tripId;
            Guid userId;
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
                    User = user,
                    Activities = new List<Activity>()
                };
                context.Trips.Add(trip);
                context.SaveChanges();
                tripId = trip.Id;
                userId = user.Id;
            }

            var request = new RequestRegisterTripJson
            {
                Name = "Updated Trip",
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2024, 9, 5),
                Country = "Canada",
                City = "Toronto",
                FullAddress = "456 Queen St, Toronto, ON"
            };

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new UpdateTripUseCase(context);

                // Act
                var result = useCase.Execute(tripId, request, userId.ToString());

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Updated Trip", result.Name);
                Assert.Equal("Canada", result.Country);
                Assert.Equal("Toronto", result.City);
                Assert.Equal("456 Queen St, Toronto, ON", result.FullAddress);
            }
        }
    }
}

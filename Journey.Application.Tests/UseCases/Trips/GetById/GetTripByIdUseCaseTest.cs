using Journey.Application.UseCases.Trips.GetById;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UnitTests.UseCases.Trips.GetById
{
    public class GetTripByIdUseCaseTest
    {
        [Fact]
        public void Execute_ShouldReturnTrip_WhenTripExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_GetTripById")
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
                    User = user,
                    Activities = new List<Activity>
                {
                    new Activity { Id = Guid.NewGuid(), Name = "Activity1", Date = new DateTime(2024, 8, 2), Status = Infrastructure.Enums.ActivityStatus.Pending },
                    new Activity { Id = Guid.NewGuid(), Name = "Activity2", Date = new DateTime(2024, 8, 3), Status = Infrastructure.Enums.ActivityStatus.Done }
                }
                };
                context.Trips.Add(trip);
                context.SaveChanges();
                tripId = trip.Id;
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetTripByIdUseCase(context);

                // Act
                var result = useCase.Execute(tripId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Trip1", result.Name);
                Assert.Equal("USA", result.Country);
                Assert.Equal("New York", result.City);
                Assert.Equal("123 Main St, New York, NY", result.FullAddress);
                Assert.Equal("user1@example.com", result.User.Email);
                Assert.Equal(2, result.Activities.Count);

                var activity1 = result.Activities.FirstOrDefault(a => a.Name == "Activity1");
                Assert.NotNull(activity1);
                Assert.Equal(Communication.Enums.ActivityStatus.Pending, activity1.Status);

                var activity2 = result.Activities.FirstOrDefault(a => a.Name == "Activity2");
                Assert.NotNull(activity2);
                Assert.Equal(Communication.Enums.ActivityStatus.Done, activity2.Status);
            }
        }

        [Fact]
        public void Execute_ShouldThrowNotFoundException_WhenTripDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_TripNotFound")
                .Options;

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetTripByIdUseCase(context);
                var nonExistentTripId = Guid.NewGuid();

                // Act & Assert
                var exception = Assert.Throws<NotFoundException>(() => useCase.Execute(nonExistentTripId));
                Assert.Equal(ResourceErrorMessages.TRIP_NOT_FOUND, exception.Message);
            }
        }

        [Fact]
        public void Execute_ShouldReturnTripWithNoActivities_WhenTripHasNoActivities()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_NoActivities")
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
                    User = user,
                    Activities = new List<Activity>()
                };
                context.Trips.Add(trip);
                context.SaveChanges();
                tripId = trip.Id;
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetTripByIdUseCase(context);

                // Act
                var result = useCase.Execute(tripId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Trip1", result.Name);
                Assert.Equal("USA", result.Country);
                Assert.Equal("New York", result.City);
                Assert.Equal("123 Main St, New York, NY", result.FullAddress);
                Assert.Equal("user1@example.com", result.User.Email);
                Assert.Empty(result.Activities);
            }
        }
    }
}

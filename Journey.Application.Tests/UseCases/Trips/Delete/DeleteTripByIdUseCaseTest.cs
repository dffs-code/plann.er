
using Journey.Exception.ExceptionsBase;
using Journey.Exception;
using Journey.Infrastructure.Entities;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Journey.Application.UseCases.Trips.Delete;

namespace Journey.Application.UnitTests.UseCases.Trips.Delete
{
    public class DeleteTripByIdUseCaseTest
    {
        [Fact]
        public void Execute_ShouldDeleteTrip_WhenTripExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_DeleteTrip")
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
                var useCase = new DeleteTripByIdUseCase(context);

                // Act
                useCase.Execute(tripId);

                // Assert
                var deletedTrip = context.Trips.FirstOrDefault(t => t.Id == tripId);
                Assert.Null(deletedTrip);
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
                var useCase = new DeleteTripByIdUseCase(context);
                var nonExistentTripId = Guid.NewGuid();

                // Act & Assert
                var exception = Assert.Throws<NotFoundException>(() => useCase.Execute(nonExistentTripId));
                Assert.Equal(ResourceErrorMessages.TRIP_NOT_FOUND, exception.Message);
            }
        }

        [Fact]
        public void Execute_ShouldDeleteTripWithNoActivities_WhenTripHasNoActivities()
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
                var useCase = new DeleteTripByIdUseCase(context);

                // Act
                useCase.Execute(tripId);

                // Assert
                var deletedTrip = context.Trips.FirstOrDefault(t => t.Id == tripId);
                Assert.Null(deletedTrip);
            }
        }
    }
}

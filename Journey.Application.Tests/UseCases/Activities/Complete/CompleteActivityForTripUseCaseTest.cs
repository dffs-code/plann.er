using Journey.Application.UseCases.Activities.Complete;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UnitTests.UseCases.Activities.Complete
{
    public class CompleteActivityForTripUseCaseTest
    {
        [Fact]
        public void Execute_ShouldCompleteActivity_WhenActivityExistsForTrip()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_CompleteActivity")
                .Options;

            Guid tripId;
            Guid activityId;
            using (var context = new JourneyDbContext(options))
            {
                var trip = new Trip
                {
                    Id = Guid.NewGuid(),
                    Name = "Trip1",
                    StartDate = new DateTime(2024, 8, 1),
                    EndDate = new DateTime(2024, 8, 5),
                    Country = "USA",
                    City = "New York",
                    FullAddress = "123 Main St, New York, NY",
                };
                context.Trips.Add(trip);
                var activity = new Activity
                {
                    Id = Guid.NewGuid(),
                    Name = "Activity1",
                    Date = new DateTime(2024, 8, 2),
                    TripId = trip.Id,
                    Status = Infrastructure.Enums.ActivityStatus.Pending
                };
                context.Activities.Add(activity);
                context.SaveChanges();
                tripId = trip.Id;
                activityId = activity.Id;
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new CompleteActivityForTripUseCase(context);

                // Act
                useCase.Execute(tripId, activityId);

                // Assert
                var activity = context.Activities.FirstOrDefault(a => a.Id == activityId);
                Assert.NotNull(activity);
                Assert.Equal(Infrastructure.Enums.ActivityStatus.Done, activity.Status);
            }
        }

        [Fact]
        public void Execute_ShouldThrowNotFoundException_WhenActivityDoesNotExistForTrip()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_ActivityNotFound")
                .Options;

            Guid tripId;
            using (var context = new JourneyDbContext(options))
            {
                var trip = new Trip
                {
                    Id = Guid.NewGuid(),
                    Name = "Trip1",
                    StartDate = new DateTime(2024, 8, 1),
                    EndDate = new DateTime(2024, 8, 5),
                    Country = "USA",
                    City = "New York",
                    FullAddress = "123 Main St, New York, NY",
                };
                context.Trips.Add(trip);
                context.SaveChanges();
                tripId = trip.Id;
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new CompleteActivityForTripUseCase(context);

                // Act & Assert
                var nonExistentActivityId = Guid.NewGuid();
                var exception = Assert.Throws<ErrorOnValidationException>(() => useCase.Execute(tripId, nonExistentActivityId));
                Assert.Contains(ResourceErrorMessages.TRIP_NOT_FOUND, exception.GetErrorMessages());
            }
        }
    }
}

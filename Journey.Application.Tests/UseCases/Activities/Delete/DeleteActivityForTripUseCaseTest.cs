using Journey.Application.UseCases.Activities.Delete;
using Journey.Exception.ExceptionsBase;
using Journey.Exception;
using Journey.Infrastructure.Entities;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journey.Application.UnitTests.UseCases.Activities.Delete
{
    public class DeleteActivityForTripUseCaseTest
    {
        [Fact]
        public void Execute_ShouldDeleteActivity_WhenActivityExistsForTrip()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_DeleteActivity")
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
                    TripId = trip.Id
                };
                context.Activities.Add(activity);
                context.SaveChanges();
                tripId = trip.Id;
                activityId = activity.Id;
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new DeleteActivityForTripUseCase(context);

                // Act
                useCase.Execute(tripId, activityId);

                // Assert
                var activity = context.Activities.FirstOrDefault(a => a.Id == activityId);
                Assert.Null(activity);
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
                var useCase = new DeleteActivityForTripUseCase(context);

                // Act & Assert
                var nonExistentActivityId = Guid.NewGuid();
                var exception = Assert.Throws<ErrorOnValidationException>(() => useCase.Execute(tripId, nonExistentActivityId));
                Assert.Contains(ResourceErrorMessages.TRIP_NOT_FOUND, exception.GetErrorMessages());
            }
        }
    }
}

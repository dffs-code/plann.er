using Journey.Application.UseCases.Activities.GetAllByTripId;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UnitTests.UseCases.Activities.GetAllByTripId
{
    public class GetAllActivitiesByTripIdUseCaseTest
    {
        [Fact]
        public void Execute_ShouldReturnAllActivities_WhenActivitiesExistForTrip()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_GetAllActivities")
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
                context.Activities.AddRange(new List<Activity>
            {
                new Activity { Id = Guid.NewGuid(), Name = "Activity1", Date = new DateTime(2024, 8, 2), TripId = trip.Id },
                new Activity { Id = Guid.NewGuid(), Name = "Activity2", Date = new DateTime(2024, 8, 3), TripId = trip.Id }
            });
                context.SaveChanges();
                tripId = trip.Id;
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetAllActivitiesByTripIdUseCase(context);

                // Act
                var result = useCase.Execute(tripId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Activities.Count);
                Assert.Contains(result.Activities, a => a.Name == "Activity1");
                Assert.Contains(result.Activities, a => a.Name == "Activity2");
            }
        }

        [Fact]
        public void Execute_ShouldReturnEmptyList_WhenNoActivitiesExistForTrip()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_NoActivities")
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
                var useCase = new GetAllActivitiesByTripIdUseCase(context);

                // Act
                var result = useCase.Execute(tripId);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result.Activities);
            }
        }
    }
}

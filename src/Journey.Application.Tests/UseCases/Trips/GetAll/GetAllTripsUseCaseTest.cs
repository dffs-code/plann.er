using Journey.Application.UseCases.Trips.GetAll;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UnitTests.UseCases.Trips.GetAll
{
    public class GetAllTripsUseCaseTest
    {
        [Fact]
        public void Execute_ShouldReturnAllTrips()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_GetAllTrips")
                .Options;

            using (var context = new JourneyDbContext(options))
            {
                var user = new User { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com" };
                context.Users.Add(user);
                context.Trips.AddRange(
                    new Trip { Id = Guid.NewGuid(), Name = "Trip1", StartDate = new DateTime(2024, 8, 1), EndDate = new DateTime(2024, 8, 5), Country = "USA", City = "New York", FullAddress = "123 Main St, New York, NY", User = user },
                    new Trip { Id = Guid.NewGuid(), Name = "Trip2", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 9, 5), Country = "Canada", City = "Toronto", FullAddress = "456 Elm St, Toronto, ON", User = user }
                );
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetAllTripsUseCase(context);

                // Act
                var result = useCase.Execute();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Trips.Count);

                var trip1 = result.Trips.FirstOrDefault(t => t.Name == "Trip1");
                Assert.NotNull(trip1);
                Assert.Equal("user1@example.com", trip1.User.Email);

                var trip2 = result.Trips.FirstOrDefault(t => t.Name == "Trip2");
                Assert.NotNull(trip2);
                Assert.Equal("user1@example.com", trip2.User.Email);
            }
        }

        [Fact]
        public void Execute_ShouldReturnEmptyList_WhenNoTripsExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_NoTrips")
                .Options;

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetAllTripsUseCase(context);

                // Act
                var result = useCase.Execute();

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result.Trips);
            }
        }

        [Fact]
        public void Execute_ShouldReturnAllTrips_WhenManyTripsExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_ManyTrips")
                .Options;

            using (var context = new JourneyDbContext(options))
            {
                var user = new User { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com" };
                context.Users.Add(user);

                var trips = new List<Trip>();
                var baseDate = new DateTime(2024, 8, 1);
                for (int i = 1; i <= 1000; i++)
                {
                    trips.Add(new Trip
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Trip{i}",
                        StartDate = baseDate.AddDays(i),
                        EndDate = baseDate.AddDays(i + 1),
                        Country = "Country",
                        City = "City",
                        FullAddress = $"Address {i}",
                        User = user
                    });
                }
                context.Trips.AddRange(trips);
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetAllTripsUseCase(context);

                // Act
                var result = useCase.Execute();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1000, result.Trips.Count);
            }
        }

        [Fact]
        public void Execute_ShouldHandleTripsWithIncompleteData()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_IncompleteData")
                .Options;

            using (var context = new JourneyDbContext(options))
            {
                var user = new User { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com" };
                context.Users.Add(user);
                context.Trips.AddRange(
                    new Trip { Id = Guid.NewGuid(), Name = "Trip1", StartDate = new DateTime(2024, 8, 1), EndDate = new DateTime(2024, 8, 5), Country = "USA", City = "New York", FullAddress = "123 Main St, New York, NY", User = user },
                    new Trip { Id = Guid.NewGuid(), Name = "Trip2", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 9, 5), User = user } // Incomplete data
                );
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetAllTripsUseCase(context);

                // Act
                var result = useCase.Execute();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Trips.Count);

                var trip1 = result.Trips.FirstOrDefault(t => t.Name == "Trip1");
                Assert.NotNull(trip1);
                Assert.Equal("123 Main St, New York, NY", trip1.FullAddress);

                var trip2 = result.Trips.FirstOrDefault(t => t.Name == "Trip2");
                Assert.NotNull(trip2);
                Assert.Empty(trip2.FullAddress); // Expect null if FullAddress is not set
            }
        }
    }
}

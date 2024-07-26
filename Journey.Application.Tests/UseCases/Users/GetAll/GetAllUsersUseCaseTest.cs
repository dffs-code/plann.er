using Journey.Application.UseCases.Users.GetAll;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;


namespace Journey.Application.UnitTests.UseCases.Users.GetAll
{
    public class GetAllUsersUseCaseTests
    {
        [Fact]
        public void Execute_ShouldReturnAllUsers()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb")
                .Options;

            using (var context = new JourneyDbContext(options))
            {
                context.Users.AddRange(
                    new User { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com" },
                    new User { Id = Guid.NewGuid(), Username = "User2", Email = "user2@example.com" }
                );
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetAllUsersUseCase(context);

                // Act
                var result = useCase.Execute();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Users.Count);

                // Verifique se os valores estão corretos
                var user1 = result.Users.FirstOrDefault(u => u.Username == "User1");
                Assert.NotNull(user1);
                Assert.Equal("user1@example.com", user1.Email);

                var user2 = result.Users.FirstOrDefault(u => u.Username == "User2");
                Assert.NotNull(user2);
                Assert.Equal("user2@example.com", user2.Email);
            }
        }

        [Fact]
        public void Execute_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_NoUsers")
                .Options;

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetAllUsersUseCase(context);

                // Act
                var result = useCase.Execute();

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result.Users);
            }
        }

        [Fact]
        public void Execute_ShouldReturnAllUsers_WhenManyUsersExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_ManyUsers")
                .Options;

            using (var context = new JourneyDbContext(options))
            {
                var users = new List<User>();
                for (int i = 1; i <= 1000; i++)
                {
                    users.Add(new User { Id = Guid.NewGuid(), Username = $"User{i}", Email = $"user{i}@example.com" });
                }
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetAllUsersUseCase(context);

                // Act
                var result = useCase.Execute();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1000, result.Users.Count);
            }
        }

        [Fact]
        public void Execute_ShouldHandleUsersWithIncompleteData()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JourneyDbContext>()
                .UseInMemoryDatabase(databaseName: "JourneyTestDb_IncompleteData")
                .Options;

            using (var context = new JourneyDbContext(options))
            {
                context.Users.AddRange(
                    new User { Id = Guid.NewGuid(), Username = "User1", Email = "user1@example.com" },
                    new User { Id = Guid.NewGuid(), Username = "User2" }  // Email is missing
                );
                context.SaveChanges();
            }

            using (var context = new JourneyDbContext(options))
            {
                var useCase = new GetAllUsersUseCase(context);

                // Act
                var result = useCase.Execute();

                // Assert
                Assert.NotNull(result);

                var user1 = result.Users.FirstOrDefault(u => u.Username == "User1");
                Assert.NotNull(user1);
                Assert.Equal("user1@example.com", user1.Email);

                var user2 = result.Users.FirstOrDefault(u => u.Username == "User2");
                Assert.NotNull(user2);
                Assert.Equal("", user2.Email);
            }
        }

    }
}
using Journey.Communication.Responses;
using Journey.Infrastructure;

namespace Journey.Application.UseCases.Users.GetAll
{
    public class GetAllUsersUseCase(JourneyDbContext dbContext)
    {
        private readonly JourneyDbContext _dbContext = dbContext;

        public ResponseUsersJson Execute()
        {
            var users = _dbContext.Users.ToList();

            return new ResponseUsersJson
            {
                Users = users.Select(user => new ResponseShortUserJson
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                }).ToList(),
            };
        }
    }
}

using Journey.Communication.Responses;
using Journey.Infrastructure;

namespace Journey.Application.UseCases.Users.GetAll
{
    public class GetAllUsersUseCase
    {
        public ResponseUsersJson Execute()
        {
            
            var dbContext = new JourneyDbContext();

            var users = dbContext.Users.ToList();

            return new ResponseUsersJson
            {
                Users = users.Select(user => new ResponseShortUserJson
                {
                    Id = user.Id,
                    Username = user.Username,
                }).ToList(),
            };
        }
    }
}

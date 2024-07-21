using Journey.Communication.Responses;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UseCases.Trips.GetAll
{
    public class GetAllTripsUseCase
    {
        private readonly JourneyDbContext _dbContext;

        public GetAllTripsUseCase(JourneyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ResponseTripsJson Execute()
        {
            var trips = _dbContext.Trips.Include(trip => trip.User).ToList();

            return new ResponseTripsJson
            {
                Trips = trips.Select(trip => new ResponseShortTripJson
                {
                    Id = trip.Id,
                    EndDate = trip.EndDate,
                    StartDate = trip.StartDate,
                    Name = trip.Name,
                    Country = trip.Country,
                    City = trip.City,
                    FullAddress = trip.FullAddress,
                    UserId = trip.User.Id.ToString(),
                    User = new ResponseShortUserJson
                    {
                        Id = trip.User.Id,
                        Email = trip.User.Email,
                        Username = trip.User.Username
                    },
                }).ToList(),
            };
        }
    }
}

using Journey.Communication.Responses;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UseCases.Trips.GetAll
{
    public class GetAllTripsUseCase
    {
        public ResponseTripsJson Execute()
        {
            
            var dbContext = new JourneyDbContext();

            var trips = dbContext.Trips.Include(trip => trip.User).ToList();

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
                    Owner = new ResponseShortUserJson
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

using Journey.Communication.Responses;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UseCases.Trips.GetById
{
    public class GetTripByIdUseCase
    {
        private readonly JourneyDbContext _dbContext;

        public GetTripByIdUseCase(JourneyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ResponseTripJson Execute(Guid id)
        {
            var trip = _dbContext
                .Trips
                .Include(trip => trip.Activities)
                .Include(trip => trip.User)
                .FirstOrDefault(trip => trip.Id == id);

            if(trip is null)
            {
                throw new NotFoundException(ResourceErrorMessages.TRIP_NOT_FOUND);
            }

            return new ResponseTripJson
            {
                Id = trip.Id,
                Name = trip.Name,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Country = trip.Country,
                City = trip.City,
                FullAddress = trip.FullAddress,
                Owner = new ResponseShortUserJson
                {
                    Id = trip.User.Id,
                    Email = trip.User.Email,
                    Username = trip.User.Username
                },
                Activities = trip.Activities.Select(activity => new ResponseActivityJson
                {
                    Id = activity.Id,
                    Name = activity.Name,
                    Date = activity.Date,
                    Status = (Communication.Enums.ActivityStatus)activity.Status
                }).ToList()
            };
        }
    }
}

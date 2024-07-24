using Journey.Communication.Requests;
using Journey.Communication.Responses;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UseCases.Trips.Update
{
    public class UpdateTripUseCase(JourneyDbContext dbContext)
    {
        private readonly JourneyDbContext _dbContext = dbContext;

        public ResponseShortTripJson Execute(Guid tripId, RequestRegisterTripJson request, string userId)
        {
            var trip = _dbContext
                .Trips
                .Include(trip => trip.User)
                .FirstOrDefault(trip => trip.Id == tripId);

            if (trip == null) throw new NotFoundException(ResourceErrorMessages.TRIP_NOT_FOUND);
            if (trip.UserId != Guid.Parse(userId))
            {
                IList<string> errorMessage = [ResourceErrorMessages.USER_UNAUTHORIZED];
                throw new ErrorOnValidationException(errorMessage);
            }

            trip.Name = request.Name;
            trip.StartDate = request.StartDate;
            trip.EndDate = request.EndDate;
            trip.Country = request.Country;
            trip.City = request.City;
            trip.FullAddress = request.FullAddress;

            _dbContext.Trips.Update(trip);
            _dbContext.SaveChanges();

            return new ResponseShortTripJson
            {
                Id = trip.Id,
                Name = request.Name,
                City = request.City,
                Country = request.Country,
                EndDate = request.EndDate,
                StartDate = request.StartDate,
                FullAddress = request.FullAddress,
                UserId = userId,
                User = new ResponseShortUserJson
                {
                    Id = trip.User.Id,
                    Email = trip.User.Email,
                    Username = trip.User.Username
                }
            };
        }
    }
}

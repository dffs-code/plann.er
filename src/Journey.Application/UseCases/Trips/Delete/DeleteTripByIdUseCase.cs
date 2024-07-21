using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Journey.Application.UseCases.Trips.Delete
{
    public class DeleteTripByIdUseCase
    {
        private readonly JourneyDbContext _dbContext;

        public DeleteTripByIdUseCase(JourneyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Execute(Guid id)
        {
            var trip = _dbContext
                .Trips
                .Include(trip => trip.Activities)
                .FirstOrDefault(trip => trip.Id == id);

            if (trip is null)
            {
                throw new NotFoundException(ResourceErrorMessages.TRIP_NOT_FOUND);
            }

            _dbContext.Trips.Remove(trip);

            _dbContext.SaveChanges();
        }
    }
}

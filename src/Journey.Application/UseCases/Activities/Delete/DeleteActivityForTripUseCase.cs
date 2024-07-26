using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;

namespace Journey.Application.UseCases.Activities.Delete
{
    public class DeleteActivityForTripUseCase(JourneyDbContext dbContext)
    {
        private readonly JourneyDbContext _dbContext = dbContext;

        public void Execute(Guid tripId, Guid activityId)
        {
            var activity = _dbContext
                .Activities
                .FirstOrDefault(activity => activity.Id == activityId && activity.TripId == tripId);

            if (activity == null) throw new ErrorOnValidationException([ResourceErrorMessages.TRIP_NOT_FOUND]);

            _dbContext.Activities.Remove(activity);
            _dbContext.SaveChanges();
        }
    }
}

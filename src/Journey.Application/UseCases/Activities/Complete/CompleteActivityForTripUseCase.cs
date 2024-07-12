using Journey.Communication.Responses;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;

namespace Journey.Application.UseCases.Activities.Complete
{
    public class CompleteActivityForTripUseCase
    {
        public void Execute(Guid tripId, Guid activityId)
        {
            var dbContext = new JourneyDbContext();

            var activity = dbContext
                .Activities
                .FirstOrDefault(activity => activity.Id == activityId && activity.TripId == tripId);

            if (activity == null) throw new NotFoundException(ResourceErrorMessages.TRIP_NOT_FOUND);


            activity.Status = Infrastructure.Enums.ActivityStatus.Done;

            dbContext.Activities.Update(activity);
            dbContext.SaveChanges();
        }
    }
}

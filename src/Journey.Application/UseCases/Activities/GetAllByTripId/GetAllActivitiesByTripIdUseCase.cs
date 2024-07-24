using Journey.Communication.Responses;
using Journey.Infrastructure;

namespace Journey.Application.UseCases.Activities.GetAllByTripId
{
    public class GetAllActivitiesByTripIdUseCase(JourneyDbContext dbContext)
    {
        private readonly JourneyDbContext _dbContext = dbContext;

        public ResponseActivitiesJson Execute(Guid tripId)
        {

            var activities = _dbContext.Activities.Where(activity => activity.TripId == tripId).ToList();

            return new ResponseActivitiesJson
            {
                Activities = activities.Select(activity => new ResponseActivityJson
                {
                    Id = activity.Id,
                    Date = activity.Date,
                    Name = activity.Name,
                    Status = (Communication.Enums.ActivityStatus)activity.Status,
                }).ToList(),
            };
        }
    }
}

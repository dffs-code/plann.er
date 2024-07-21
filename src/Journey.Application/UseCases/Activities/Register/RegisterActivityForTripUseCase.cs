using FluentValidation.Results;
using Journey.Communication.Requests;
using Journey.Communication.Responses;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;

namespace Journey.Application.UseCases.Activities.Register
{
    public class RegisterActivityForTripUseCase
    {
        private readonly JourneyDbContext _dbContext;

        public RegisterActivityForTripUseCase(JourneyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ResponseActivityJson Execute(Guid tripId, RequestRegisterActivityJson request)
        {
            var trip = _dbContext
                .Trips
                .FirstOrDefault(trip => trip.Id == tripId);

            Validate(trip, request);

            var entity = new Activity
            {
                Name = request.Name,
                Date = request.Date,
                TripId = tripId,
            };

            _dbContext.Activities.Add(entity);

            _dbContext.SaveChanges();

            return new ResponseActivityJson
            {
                Id = entity.Id,
                Name = request.Name,
                Date = request.Date,
                Status = (Communication.Enums.ActivityStatus)entity.Status
            };
        }

        private void Validate(Trip? trip, RequestRegisterActivityJson request)
        {

            if (trip == null) throw new NotFoundException(ResourceErrorMessages.TRIP_NOT_FOUND);

            var validator = new RegisterActivityValidator();

            var result = validator.Validate(request);

            if(!(request.Date >= trip.StartDate && request.Date <= trip.EndDate))
            {
                result.Errors.Add(new ValidationFailure("Date", ResourceErrorMessages.DATE_NOT_WITHIN_TRAVEL_PERIOD));
            }

            if(!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}

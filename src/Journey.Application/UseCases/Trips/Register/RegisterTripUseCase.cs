using Journey.Communication.Requests;
using Journey.Communication.Responses;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;

namespace Journey.Application.UseCases.Trips.Register
{
    public class RegisterTripUseCase
    {
        private readonly JourneyDbContext _dbContext;

        public RegisterTripUseCase(JourneyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ResponseShortTripJson Execute(RequestRegisterTripJson request)
        {
            Validate(request);

            var entity = new Trip
            {
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Country = request.Country,
                City = request.City,
                FullAddress = request.FullAddress,
            };

            _dbContext.Trips.Add(entity);

            _dbContext.SaveChanges();

            return new ResponseShortTripJson
            {
                EndDate = entity.EndDate,
                StartDate = entity.StartDate,
                Name = entity.Name,
                Id = entity.Id,
                Country = entity.Country,
                City = entity.City,
                FullAddress = entity.FullAddress
            };
        }

        private void Validate(RequestRegisterTripJson request)
        {
            var validator = new RegisterTripValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}

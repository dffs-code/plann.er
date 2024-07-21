using Journey.Application.UseCases.Trips.Delete;
using Journey.Application.UseCases.Trips.GetAll;
using Journey.Application.UseCases.Trips.GetById;
using Journey.Application.UseCases.Trips.Register;
using Journey.Communication.Requests;
using Journey.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Journey.Api.Controllers
{
    [Route("api/trips")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly GetAllTripsUseCase _getAllTripsUseCase;
        private readonly RegisterTripUseCase _registerTripUseCase;
        private readonly DeleteTripByIdUseCase _deleteTripByIdUseCase;
        private readonly GetTripByIdUseCase _getTripByIdUseCase;

        public TripsController(
            GetAllTripsUseCase getAllTripsUseCase, 
            RegisterTripUseCase registerTripUseCase,
            DeleteTripByIdUseCase deleteTripByIdUseCase,
            GetTripByIdUseCase getTripByIdUseCase)
        {
            _getAllTripsUseCase = getAllTripsUseCase;
            _registerTripUseCase = registerTripUseCase;
            _deleteTripByIdUseCase = deleteTripByIdUseCase;
            _getTripByIdUseCase = getTripByIdUseCase;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ResponseShortTripJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public IActionResult Register([FromBody] RequestRegisterTripJson request)
        {
            var response = _registerTripUseCase.Execute(request);
            return Created(string.Empty, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseTripsJson), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var result = _getAllTripsUseCase.Execute();

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseTripJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status404NotFound)]
        public IActionResult GetById([FromRoute]Guid id)
        {
            var response = _getTripByIdUseCase.Execute(id);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseTripJson), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public IActionResult Delete([FromRoute] Guid id)
        {
            _deleteTripByIdUseCase.Execute(id);

            return NoContent();
        }

        
    }
}

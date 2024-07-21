using Journey.Application.UseCases.Activities.Complete;
using Journey.Application.UseCases.Activities.Delete;
using Journey.Application.UseCases.Activities.GetAllByTripId;
using Journey.Application.UseCases.Activities.Register;
using Journey.Communication.Requests;
using Journey.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Journey.Api.Controllers
{
    [Route("api/trips/")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly RegisterActivityForTripUseCase _registerActivityForTripUseCase;
        private readonly GetAllActivitiesByTripIdUseCase _getAllActivitiesByTripIdUseCase;
        private readonly CompleteActivityForTripUseCase _completeActivityForTripUseCase;
        private readonly DeleteActivityForTripUseCase _deleteActivityForTripUseCase;

        [HttpPost]
        [Route("{tripId}/activity")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseActivityJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public IActionResult RegisterActivity(
           [FromRoute] Guid tripId,
           [FromBody] RequestRegisterActivityJson request)
        {
            var response = _registerActivityForTripUseCase.Execute(tripId, request);

            return Created(string.Empty, response);
        }

        [HttpPut]
        [Route("{tripId}/activity/{activityId}/complete")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseActivityJson), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status404NotFound)]
        public IActionResult CompleteActivity(
           [FromRoute] Guid tripId,
           [FromRoute] Guid activityId)
        {
            _completeActivityForTripUseCase.Execute(tripId, activityId);

            return NoContent();
        }

        [HttpDelete]
        [Route("{tripId}/activity/{activityId}/")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseActivityJson), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status404NotFound)]
        public IActionResult DeleteActivity(
           [FromRoute] Guid tripId,
           [FromRoute] Guid activityId)
        {
            _deleteActivityForTripUseCase.Execute(tripId, activityId);

            return NoContent();
        }

        [HttpGet]
        [Route("{tripId}/activities")]
        [ProducesResponseType(typeof(ResponseTripJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status404NotFound)]
        public IActionResult GetActivitiesByTripId([FromRoute] Guid tripId)
        {
            var response = _getAllActivitiesByTripIdUseCase.Execute(tripId);

            return Ok(response);
        }
    }
}

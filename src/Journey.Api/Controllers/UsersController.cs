using Journey.Api.Services;
using Journey.Application.UseCases.Users.AuthenticateUser;
using Journey.Application.UseCases.Users.GetAll;
using Journey.Application.UseCases.Users.Register;
using Journey.Communication.Requests;
using Journey.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Journey.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseShortUserJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] RequestRegisterUserJson request)
        {
            var useCase = new RegisterUserUseCase();

            var response = useCase.Execute(request);
            return Created(string.Empty, response);
        }

        [HttpPost("auth")]
        [ProducesResponseType(typeof(ResponseAuthenticatedUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status400BadRequest)]
        public IActionResult Authenticate([FromBody] RequestRegisterUserJson request)
        {
            var useCase = new AuthenticateUserUseCase();

            var result = useCase.Execute(request.Username, request.Password);

            var token = new TokenService().GenerateToken(result);

            return Ok(new ResponseAuthenticatedUser
            {
                Id = result.Id,
                Username = result.Username,
                Token = token
            });
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseUsersJson), StatusCodes.Status200OK)]
        [Authorize]
        public IActionResult GetAll()
        {
            var useCase = new GetAllUsersUseCase();

            var result = useCase.Execute();

            return Ok(result);
        }
    }
}

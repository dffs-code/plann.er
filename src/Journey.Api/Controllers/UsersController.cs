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
        private readonly AuthenticateUserUseCase _authenticateUserUseCase;
        private readonly GetAllUsersUseCase _getAllUsersUseCase;
        private readonly RegisterUserUseCase _registerUserUseCase;
        
        [HttpPost]
        [ProducesResponseType(typeof(ResponseShortUserJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] RequestRegisterUserJson request)
        {
            var response = _registerUserUseCase.Execute(request);
            return Created(string.Empty, response);
        }

        [HttpPost("auth")]
        [ProducesResponseType(typeof(ResponseAuthenticatedUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorsJson), StatusCodes.Status400BadRequest)]
        public IActionResult Authenticate([FromBody] RequestAuthenticateUserJson request)
        {
            var result = _authenticateUserUseCase.Execute(request.Username, request.Password);

            var token = new TokenService().GenerateToken(result);

            return Ok(new ResponseAuthenticatedUser
            {
                Id = result.Id,
                Username = result.Username,
                Token = token,
                Email = result.Email,
            });
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseUsersJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public IActionResult GetAll()
        {
            var result = _getAllUsersUseCase.Execute();

            return Ok(result);
        }
    }
}

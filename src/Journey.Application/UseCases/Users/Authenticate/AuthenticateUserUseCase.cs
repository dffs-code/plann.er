using Journey.Application.UseCases.Users.Common;
using Journey.Communication.Responses;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;

namespace Journey.Application.UseCases.Users.AuthenticateUser
{
    public  class AuthenticateUserUseCase
    {
        public ResponseShortUserJson Execute(string username, string password)
        {
            var dbContext = new JourneyDbContext();

            var user = dbContext.Users.FirstOrDefault(user => user.Username == username);

            var isPasswordValid = PasswordHandler.VerifyPassword(password, user.Password, user.Salt);

            if (!isPasswordValid)
            {
                IList<string> errorMessage = [ResourceErrorMessages.WRONG_USERNAME_OR_PASSWORD];

                throw new ErrorOnValidationException(errorMessage);
            }

            return new ResponseShortUserJson
            {
                Id = user.Id,
                Username = user.Username,
            };
        }
    }
}

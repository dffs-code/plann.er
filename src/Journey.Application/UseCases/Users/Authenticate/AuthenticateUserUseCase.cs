﻿using Journey.Application.UseCases.Users.Common;
using Journey.Communication.Responses;
using Journey.Exception;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;

namespace Journey.Application.UseCases.Users.AuthenticateUser
{
    public  class AuthenticateUserUseCase
    {
        private readonly JourneyDbContext _dbContext;

        public AuthenticateUserUseCase(JourneyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResponseShortUserJson Execute(string username, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(user => user.Username == username);

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

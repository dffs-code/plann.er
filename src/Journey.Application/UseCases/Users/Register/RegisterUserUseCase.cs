﻿using Journey.Application.UseCases.Users.Common;
using Journey.Communication.Requests;
using Journey.Communication.Responses;
using Journey.Exception.ExceptionsBase;
using Journey.Infrastructure;
using Journey.Infrastructure.Entities;

namespace Journey.Application.UseCases.Users.Register
{
    public class RegisterUserUseCase
    {
        private readonly JourneyDbContext _dbContext;

        public RegisterUserUseCase(JourneyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResponseShortUserJson Execute(RequestRegisterUserJson request)
        {
            Validate(request);

            var salt = PasswordHandler.GenerateSalt();
            var hashedPassword = PasswordHandler.HashPassword(request.Password, salt);

            var entity = new User
            {
                Username = request.Username,
                Password = hashedPassword,
                Salt = salt,
                Email = request.Email,

            };

            _dbContext.Users.Add(entity);

            _dbContext.SaveChanges();

            return new ResponseShortUserJson
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
            };
        }

        private void Validate(RequestRegisterUserJson request)
        {
            var validator = new RegisterUserValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}

using FluentValidation;
using Journey.Communication.Requests;
using Journey.Exception;

namespace Journey.Application.UseCases.Users.Register
{
    public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
    {
        public RegisterUserValidator()
        {
            RuleFor(request => request.Username).NotEmpty().WithMessage(ResourceErrorMessages.USERNAME_EMPTY);
            RuleFor(request => request.Password).NotEmpty().WithMessage(ResourceErrorMessages.PASSWORD_EMPTY);
        }
    }
}

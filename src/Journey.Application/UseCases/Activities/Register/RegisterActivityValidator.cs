using FluentValidation;
using Journey.Communication.Requests;
using Journey.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journey.Application.UseCases.Activities.Register
{
    public class RegisterActivityValidator : AbstractValidator<RequestRegisterActivityJson>
    {
        public RegisterActivityValidator()
        {
            RuleFor(request => request.Name).NotEmpty().WithMessage(ResourceErrorMessages.NAME_EMPTY);
        }
    }
}

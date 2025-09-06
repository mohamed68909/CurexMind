using ClincManagement.API.Contracts.Authentications.Requests;
using FluentValidation;

namespace ClincManagement.API.Validations
{
    public class GoogleSignInRequestValidator : AbstractValidator<GoogleSignInRequest>
    {
        public GoogleSignInRequestValidator()
        {
            RuleFor(x => x.TokenID)
                .NotEmpty()
                .WithMessage("IdToken is required.");
        }
    }

}

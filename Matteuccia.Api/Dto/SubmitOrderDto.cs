using FluentValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace Matteuccia.Api.Dto
{
    public class SubmitOrderDto
    {
        [SwaggerSchema("The customer id")]
        public string CustomerId { get; set; }
    }

    public class SubmitOrderDtoValidator : AbstractValidator<SubmitOrderDto>
    {
        public SubmitOrderDtoValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
        }
    }
}
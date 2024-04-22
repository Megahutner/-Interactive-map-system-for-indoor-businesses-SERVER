using FluentValidation;

namespace UWEServer.Requests
{
    public class ZoneTransfer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class ZoneTransferValidator : AbstractValidator<ZoneTransfer>
    {
        public ZoneTransferValidator()
        {
            RuleFor(m => m.Name).NotEmpty().NotNull().WithErrorCode("000009").WithMessage("Name is not valid.");
            RuleFor(m => m.Name).MaximumLength(100).WithErrorCode("000009").WithMessage("Name must not be more than 100 characters");
        }
    }
}

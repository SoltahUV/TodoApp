using System.ComponentModel.DataAnnotations;
using System.Text;
using Shared.Constraints;

namespace Auth.Dto
{
    public record RegisterDto(
        [Required, StringLength(FieldLimits.UserNameMax, MinimumLength = FieldLimits.UserNameMin)] string Name,
        [Required, StringLength(FieldLimits.PasswordMaxChars, MinimumLength = FieldLimits.PasswordMin)] string Password)
        : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        { 
            if (Password is not null && Encoding.UTF8.GetByteCount(Password) > FieldLimits.PasswordMaxBytes)
                yield return new ValidationResult(
                    $"Password must not exceed {FieldLimits.PasswordMaxBytes} bytes.",
                    [nameof(Password)]);
        }
    }

    public record LoginDto(
        [Required, StringLength(FieldLimits.UserNameMax)] string Name,
        [Required, StringLength(FieldLimits.PasswordMaxChars)] string Password);
}

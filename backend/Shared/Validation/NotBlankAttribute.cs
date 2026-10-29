using System.ComponentModel.DataAnnotations;

namespace Shared.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class NotBlankAttribute : ValidationAttribute
    {
        public NotBlankAttribute()
        {
            ErrorMessage = "The field must not be empty or whitespace.";
        }

        public override bool IsValid(object? value) =>
            value is not string text || !string.IsNullOrWhiteSpace(text);
    }
}

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace XUnit_Tests.Validation;

/// <summary>
/// Повторяет проверку, которую делает MVC для record-DTO:
/// атрибуты берутся с параметров конструктора, а не со свойств.
/// </summary>
internal static class DtoValidator
{
    public static IReadOnlyList<string> InvalidMembers(object dto)
    {
        var type = dto.GetType();
        var constructor = type.GetConstructors().Single();
        var invalid = new List<string>();

        foreach (var parameter in constructor.GetParameters())
        {
            var property = type.GetProperty(parameter.Name!, BindingFlags.Public | BindingFlags.Instance);
            var value = property?.GetValue(dto);
            var attributes = parameter.GetCustomAttributes<ValidationAttribute>().ToArray();

            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateValue(
                value,
                new ValidationContext(dto) { MemberName = parameter.Name },
                results,
                attributes);

            if (!isValid)
                invalid.Add(parameter.Name!);
        }

        if (dto is IValidatableObject validatable)
        {
            foreach (var result in validatable.Validate(new ValidationContext(dto)))
            {
                var members = result.MemberNames.ToArray();
                if (members.Length == 0)
                    invalid.Add("<object>");
                else
                    invalid.AddRange(members);
            }
        }

        return invalid;
    }
}

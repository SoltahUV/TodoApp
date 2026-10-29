using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Auth.Dto;
using BusinessLogic.Dto;

namespace XUnit_Tests.Validation;

public class RecordValidationContractTests
{
    public static TheoryData<Type> AllDtos => new()
    {
        typeof(CreateTaskDto),
        typeof(UpdateTaskDto),
        typeof(PatchTaskDto),
        typeof(TaskDto),
        typeof(CreateNoteDto),
        typeof(UpdateNoteDto),
        typeof(PatchNoteDto),
        typeof(NoteDto),
        typeof(RegisterDto),
        typeof(LoginDto)
    };

    public static TheoryData<Type> InputDtos => new()
    {
        typeof(CreateTaskDto),
        typeof(UpdateTaskDto),
        typeof(PatchTaskDto),
        typeof(CreateNoteDto),
        typeof(UpdateNoteDto),
        typeof(PatchNoteDto),
        typeof(RegisterDto),
        typeof(LoginDto)
    };

    [Theory]
    [MemberData(nameof(AllDtos))]
    public void ValidationAttributesAreNotDeclaredOnProperties(Type dtoType)
    {
        // MVC кидает InvalidOperationException, если атрибут висит на свойстве record.
        var offenders = dtoType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetCustomAttributes<ValidationAttribute>(inherit: true).Any())
            .Select(property => property.Name)
            .ToArray();

        Assert.Empty(offenders);
    }

    [Theory]
    [MemberData(nameof(InputDtos))]
    public void InputDtosDeclareValidationOnConstructorParameters(Type dtoType)
    {
        var constructor = dtoType.GetConstructors().Single();

        Assert.Contains(
            constructor.GetParameters(),
            parameter => parameter.GetCustomAttributes<ValidationAttribute>().Any());
    }

    [Theory]
    [MemberData(nameof(AllDtos))]
    public void EveryConstructorParameterHasMatchingProperty(Type dtoType)
    {
        var constructor = dtoType.GetConstructors().Single();

        foreach (var parameter in constructor.GetParameters())
        {
            var property = dtoType.GetProperty(parameter.Name!);

            Assert.NotNull(property);
            Assert.Equal(parameter.ParameterType, property.PropertyType);
        }
    }
}

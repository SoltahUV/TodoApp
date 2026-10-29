using Shared.Validation;

namespace XUnit_Tests.Validation;

public class NotBlankAttributeTests
{
    private readonly NotBlankAttribute _attribute = new();

    [Theory]
    [InlineData(null)]
    [InlineData("name")]
    public void AllowsMissingOrFilledValue(string? value) =>
        Assert.True(_attribute.IsValid(value));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\n")]
    public void RejectsBlankValue(string value) =>
        Assert.False(_attribute.IsValid(value));
}

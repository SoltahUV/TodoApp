using Auth.Dto;
using Shared.Constraints;

namespace XUnit_Tests.Validation;

public class AuthDtoValidationTests
{
    [Fact]
    public void Register_MinimalPassword_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(
            new RegisterDto("user", new string('p', FieldLimits.PasswordMin))));

    [Fact]
    public void Register_ShortPassword_IsInvalid() =>
        Assert.Contains(nameof(RegisterDto.Password), DtoValidator.InvalidMembers(
            new RegisterDto("user", new string('p', FieldLimits.PasswordMin - 1))));

    [Fact]
    public void Register_PasswordAtCharLimit_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(
            new RegisterDto("user", new string('p', FieldLimits.PasswordMaxChars))));

    [Fact]
    public void Register_PasswordOverCharLimit_IsInvalid() =>
        Assert.Contains(nameof(RegisterDto.Password), DtoValidator.InvalidMembers(
            new RegisterDto("user", new string('p', FieldLimits.PasswordMaxChars + 1))));

    [Theory]
    [InlineData(FieldLimits.PasswordMaxBytes / 2)]
    public void Register_PasswordAtByteLimit_IsValid(int cyrillicChars) =>
        Assert.Empty(DtoValidator.InvalidMembers(
            new RegisterDto("user", new string('ю', cyrillicChars))));

    [Theory]
    [InlineData(FieldLimits.PasswordMaxBytes / 2 + 1)]
    public void Register_PasswordOverByteLimit_IsInvalid(int cyrillicChars) =>
        Assert.Contains(nameof(RegisterDto.Password), DtoValidator.InvalidMembers(
            new RegisterDto("user", new string('ю', cyrillicChars))));

    [Fact]
    public void Register_ShortName_IsInvalid() =>
        Assert.Contains(nameof(RegisterDto.Name), DtoValidator.InvalidMembers(
            new RegisterDto("ab", new string('p', FieldLimits.PasswordMin))));

    [Fact]
    public void Register_NameOverLimit_IsInvalid() =>
        Assert.Contains(nameof(RegisterDto.Name), DtoValidator.InvalidMembers(
            new RegisterDto(new string('n', FieldLimits.UserNameMax + 1), new string('p', FieldLimits.PasswordMin))));

    [Fact]
    public void Login_ShortPassword_IsValid() =>
        Assert.Empty(DtoValidator.InvalidMembers(new LoginDto("user", "short")));

    [Fact]
    public void Login_MissingFields_IsInvalid()
    {
        var invalid = DtoValidator.InvalidMembers(new LoginDto(null!, null!));

        Assert.Contains(nameof(LoginDto.Name), invalid);
        Assert.Contains(nameof(LoginDto.Password), invalid);
    }

    [Fact]
    public void Login_PasswordOverLimit_IsInvalid() =>
        Assert.Contains(nameof(LoginDto.Password), DtoValidator.InvalidMembers(
            new LoginDto("user", new string('p', FieldLimits.PasswordMaxChars + 1))));
}

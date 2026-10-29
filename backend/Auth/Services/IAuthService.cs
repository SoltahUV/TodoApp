namespace Auth.Services
{
    using RegisterDto = Dto.RegisterDto;
    using LoginDto = Dto.LoginDto;
    public interface IAuthService
    {

        Task RegisterAsync(RegisterDto registerDto, CancellationToken cancellation = default);
        Task<string> LoginAsync(LoginDto loginDto, CancellationToken cancellation = default);
    }
}

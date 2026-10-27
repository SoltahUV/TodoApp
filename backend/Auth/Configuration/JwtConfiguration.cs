using System.ComponentModel.DataAnnotations;

namespace Auth.Configuration
{
    public class JwtConfiguration
    {
        public const string SectionName = "JwtConfiguration";

        [Required]
        public string SecretKey { get; set; } = string.Empty;

        [Required]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        public string Audience { get; set; } = string.Empty;

        [Range(5, 1440)]
        public int ExpiresInMinutes { get; set; } = 60;
    }
}

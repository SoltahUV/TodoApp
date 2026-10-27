using System.ComponentModel.DataAnnotations;

namespace DataAccess.Configuration
{
    public class DatabaseConfiguration
    {
        public const string SectionName = "DatabaseConfiguration";

        [Required]
        public string Host { get; set; } = string.Empty;

        [Range(1, 65535)]
        public int Port { get; set; } = 5432;

        [Required]
        public string Database { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

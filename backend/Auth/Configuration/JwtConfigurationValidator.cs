namespace Auth.Configuration
{
    public static class JwtConfigurationValidator
    {
        private const int MinimumSecretLength = 32;
        private const int MinimumDistinctCharacters = 10;

        private static readonly HashSet<string> PlaceholderSecrets = new(StringComparer.OrdinalIgnoreCase)
        {
            "your_secret_key_minimum_32_characters",
            "change_me",
            "change_me_to_a_random_string_of_at_least_32_chars",
            "changeme",
            "secret",
            "password"
        };

        public static void Validate(JwtConfiguration configuration)
        {
            var errors = new List<string>();

            if (!IsStrongSecret(configuration.SecretKey))
                errors.Add(
                    $"SecretKey must be at least {MinimumSecretLength} characters long, contain at least " +
                    $"{MinimumDistinctCharacters} distinct characters and must not be a placeholder. " +
                    "Generate one with 'openssl rand -base64 48' and provide it via the " +
                    "JwtConfiguration__SecretKey environment variable or user-secrets.");

            if (string.IsNullOrWhiteSpace(configuration.Issuer))
                errors.Add("Issuer is required.");

            if (string.IsNullOrWhiteSpace(configuration.Audience))
                errors.Add("Audience is required.");

            if (configuration.ExpiresInMinutes is < 5 or > 1440)
                errors.Add("ExpiresInMinutes must be between 5 and 1440.");

            if (errors.Count > 0)
                throw new InvalidOperationException(
                    "JwtConfiguration is invalid:" +
                    Environment.NewLine + " - " + string.Join(Environment.NewLine + " - ", errors));
        }

        private static bool IsStrongSecret(string? secret)
        {
            if (string.IsNullOrWhiteSpace(secret) || secret.Length < MinimumSecretLength)
                return false;

            if (PlaceholderSecrets.Contains(secret))
                return false;

            return secret.Distinct().Count() >= MinimumDistinctCharacters;
        }
    }
}

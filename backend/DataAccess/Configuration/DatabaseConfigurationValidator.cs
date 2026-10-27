namespace DataAccess.Configuration
{
    public static class DatabaseConfigurationValidator
    {
        public static void Validate(DatabaseConfiguration configuration)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(configuration.Host) || IsPlaceholder(configuration.Host))
                errors.Add("Host is required and must not be a placeholder.");

            if (configuration.Port is < 1 or > 65535)
                errors.Add("Port must be between 1 and 65535.");

            if (string.IsNullOrWhiteSpace(configuration.Database) || IsPlaceholder(configuration.Database))
                errors.Add("Database is required and must not be a placeholder.");

            if (string.IsNullOrWhiteSpace(configuration.Username) || IsPlaceholder(configuration.Username))
                errors.Add("Username is required and must not be a placeholder.");

            if (string.IsNullOrWhiteSpace(configuration.Password) || IsPlaceholder(configuration.Password))
                errors.Add("Password is required and must not be a placeholder.");

            if (errors.Count > 0)
                throw new InvalidOperationException(
                    "DatabaseConfiguration is invalid:" +
                    Environment.NewLine + " - " + string.Join(Environment.NewLine + " - ", errors) +
                    Environment.NewLine +
                    "Configure it via the DatabaseConfiguration__* environment variables or user-secrets.");
        }

        private static bool IsPlaceholder(string value) =>
            value.StartsWith("your_", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("change_me", StringComparison.OrdinalIgnoreCase);
    }
}

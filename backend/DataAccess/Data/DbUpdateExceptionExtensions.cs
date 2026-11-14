using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DataAccess.Data
{
    public static class DbUpdateExceptionExtensions
    {
        public static bool IsUniqueConstraintViolation(
            this DbUpdateException exception,
            string constraintName)
        {
            return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            } postgresException
            && postgresException.ConstraintName == constraintName;
        }
    }
}

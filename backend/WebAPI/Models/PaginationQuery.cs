using System.ComponentModel.DataAnnotations;
using Shared.Constraints;

namespace WebAPI.Models
{
    public sealed class PaginationQuery
    {
        [Range(0, int.MaxValue)]
        public int Skip { get; init; }

        [Range(1, FieldLimits.MaxPageSize)]
        public int Take { get; init; } = FieldLimits.DefaultPageSize;
    }
}

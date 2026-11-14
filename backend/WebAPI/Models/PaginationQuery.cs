using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Shared.Constraints;

namespace WebAPI.Models
{
    public sealed class PaginationQuery
    {
        [FromQuery(Name = "skip")]
        [DefaultValue(0)]
        [Range(0, int.MaxValue)]
        [Description("Количество записей, которые нужно пропустить.")]
        public int Skip { get; init; }

        [FromQuery(Name = "take")]
        [DefaultValue(FieldLimits.DefaultPageSize)]
        [Range(1, FieldLimits.MaxPageSize)]
        [Description("Размер страницы: от 1 до 100.")]
        public int Take { get; init; } = FieldLimits.DefaultPageSize;
    }
}

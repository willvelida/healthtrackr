using Healthtrackr.Api.Activity.Common.Filters;

namespace Healthtrackr.Api.Activity.Services.Interfaces
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter paginationFilter, string route);
    }
}

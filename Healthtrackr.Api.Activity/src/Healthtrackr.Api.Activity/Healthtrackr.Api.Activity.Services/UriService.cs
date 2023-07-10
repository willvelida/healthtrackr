using Healthtrackr.Api.Activity.Common.Filters;
using Healthtrackr.Api.Activity.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;

namespace Healthtrackr.Api.Activity.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetPageUri(PaginationFilter paginationFilter, string route)
        {
            var endpointUri = new Uri(string.Concat(_baseUri, route));
            var modifiedUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "pageNumber", paginationFilter.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", paginationFilter.PageSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}

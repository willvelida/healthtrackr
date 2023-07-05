using Healthtrackr.Api.Activity.Common.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthtrackr.Api.Activity.Services.Interfaces
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter paginationFilter, string route);
    }
}

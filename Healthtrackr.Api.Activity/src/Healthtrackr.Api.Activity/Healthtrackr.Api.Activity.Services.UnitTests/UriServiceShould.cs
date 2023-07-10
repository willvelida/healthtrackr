using FluentAssertions;
using Healthtrackr.Api.Activity.Common.Filters;

namespace Healthtrackr.Api.Activity.Services.UnitTests
{
    public class UriServiceShould
    {
        [Fact]
        public void ReturnProperUri()
        {
            // Arrange
            var baseUri = "http://localhost:5000/";
            var paginationFilter = new PaginationFilter { PageNumber = 1, PageSize = 10 };
            var route = "api/users";

            var uriService = new UriService(baseUri);

            // Act
            var result = uriService.GetPageUri(paginationFilter, route);

            // Assert
            var expectedUri = new Uri("http://localhost:5000/api/users?pageNumber=1&pageSize=10");
            result.Should().Be(expectedUri);
        }
    }
}

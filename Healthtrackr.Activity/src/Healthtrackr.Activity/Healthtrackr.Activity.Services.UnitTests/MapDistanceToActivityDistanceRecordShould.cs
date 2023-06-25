using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Services.Mappers;

namespace Healthtrackr.Activity.Services.UnitTests
{
    public class MapDistanceToActivityDistanceRecordShould
    {
        private readonly IMapper _mapper;

        public MapDistanceToActivityDistanceRecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapDistanceToActivityDistancesRecord());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var distance = fixture.Create<Distance>();

            // ACT
            var expectedActivityDistanceRecord = _mapper.Map<ActivityDistancesRecord>(distance);

            // ASSERT
            using (new AssertionScope())
            {
                expectedActivityDistanceRecord.ActivityType.Should().Be(distance.activity);
                expectedActivityDistanceRecord.Distance.Should().Be(distance.distance);
            }
        }
    }
}

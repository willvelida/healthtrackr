using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Services.Mappers;
using res = Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services.UnitTests
{
    public class MapActivityToActivityRecordShould
    {
        private readonly IMapper _mapper;

        public MapActivityToActivityRecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapActivityToActivityRecord());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var activity = fixture.Create<res.Activity>();

            // ACT
            var expectedActivityRecord = _mapper.Map<ActivityRecord>(activity);

            // ASSERT
            using (new AssertionScope())
            {
                expectedActivityRecord.ActivityName.Should().Be(activity.name);
                expectedActivityRecord.Calories.Should().Be(activity.calories);
                expectedActivityRecord.Duration.Should().Be(activity.duration);
                expectedActivityRecord.Steps.Should().Be(activity.steps);
                expectedActivityRecord.Date.Should().Be(activity.startDate);
                expectedActivityRecord.Time.Should().Be(activity.startTime);
            }
        }
    }
}

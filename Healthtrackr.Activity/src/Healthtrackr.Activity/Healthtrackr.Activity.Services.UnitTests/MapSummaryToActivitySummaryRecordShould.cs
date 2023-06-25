using AutoFixture;
using AutoMapper;
using FluentAssertions.Execution;
using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Services.Mappers;
using res = Healthtrackr.Activity.Common.FitbitResponses;

namespace Healthtrackr.Activity.Services.UnitTests
{
    public class MapSummaryToActivitySummaryRecordShould
    {
        private readonly IMapper _mapper;

        public MapSummaryToActivitySummaryRecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapSummaryToActivitySummaryRecord());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var summary = fixture.Create<res.Summary>();

            // ACT
            var activitySummaryRecord = _mapper.Map<ActivitySummaryRecord>(summary);

            // ASSERT
            using (new AssertionScope())
            {
                Assert.Equal(summary.caloriesOut, activitySummaryRecord.CaloriesBurned);
                Assert.Equal(summary.steps, activitySummaryRecord.Steps);
                Assert.Equal(summary.distances.Where(d => d.activity == "total").Select(d => d.distance).FirstOrDefault(), activitySummaryRecord.Distance);
                Assert.Equal(summary.floors, activitySummaryRecord.Floors);
                Assert.Equal(summary.sedentaryMinutes, activitySummaryRecord.MinutesSedentary);
                Assert.Equal(summary.lightlyActiveMinutes, activitySummaryRecord.MinutesLightlyActive);
                Assert.Equal(summary.fairlyActiveMinutes, activitySummaryRecord.MinutesFairlyActive);
                Assert.Equal(summary.veryActiveMinutes, activitySummaryRecord.MinutesVeryActive);
                Assert.Equal(summary.activityCalories, activitySummaryRecord.ActivityCalories);
            }
        }
    }
}

using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Healthtrackr.Activity.Common.FitbitResponses;
using Healthtrackr.Activity.Common.Models;
using Healthtrackr.Activity.Services.Mappers;

namespace Healthtrackr.Activity.Services.UnitTests
{
    public class MapHeartRateZoneToActivityHeartRateZoneRecordShould
    {
        private readonly IMapper _mapper;

        public MapHeartRateZoneToActivityHeartRateZoneRecordShould()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapHeartRateZoneToActivityHeartRateZonesRecord());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public void MapResponseToRecord()
        {
            // ARRANGE
            var fixture = new Fixture();
            var heartRateZone = fixture.Create<HeartRateZone>();

            // ACT
            var expectedActivityHeartRateZoneRecord = _mapper.Map<ActivityHeartRateZonesRecord>(heartRateZone);

            // ASSERT
            using (new AssertionScope())
            {
                expectedActivityHeartRateZoneRecord.Name.Should().Be(heartRateZone.name);
                expectedActivityHeartRateZoneRecord.Minutes.Should().Be(heartRateZone.minutes);
                expectedActivityHeartRateZoneRecord.MaxHR.Should().Be(heartRateZone.max);
                expectedActivityHeartRateZoneRecord.MinHR.Should().Be(heartRateZone.min);
                expectedActivityHeartRateZoneRecord.CaloriesOut.Should().Be(heartRateZone.caloriesOut);
            }
        }
    }
}

using AutoMapper;
using Recyclarr.TrashLib.Config.Parsing;
using Recyclarr.TrashLib.Config.Services;

namespace Recyclarr.TrashLib.Tests.Config.Parsing;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ConfigYamlMapperProfileTest
{
    private static IMapper CreateMapper()
    {
        return new MapperConfiguration(c => c.AddProfile<ConfigYamlMapperProfile>())
            .CreateMapper();
    }

    [Test]
    public void Profile_quality_null_substitutions()
    {
        var yaml = new QualityProfileQualityConfigYaml
        {
            Enabled = null
        };

        var mapper = CreateMapper();
        var result = mapper.Map<QualityProfileQualityConfig>(yaml);

        result.Enabled.Should().BeTrue();
    }

    [Test]
    public void Profile_null_substitutions()
    {
        var yaml = new QualityProfileConfigYaml
        {
            QualitySort = null
        };

        var mapper = CreateMapper();
        var result = mapper.Map<QualityProfileConfig>(yaml);

        result.QualitySort.Should().Be(QualitySortAlgorithm.Top);
    }

    [Test]
    public void Non_null_reset_unmatched_scores_when_source_is_null()
    {
        var yaml = new QualityProfileConfigYaml();

        var mapper = CreateMapper();
        var result = mapper.Map<QualityProfileConfig>(yaml);

        result.ResetUnmatchedScores.Should().BeEquivalentTo(new ResetUnmatchedScoresConfig());
    }
}

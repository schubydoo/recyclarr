using System.IO.Abstractions;
using Recyclarr.Cli.Console.Commands;
using Recyclarr.Cli.Processors.Config;
using Recyclarr.Common;
using Recyclarr.TrashGuide;
using Recyclarr.TrashLib.TestLibrary;
using Spectre.Console.Testing;

namespace Recyclarr.Cli.Tests.Processors;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class ConfigTemplateListerTest : TrashLibIntegrationFixture
{
    [Test, AutoMockData]
    public void Hidden_templates_are_not_rendered(
        IFileInfo stubFile,
        [Frozen(Matching.ImplementedInterfaces)] TestConsole console,
        [Frozen] IConfigTemplateGuideService guideService,
        ConfigListTemplateProcessor sut)
    {
        guideService.GetTemplateData().Returns(new[]
        {
            new TemplatePath {Id = "r1", TemplateFile = stubFile, Service = SupportedServices.Radarr, Hidden = false},
            new TemplatePath {Id = "r2", TemplateFile = stubFile, Service = SupportedServices.Radarr, Hidden = false},
            new TemplatePath {Id = "s1", TemplateFile = stubFile, Service = SupportedServices.Sonarr, Hidden = false},
            new TemplatePath {Id = "s2", TemplateFile = stubFile, Service = SupportedServices.Sonarr, Hidden = true}
        });

        var settings = Substitute.For<IConfigListTemplatesSettings>();
        settings.Includes.Returns(false);

        sut.Process(settings);

        console.Output.Should().NotContain("s2");
    }
}

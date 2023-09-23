using System.IO.Abstractions;
using Recyclarr.Cli.Console.Settings;
using Recyclarr.Common;
using Recyclarr.Common.Extensions;
using Recyclarr.Platform;
using Recyclarr.TrashLib.ExceptionTypes;

namespace Recyclarr.Cli.Processors.Config;

public class LocalConfigCreator : IConfigCreator
{
    private readonly ILogger _log;
    private readonly IAppPaths _paths;
    private readonly IFileSystem _fs;
    private readonly IResourceDataReader _resources;

    public LocalConfigCreator(ILogger log, IAppPaths paths, IFileSystem fs, IResourceDataReader resources)
    {
        _log = log;
        _paths = paths;
        _fs = fs;
        _resources = resources;
    }

    public bool CanHandle(ICreateConfigSettings settings)
    {
        return true;
    }

    public void Create(ICreateConfigSettings settings)
    {
        var configFile = settings.Path is null
            ? _paths.AppDataDirectory.File("recyclarr.yml")
            : _fs.FileInfo.New(settings.Path);

        if (configFile.Exists)
        {
            throw new FileExistsException(configFile.FullName);
        }

        configFile.CreateParentDirectory();
        using var stream = configFile.CreateText();

        var ymlData = _resources.ReadData("config-template.yml");
        stream.Write(ymlData);

        _log.Information("Created configuration at: {Path}", configFile.FullName);
    }
}

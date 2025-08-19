using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Hocon.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using Concepts.Crud.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length > 0 && args[0].TrimStart('-').Equals("version", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Version             : {typeof(Program).Assembly.GetName().Version?.ToString()}");
            Console.WriteLine($"NameVersion         : {Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine($"FileVersion         : {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}");
            Console.WriteLine($"InformationalVersion: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");
            return;
        }

        var selfDir = Directory.GetCurrentDirectory();
#if DEBUG
        while (!File.Exists(Path.Combine(selfDir, "Concepts.Crud.WebApi", "appconfig.cfg")))
        {
            selfDir = Path.Combine(Directory.GetParent(selfDir)?.FullName);
        }

        selfDir = Path.Combine(selfDir, "Concepts.Crud.WebApi");
#endif

        #region env

        const string ENV_VARIABLES_PREFIX = "WEBAPI_";
        var env = Environment.GetEnvironmentVariable($"{ENV_VARIABLES_PREFIX}ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable($"{ENV_VARIABLES_PREFIX}ENV")
                  ?? "Production";
        Environment.SetEnvironmentVariable($"{ENV_VARIABLES_PREFIX}ENVIRONMENT", env);

        #endregion

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = env
        });

        var cmdLineConf = await ParseCommandLine(args);

        #region Configuration

        var confFiles = cmdLineConf.GetValue<string[]?>("--config");
        if ((confFiles?.Length ?? 0) == 0)
        {
            confFiles = ["appconfig.cfg", $"appconfig.{env}.cfg"];
        }

        UseConfigFiles(confFiles, selfDir, builder);

        builder.Configuration.AddCommandLine(args);
        builder.Configuration.AddEnvironmentVariables(ENV_VARIABLES_PREFIX);

        #endregion

        #region Logging

        var logConfigFilename = cmdLineConf.GetValue<string?>("--logconfig");
        if (string.IsNullOrEmpty(logConfigFilename))
        {
            logConfigFilename = "nlog.config";
        }

        builder
            .Logging
            .AddConfiguration(builder.Configuration.GetSection("Logging"))
            .ClearProviders()
            .AddNLog(logConfigFilename);

        #endregion

        builder
            .Services
            .AddControllers()
            .ConfigureApplicationPartManager(pm =>
            {
                pm.ApplicationParts.Clear();
                pm.ApplicationParts.Add(new AssemblyPart(Assembly.GetExecutingAssembly()));
            });

        builder
            .AddApplicationServices();

        var appUrls = builder.Configuration.GetSection("Http:Urls").Get<string[]>();
        if ((appUrls?.Length ?? 0) == 0)
        {
            appUrls = ["http://*:5000"];
        }

        Debug.Assert(appUrls != null);
        builder
            .WebHost
            .UseUrls(appUrls);

        await using var app = builder.Build();

        app.MapControllers();
        await app.RunAsync();

        static void UseConfigFiles(string[] confFilenames, string selfDir, WebApplicationBuilder builder)
        {
            var appliedConfigs = 0;
            foreach (var confFilename in confFilenames)
            {
                var confFile = confFilename;
                ArgumentException.ThrowIfNullOrEmpty(confFile);

                if (!File.Exists(confFile))
                {
                    if (Path.IsPathRooted(confFile))
                    {
                        continue;
                    }

                    confFile = Path.Join(selfDir, confFile);
                    if (!File.Exists(confFile))
                        continue;
                }

                switch (Path.GetExtension(confFile).ToLowerInvariant())
                {
                    case ".json":
                        builder.Configuration.AddJsonFile(confFile, false);
                        break;
                    case ".cfg":
                    case ".hocon":
                    case "config":
                        builder.Configuration.AddHoconFile(confFile, false);
                        break;
                    default:
                        throw new NotSupportedException($"Config file format [{confFile}] not supported");
                }

                ++appliedConfigs;
            }

            if (appliedConfigs == 0)
            {
                throw new InvalidOperationException("No config files were found");
            }
        }

        static async Task<ParseResult> ParseCommandLine(string[] strings)
        {
            var commandLine = new RootCommand("crud.webapi")
            {
                TreatUnmatchedTokensAsErrors = true
            };
            commandLine.Options.Add(new Option<string?>("--logconfig")
            {
                Description = "Log config filename.",
                Required = false,
            });
            commandLine.Options.Add(new Option<string[]?>("--config")
            {
                Description = "Config filename.",
                Required = false,
                AllowMultipleArgumentsPerToken = true
            });
            var parseResult = commandLine.Parse(strings);
            // if (parseResult.Errors.Count > 0)
            // {
            //     await Console.Error.WriteLineAsync("invalid command line arguments:");
            //     foreach (var error in parseResult.Errors)
            //     {
            //         await Console.Error.WriteLineAsync($"  {error.Message}");
            //     }
            //
            //     Environment.Exit(1);
            // }

            return parseResult;
        }
    }
}
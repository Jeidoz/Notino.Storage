using DocumentsStorage.Web.Services;
using DocumentsStorage.Web.Services.Options;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace DocumentsStorage.Web.Formatters.Config;

public static partial class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Registers a list of configurated Output Formatters for OutputFormattersService and adds singleton of that service 
    /// </summary>
    /// <remarks>Should be called after the all initializations of output formatters (i.e. AddXmlSerializerFormatters() or AddMessagePack()).</remarks>
    public static IServiceCollection AddOutputFormattersService(this IServiceCollection services)
    {
        OutputFormatter?[] initializedOutputFormatters = default;
        services.AddMvcCore(options =>
        {
            initializedOutputFormatters = options.OutputFormatters
                .Select(formatterInterface => formatterInterface as OutputFormatter)
                .SkipWhile(formatter => formatter is null)
                .ToArray();
        });

        services.Configure<OutputFormattersServiceOptions>(options =>
            options.InitializedOutputFormatters = initializedOutputFormatters);
        services.AddSingleton<IOutputFormattersService, OutputFormattersService>();

        return services;
    }
}
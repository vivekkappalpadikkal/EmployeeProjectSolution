using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider p) => _provider = p;

    public void Configure(SwaggerGenOptions opts)
    {
        foreach (var desc in _provider.ApiVersionDescriptions)
        {
            // avoid duplicate-key error
            if (!opts.SwaggerGeneratorOptions.SwaggerDocs.ContainsKey(desc.GroupName))
            {
                opts.SwaggerDoc(desc.GroupName,
                    new OpenApiInfo
                    {
                        Title = $"Employee API {desc.ApiVersion}",
                        Version = desc.GroupName
                    });
            }
        }
    }
}

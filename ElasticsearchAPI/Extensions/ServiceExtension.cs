using ElasticsearchAPI.Configuration;
using Nest;
using Uri = System.Uri;

namespace ElasticsearchAPI.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddElasticsearch(this IServiceCollection service)
    {
        var settings = service.GetOptions<ElasticsearchSettings>(nameof(ElasticsearchSettings));
        var settingElasticsearch = new ConnectionSettings(new Uri(settings.Uri))
            .BasicAuthentication("elastic", "admin1234")
            .PrettyJson()
            .DefaultIndex(settings.DefaultIndex);
        var client = new ElasticClient(settingElasticsearch);
        service.AddSingleton<IElasticClient>(client);
        return service;
    }
}
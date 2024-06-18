using ElasticsearchAPI.Models;
using ElasticsearchAPI.SeedWork;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nest;

namespace ElasticsearchAPI.Services;

public class ElasticSearchService<T> : IElasticSearchService<T> where T : class
{
    private readonly IElasticClient _elasticClient;

    public ElasticSearchService(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task<string> IndexDocumentAsync(string indexName, T document, Func<T, int> idExtractor)
    {
        var request = new IndexRequest<T>(document, indexName, idExtractor(document));
        var response = await _elasticClient.IndexAsync(request);
        if (!response.IsValid)
        {
            throw new Exception($"Failed to index document: {response.OriginalException.Message}");
        }

        return response.Id;
    }

    public async Task<string> UpdateDocumentAsync(string indexName, T document, int documentId)
    {
        var response = await _elasticClient.UpdateAsync<T>(documentId, u => u
            .Index(indexName)
            .Doc(document)
        );

        if (!response.IsValid)
        {
            throw new Exception($"Failed to update index document: {response.OriginalException.Message}");
        }

        return response.Id;
    }

    public async Task<PagedList<T>> SearchAsync(string indexName, PagingRequestParameters pagingRequestParameters,
        string key, string value)
    {
        var response = await _elasticClient.SearchAsync<T>(s => s
            .Index(indexName)
            .Query(q => q
                .QueryString(d => d
                    .Query('*' + value + '*')
                    .Fields(f => f
                        .Field(key)
                    )
                )
            )
            .Size(pagingRequestParameters.PageSize)
        );

        if (!response.IsValid)
        {
            throw new Exception($"Search query failed: {response.OriginalException.Message}");
        }

        var result = response.Hits.Select(hit => hit.Source).AsQueryable();
        var count = response.Total;

        return new PagedList<T>(result, count, pagingRequestParameters.PageIndex, pagingRequestParameters.PageSize);
    }

    public async Task<string> DeleteDocumentAsync(string indexName, int documentId)
    {
        var request = new DeleteRequest<T>(indexName, documentId);
        var response = await _elasticClient.DeleteAsync(request);

        if (!response.IsValid)
        {
            throw new Exception($"Failed to delete document: {response.OriginalException.Message}");
        }

        return response.Result.ToString();
    }
}
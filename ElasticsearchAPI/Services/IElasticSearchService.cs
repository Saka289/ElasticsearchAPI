using ElasticsearchAPI.SeedWork;

namespace ElasticsearchAPI.Services;

public interface IElasticSearchService<T> where T : class
{
    Task<string> IndexDocumentAsync(string indexName, T document, Func<T, int> idExtractor);
    Task<string> UpdateDocumentAsync(string indexName, T document, int documentId);
    Task<PagedList<T>> SearchAsync(string indexName, PagingRequestParameters pagingRequestParameters, string key, string value);
    Task<string> DeleteDocumentAsync(string indexName, int documentId);
}
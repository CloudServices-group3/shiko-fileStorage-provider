using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace shiko_filestorage_provider.OpenApi;

public sealed class OpenApiDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

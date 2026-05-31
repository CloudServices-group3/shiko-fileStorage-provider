namespace shiko_filestorage_provider.OpenApi;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddImageProviderOpenApi (this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<OpenApiDocumentTransformer>();
        });
        return services;
    }
}

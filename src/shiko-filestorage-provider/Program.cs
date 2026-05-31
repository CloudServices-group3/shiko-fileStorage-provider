using Azure.Storage.Blobs;
using shiko_filestorage_provider.Endpoints;
using shiko_filestorage_provider.OpenApi;
using shiko_filestorage_provider.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorsConfiguration();
builder.Services.AddImageProviderOpenApi();

builder.Services.AddSingleton(_ =>
{
    var connectionstring = builder.Configuration.GetConnectionString("AzureBlobStorage")
    ?? throw new InvalidOperationException("AzureBlobStorage connectionstring is missing");

    return new BlobServiceClient(connectionstring);
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("All");
app.UseHttpsRedirection();

app.MapOpenApiEndpoints();
app.MapImageEndpoints();

app.Run();

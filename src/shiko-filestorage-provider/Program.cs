using Azure.Storage.Blobs;
using shiko_filestorage_provider.Endpoints;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddCorsConfiguration();
//builder.Services.AddOpenApiConfiguration();

builder.Services.AddSingleton(_ =>
{
    var connectionstring = builder.Configuration.GetConnectionString("AzureBlobStorage")
    ?? throw new InvalidOperationException("AzureBlobStorage connectionstring is missing");

    return new BlobServiceClient(connectionstring);
});


builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("All");
app.UseHttpsRedirection();

//app.MapOpenApiEndpoints();
app.MapImageEndpoints();

app.Run();

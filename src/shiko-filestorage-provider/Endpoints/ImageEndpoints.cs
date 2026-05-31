using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using shiko_filestorage_provider.Dtos;

namespace shiko_filestorage_provider.Endpoints;

public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/image");

        group.MapPost("/upload", UploadImageAsync)
            .DisableAntiforgery() 
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadImageResult>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> UploadImageAsync(IFormFile file, BlobServiceClient client, IConfiguration configuration, CancellationToken ct = default)
    {
        // Controll if there are any file attatch.
        if (file.Length == 0)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["file"] = ["A file is required."]
            });

        // Controll file format and compare formats.
        var allowedContentType = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/jpg",
            "image/webp",
            "image/png"
        };

        // Controll if the file format compare and if the fileformat is valid.
        if (!allowedContentType.Contains(file.ContentType))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["file"] = ["Only jpeg, jpg, webp and png images are allowed."]
            });

        // Create of the container and upstreams of the image.
        var containerName = configuration["BlobStorageContainers:ImageContainerName"] // From appsettings.json
            ?? throw new InvalidOperationException("BlobStorageContainers:ImageContainerName is missing.");

        // A containerClient of the blob.
        var containerClient = client.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct); // create a container.

        // Imageprovider
        var extension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}"; // create a unique id and fileformat.

        var blobClient = containerClient.GetBlobClient(uniqueFileName);
        await using var stream = file.OpenReadStream();

        // Upload of the file to Blobstorage
        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType,
                CacheControl = "public, max-age=31536000"
            }
        },
        cancellationToken: ct);

        var result = UploadImageResult.Success
            (
                uniqueFileName,
                blobClient.Uri.ToString(),
                file.ContentType,
                file.Length
            );

        return Results.Created(result.Url, result);
    }
}


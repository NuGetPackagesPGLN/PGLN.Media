using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using PGLN.Media.Abstractions;

namespace PGLN.Media.Storage.Azure;

/// <summary>
/// Azure Blob Storage implementation of <see cref="IMediaStorage"/>.
/// </summary>
public class AzureBlobStorage : IMediaStorage
{
    private readonly BlobContainerClient _containerClient;
    private readonly string? _blobPrefix;

    /// <summary>
    /// Initializes a new instance of <see cref="AzureBlobStorage"/>.
    /// </summary>
    /// <param name="options">Configuration options containing connection string and container name.</param>
    /// <exception cref="ArgumentNullException">Thrown if options is null.</exception>
    /// <exception cref="ArgumentException">Thrown if connection string or container name is missing.</exception>
    public AzureBlobStorage(IOptions<AzureBlobOptions> options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrEmpty(options.Value.ConnectionString))
            throw new ArgumentException("Connection string must be provided.", nameof(options));
        if (string.IsNullOrEmpty(options.Value.ContainerName))
            throw new ArgumentException("Container name must be provided.", nameof(options));

        _blobPrefix = options.Value.BlobPrefix?.Trim('/');
        if (!string.IsNullOrEmpty(_blobPrefix) && !_blobPrefix.EndsWith("/"))
            _blobPrefix += "/";

        var blobServiceClient = new BlobServiceClient(options.Value.ConnectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);

        if (options.Value.CreateContainerIfNotExists)
        {
            _containerClient.CreateIfNotExists();
        }
    }

    /// <inheritdoc />
    public async Task<string> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("File name cannot be empty.", nameof(fileName));

        // Generate a unique blob name with optional prefix
        var extension = Path.GetExtension(fileName);
        var blobName = $"{Guid.NewGuid():N}{extension}";
        
        if (!string.IsNullOrEmpty(_blobPrefix))
        {
            blobName = _blobPrefix + blobName;
        }

        var blobClient = _containerClient.GetBlobClient(blobName);

        // Ensure stream is at beginning
        if (stream.CanSeek)
            stream.Position = 0;

        var blobHttpHeaders = new BlobHttpHeaders 
        { 
            ContentType = contentType,
            ContentDisposition = $"inline; filename=\"{fileName}\""
        };

        await blobClient.UploadAsync(stream, blobHttpHeaders, cancellationToken: cancellationToken);

        return blobName;
    }

    /// <inheritdoc />
    public async Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path cannot be empty.", nameof(path));

        var blobClient = _containerClient.GetBlobClient(path);
        
        // Check if blob exists
        var exists = await blobClient.ExistsAsync(cancellationToken);
        if (!exists)
            throw new FileNotFoundException($"Blob not found at path: {path}");

        var response = await blobClient.DownloadAsync(cancellationToken);
        return response.Value.Content;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path cannot be empty.", nameof(path));

        var blobClient = _containerClient.GetBlobClient(path);
        return await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets a URL for the blob (if public access is enabled).
    /// </summary>
    /// <param name="path">Blob path.</param>
    /// <returns>URI of the blob.</returns>
    public Uri GetBlobUri(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentException("Path cannot be empty.", nameof(path));
        
        var blobClient = _containerClient.GetBlobClient(path);
        return blobClient.Uri;
    }
}
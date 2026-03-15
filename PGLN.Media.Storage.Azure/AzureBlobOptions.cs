using System;

namespace PGLN.Media.Storage.Azure;

/// <summary>
/// Configuration options for Azure Blob Storage.
/// </summary>
public class AzureBlobOptions
{
    /// <summary>
    /// Azure Blob Storage connection string.
    /// Format: "DefaultEndpointsProtocol=https;AccountName=youraccount;AccountKey=yourkey;EndpointSuffix=core.windows.net"
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Name of the container to use for storing media files.
    /// Container will be created automatically if it doesn't exist.
    /// </summary>
    public string ContainerName { get; set; } = string.Empty;

    /// <summary>
    /// Optional prefix to add to all blob names (like a folder path).
    /// Example: "uploads/" will store files as "uploads/guid-filename.jpg"
    /// </summary>
    public string? BlobPrefix { get; set; }

    /// <summary>
    /// Whether to create the container if it doesn't exist. Default is true.
    /// </summary>
    public bool CreateContainerIfNotExists { get; set; } = true;
}
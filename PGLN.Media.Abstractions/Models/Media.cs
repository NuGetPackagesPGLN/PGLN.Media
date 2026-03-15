using System;
using System.Collections.Generic;

namespace PGLN.Media.Abstractions.Models;

/// <summary>Represents metadata for an uploaded media file.</summary>
public class Media
{
    /// <summary>Unique identifier for the media record.</summary>
    public int Id { get; set; }

    /// <summary>Original file name as provided by the client.</summary>
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>MIME type of the file (e.g., "image/jpeg").</summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>File size in bytes.</summary>
    public long Size { get; set; }

    /// <summary>Unique path/blob name inside the storage provider.</summary>
    public string StoragePath { get; set; } = string.Empty;

    /// <summary>UTC timestamp of the upload.</summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>Identifier of the user who uploaded the file (if any).</summary>
    public string? UploadedBy { get; set; }

    /// <summary>Current processing status of the media.</summary>
    public MediaStatus Status { get; set; }

    /// <summary>Error message if processing failed; otherwise null.</summary>
    public string? ProcessingError { get; set; }

    /// <summary>Flexible JSON metadata (e.g., dimensions, thumbnail path).</summary>
    public Dictionary<string, string>? Metadata { get; set; }
}
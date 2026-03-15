namespace PGLN.Media.Abstractions.Models;

/// <summary>Represents the processing state of a media file.</summary>
public enum MediaStatus
{
    /// <summary>Uploaded but not yet processed (e.g., thumbnail not generated).</summary>
    Pending = 0,

    /// <summary>All processing completed successfully.</summary>
    Processed = 1,

    /// <summary>Processing failed; see <see cref="Media.ProcessingError"/>.</summary>
    Failed = 2
}
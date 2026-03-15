using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PGLN.Media.Abstractions;

/// <summary>Represents the result of an image processing operation.</summary>
/// <param name="Data">Stream containing the processed image data.</param>
/// <param name="ContentType">MIME type of the processed image.</param>
public record ProcessedImage(Stream Data, string ContentType);

/// <summary>Defines image manipulation operations.</summary>
public interface IImageProcessor
{
    /// <summary>Creates a thumbnail from an image stream.</summary>
    /// <param name="source">Source image stream (position will be reset).</param>
    /// <param name="width">Desired thumbnail width.</param>
    /// <param name="height">Desired thumbnail height.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="ProcessedImage"/> containing the thumbnail.</returns>
    Task<ProcessedImage> CreateThumbnailAsync(Stream source, int width, int height, CancellationToken cancellationToken = default);
}
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PGLN.Media.Abstractions;

/// <summary>Validates uploaded media files for security and compliance.</summary>
public interface IMediaValidator
{
    /// <summary>Validates the file stream, checking size, content type, and filename safety.</summary>
    /// <param name="stream">The file stream to validate. Position will be reset after validation.</param>
    /// <param name="fileName">Original file name (used for extension check).</param>
    /// <param name="providedContentType">Content type sent by the client.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    Task ValidateAsync(Stream stream, string fileName, string providedContentType, CancellationToken cancellationToken = default);
}
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PGLN.Media.Abstractions;

/// <summary>Defines operations for storing and retrieving media files.</summary>
public interface IMediaStorage
{
    /// <summary>Saves a stream to storage and returns the unique storage path.</summary>
    /// <param name="stream">The file stream to save. Must be readable and seekable.</param>
    /// <param name="fileName">Original file name (used to determine extension).</param>
    /// <param name="contentType">MIME type of the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The unique path (blob name) under which the file was saved.</returns>
    Task<string> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a file as a stream.</summary>
    /// <param name="path">Storage path returned by <see cref="SaveAsync"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A readable stream of the file content. The caller must dispose it.</returns>
    Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>Permanently deletes a file from storage.</summary>
    /// <param name="path">Storage path of the file to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the file existed and was deleted; false if it didn't exist.</returns>
    Task<bool> DeleteAsync(string path, CancellationToken cancellationToken = default);
}
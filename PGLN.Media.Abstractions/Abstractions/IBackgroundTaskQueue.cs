using System.Collections.Generic;
using System.Threading;

namespace PGLN.Media.Abstractions;

/// <summary>Represents a request to generate a thumbnail for a specific media.</summary>
/// <param name="MediaId">Identifier of the media to process.</param>
/// <param name="Width">Desired thumbnail width.</param>
/// <param name="Height">Desired thumbnail height.</param>
public record ThumbnailRequest(int MediaId, int Width = 200, int Height = 200);

/// <summary>Simple in‑memory queue for background jobs.</summary>
public interface IBackgroundTaskQueue
{
    /// <summary>Queues a thumbnail generation request.</summary>
    void QueueThumbnailRequest(ThumbnailRequest request);

    /// <summary>Reads all queued requests as an async enumerable (for the background service).</summary>
    IAsyncEnumerable<ThumbnailRequest> ReadAllAsync(CancellationToken cancellationToken = default);
}
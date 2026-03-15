using System.Threading;
using System.Threading.Tasks;
using MediaModel = PGLN.Media.Abstractions.Models.Media;

namespace PGLN.Media.Abstractions;

/// <summary>Provides data access for media metadata.</summary>
public interface IMediaRepository
{
    /// <summary>Adds a new media record.</summary>
    /// <returns>The added media with its generated Id.</returns>
    Task<MediaModel> AddAsync(MediaModel media, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a media record by its Id.</summary>
    /// <returns>The media if found; otherwise null.</returns>
    Task<MediaModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing media record.</summary>
    Task UpdateAsync(MediaModel media, CancellationToken cancellationToken = default);

    /// <summary>Deletes a media record by Id.</summary>
    /// <returns>True if the record was deleted; false if it didn't exist.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
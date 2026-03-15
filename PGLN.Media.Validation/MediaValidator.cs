using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PGLN.Media.Abstractions;
using PGLN.Media.Validation.Core;

namespace PGLN.Media.Validation;

/// <summary>Default implementation of <see cref="IMediaValidator"/> with magic number validation.</summary>
public class MediaValidator : IMediaValidator
{
    private readonly MediaValidationOptions _options;

    /// <summary>Initializes a new instance of <see cref="MediaValidator"/>.</summary>
    /// <param name="options">Validation options.</param>
    public MediaValidator(IOptions<MediaValidationOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public Task ValidateAsync(Stream stream, string fileName, string providedContentType, CancellationToken cancellationToken = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("File name cannot be empty.", nameof(fileName));
        if (!stream.CanSeek) throw new ArgumentException("Stream must be seekable.", nameof(stream));

        // 1. Size check
        if (stream.Length > _options.MaxFileSize)
            throw new InvalidOperationException($"File size {stream.Length} exceeds maximum allowed {_options.MaxFileSize} bytes.");

        string detectedType = null;

        // 2. Magic number detection (if enabled)
        if (_options.EnableSignatureCheck)
        {
            detectedType = FileTypeDetector.DetectContentType(stream);
            if (detectedType == null)
                throw new InvalidOperationException("Unrecognized or unsupported file format.");
        }

        // 3. Check against allowed types
        if (!string.IsNullOrEmpty(detectedType) && !_options.AllowedContentTypes.Contains(detectedType, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException($"File type '{detectedType}' is not allowed. Allowed: {string.Join(", ", _options.AllowedContentTypes)}");

        // 4. Check provided content type matches detected type (if both available)
        if (!string.IsNullOrEmpty(detectedType) && !string.IsNullOrEmpty(providedContentType))
        {
            if (!detectedType.Equals(providedContentType, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Content type mismatch. Client sent '{providedContentType}' but file signature indicates '{detectedType}'.");
        }

        // 5. Filename safety (basic path traversal check)
        if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
            throw new InvalidOperationException("Invalid file name contains path traversal characters.");

        // 6. Extension check (if enabled)
        if (_options.EnableExtensionCheck && !string.IsNullOrEmpty(detectedType))
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            bool validExtension = detectedType switch
            {
                "image/jpeg" => extension is ".jpg" or ".jpeg",
                "image/png" => extension == ".png",
                "image/gif" => extension == ".gif",
                "image/bmp" => extension == ".bmp",
                "image/tiff" => extension is ".tif" or ".tiff",
                "application/pdf" => extension == ".pdf",
                _ => true // Skip check for unknown types
            };

            if (!validExtension)
                throw new InvalidOperationException($"File extension '{extension}' does not match file type '{detectedType}'.");
        }

        return Task.CompletedTask;
    }
}
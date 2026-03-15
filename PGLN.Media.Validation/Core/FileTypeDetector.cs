using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PGLN.Media.Validation.Core;

/// <summary>
/// Detects file MIME types based on magic numbers (file signatures).
/// This class is internal to the validation package.
/// </summary>
internal static class FileTypeDetector
{
    // Dictionary of file signatures (first few bytes) and their corresponding MIME types.
    private static readonly Dictionary<byte[], string> _signatures = new()
    {
        { new byte[] { 0xFF, 0xD8, 0xFF }, "image/jpeg" },
        { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, "image/jpeg" },
        { new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }, "image/jpeg" },
        { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "image/png" },
        { new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, "image/gif" },
        { new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, "image/gif" },
        { new byte[] { 0x42, 0x4D }, "image/bmp" },
        { new byte[] { 0x49, 0x49, 0x2A, 0x00 }, "image/tiff" },
        { new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, "image/tiff" },
        { new byte[] { 0x25, 0x50, 0x44, 0x46 }, "application/pdf" },
        { new byte[] { 0x50, 0x4B, 0x03, 0x04 }, "application/zip" },
        { new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07 }, "application/x-rar-compressed" },
        { new byte[] { 0x7F, 0x45, 0x4C, 0x46 }, "application/x-executable" }
    };

    /// <summary>Reads the file header and returns the detected MIME type, or null if unknown.</summary>
    public static string? DetectContentType(Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (!stream.CanSeek) throw new ArgumentException("Stream must be seekable.", nameof(stream));

        var originalPosition = stream.Position;
        stream.Position = 0;
        
        try
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
            var headerBytes = reader.ReadBytes(16); // Read first 16 bytes for better detection

            foreach (var signature in _signatures)
            {
                if (headerBytes.Take(signature.Key.Length).SequenceEqual(signature.Key))
                    return signature.Value;
            }

            return null;
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }
}
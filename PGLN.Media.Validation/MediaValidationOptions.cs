using System;

namespace PGLN.Media.Validation;

/// <summary>Configuration options for media validation.</summary>
public class MediaValidationOptions
{
    /// <summary>Maximum allowed file size in bytes. Default is 10 MB.</summary>
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024;

    /// <summary>Array of allowed MIME types. Default includes common image types.</summary>
    public string[] AllowedContentTypes { get; set; } = new[] 
    { 
        "image/jpeg", 
        "image/png", 
        "image/gif", 
        "image/bmp", 
        "image/tiff",
        "application/pdf"
    };

    /// <summary>Whether to check file signatures (magic numbers). Default is true.</summary>
    public bool EnableSignatureCheck { get; set; } = true;

    /// <summary>Whether to check file extension against content type. Default is true.</summary>
    public bool EnableExtensionCheck { get; set; } = true;
}
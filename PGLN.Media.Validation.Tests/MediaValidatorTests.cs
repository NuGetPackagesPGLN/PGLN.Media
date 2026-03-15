using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;

namespace PGLN.Media.Validation.Tests;

public class MediaValidatorTests
{
    private readonly MediaValidator _validator;
    private readonly MediaValidationOptions _options;

    public MediaValidatorTests()
    {
        _options = new MediaValidationOptions();
        var optionsWrapper = Options.Create(_options);
        _validator = new MediaValidator(optionsWrapper);
    }

    [Fact]
    public async Task ValidJpeg_ShouldPass()
    {
        // Arrange
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
        using var stream = new MemoryStream(jpegBytes);

        // Act & Assert
        await _validator.ValidateAsync(stream, "test.jpg", "image/jpeg");
    }

    [Fact]
    public async Task ValidPng_ShouldPass()
    {
        // Arrange
        var pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00 };
        using var stream = new MemoryStream(pngBytes);

        // Act & Assert
        await _validator.ValidateAsync(stream, "test.png", "image/png");
    }

    [Fact]
    public async Task InvalidSignature_ShouldThrow()
    {
        // Arrange
        var fakeJpeg = Encoding.UTF8.GetBytes("This is not a JPEG");
        using var stream = new MemoryStream(fakeJpeg);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _validator.ValidateAsync(stream, "fake.jpg", "image/jpeg"));
        Assert.Contains("Unrecognized or unsupported file format", ex.Message);
    }

    [Fact]
    public async Task FileTooLarge_ShouldThrow()
    {
        // Arrange
        _options.MaxFileSize = 100; // 100 bytes max
        var largeFile = new byte[200];
        using var stream = new MemoryStream(largeFile);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _validator.ValidateAsync(stream, "large.jpg", "image/jpeg"));
        Assert.Contains("exceeds maximum allowed", ex.Message);
    }

    [Fact]
    public async Task DisallowedContentType_ShouldThrow()
    {
        // Arrange
        _options.AllowedContentTypes = new[] { "image/png" }; // Only allow PNG
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
        using var stream = new MemoryStream(jpegBytes);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _validator.ValidateAsync(stream, "test.jpg", "image/jpeg"));
        Assert.Contains("not allowed", ex.Message);
    }

    [Fact]
    public async Task ContentTypeMismatch_ShouldThrow()
    {
        // Arrange
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
        using var stream = new MemoryStream(jpegBytes);

        // Act & Assert - Claim it's PNG but it's actually JPEG
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _validator.ValidateAsync(stream, "test.jpg", "image/png"));
        Assert.Contains("Content type mismatch", ex.Message);
    }

    [Fact]
    public async Task InvalidFileName_ShouldThrow()
    {
        // Arrange
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
        using var stream = new MemoryStream(jpegBytes);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _validator.ValidateAsync(stream, "../../test.jpg", "image/jpeg"));
        Assert.Contains("Invalid file name", ex.Message);
    }

    [Fact]
    public async Task ExtensionMismatch_ShouldThrow()
    {
        // Arrange
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
        using var stream = new MemoryStream(jpegBytes);

        // Act & Assert - .png extension but JPEG content
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _validator.ValidateAsync(stream, "test.png", "image/jpeg"));
        Assert.Contains("extension", ex.Message);
    }

    [Fact]
    public async Task SignatureCheckDisabled_ShouldSkipDetection()
    {
        // Arrange
        _options.EnableSignatureCheck = false;
        var textBytes = Encoding.UTF8.GetBytes("This is just text");
        using var stream = new MemoryStream(textBytes);

        // Act & Assert - Should pass because signature check is disabled
        await _validator.ValidateAsync(stream, "test.jpg", "image/jpeg");
    }

    [Fact]
    public async Task NullStream_ShouldThrow()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _validator.ValidateAsync(null, "test.jpg", "image/jpeg"));
    }

    [Fact]
    public async Task EmptyFileName_ShouldThrow()
    {
        // Arrange
        using var stream = new MemoryStream(new byte[10]);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _validator.ValidateAsync(stream, "", "image/jpeg"));
    }
}

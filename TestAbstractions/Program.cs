using PGLN.Media.Abstractions;
using PGLN.Media.Abstractions.Models;

Console.WriteLine("Testing PGLN.Media.Abstractions package:");
Console.WriteLine($"Media type: {typeof(Media).Name}");
Console.WriteLine($"MediaStatus type: {typeof(MediaStatus).Name}");
Console.WriteLine($"IMediaStorage type: {typeof(IMediaStorage).Name}");
Console.WriteLine($"IMediaRepository type: {typeof(IMediaRepository).Name}");
Console.WriteLine($"IImageProcessor type: {typeof(IImageProcessor).Name}");
Console.WriteLine($"IMediaValidator type: {typeof(IMediaValidator).Name}");
Console.WriteLine($"IBackgroundTaskQueue type: {typeof(IBackgroundTaskQueue).Name}");
Console.WriteLine($"ThumbnailRequest type: {typeof(ThumbnailRequest).Name}");

// Try creating instances
var media = new Media
{
    OriginalName = "test.jpg",
    ContentType = "image/jpeg",
    Size = 1024,
    StoragePath = "test/path.jpg",
    UploadedAt = DateTime.UtcNow,
    Status = MediaStatus.Pending
};

Console.WriteLine($"\nCreated test Media: {media.OriginalName} - {media.Status}");

// Create a thumbnail request
var request = new ThumbnailRequest(MediaId: 1, Width: 200, Height: 200);
Console.WriteLine($"Created ThumbnailRequest for MediaId: {request.MediaId}");

Console.WriteLine("\nAll types are accessible! Package is ready for publishing.");
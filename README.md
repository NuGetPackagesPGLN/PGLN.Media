# PGLN.Media.Abstractions

Core interfaces, models, and enums for the PGLN.Media ecosystem.  
This package contains no implementation – it is intended to be shared across all media processing and storage packages.

## Includes
- `Media` and `MediaStatus` models
- Storage abstraction `IMediaStorage`
- Repository abstraction `IMediaRepository`
- Image processing abstraction `IImageProcessor`
- Validation abstraction `IMediaValidator`
- Background task queue abstraction `IBackgroundTaskQueue` and `ThumbnailRequest`

All public APIs are fully documented with XML comments.

## Usage
Reference this package in any project that needs to interact with media without depending on concrete implementations.

```xml
<PackageReference Include="PGLN.Media.Abstractions" Version="1.0.0" />

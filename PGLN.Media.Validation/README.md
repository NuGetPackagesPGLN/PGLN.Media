# PGLN.Media.Validation

File validation for media uploads using magic numbers (file signatures) to ensure security and correctness.

## Features
-  Detects actual file type via magic numbers (not just extension)
-  Enforces maximum file size
-  Restricts allowed MIME types
-  Basic filename safety checks
-  Extension validation against content type
-  Fully configurable options

## Installation
```bash
dotnet add package PGLN.Media.Validation



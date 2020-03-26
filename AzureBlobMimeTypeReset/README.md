# Azure-Blob-MimeType-Reset

Reset Mime Type for file in Azure Blob Storage

Reference: http://www.thepatrickdavis.com/blob-storage-dont-forget-the-mime/

Added features:

- Use .NET Core instead of .NET Framework
- Can reset any known types of files, not just images
- Can use command line parameters
- Deal with extension name case sensitive
- Only update incorrect mime types, not every file

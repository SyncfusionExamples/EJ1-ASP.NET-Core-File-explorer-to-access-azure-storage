# How to Access Azure Storage Files Using the Syncfusion EJ1 ASP.NET Core FileExplorer

## Overview

This sample demonstrates how to access and manage files stored in Azure Blob Storage using the Syncfusion [ASP.NET Core File Manager](https://www.syncfusion.com/aspnet-core-ui-controls/file-manager). The application connects the FileExplorer to an Azure Blob Storage container through a custom storage provider and server-side controller actions. It provides a browser-based interface for navigating and managing files stored in the configured container.

## Key Features

- Uses the Syncfusion EJ1 ASP.NET Core `FileExplorer` to display files and folders stored in Azure Blob Storage.
- Uses `AzureFileOperations`, derived from `BasicFileOperations`, to process Azure storage operations.
- Handles read, folder creation, removal, rename, copy, move, search, and details requests through `FileActionDefault`.
- Uses dedicated `Upload`, `Download`, and `GetImage` actions for file transfers and image previews.
- Restricts displayed files through `FileTypes` to PNG, GIF, JPG, JPEG, and DOCX files.
- Displays files and folders using `LayoutType.LargeIcons`.
- Renders a `900px` by `400px` FileExplorer connected to the configured Azure Blob Storage container.

## Prerequisites

- Visual Studio 2022 or Visual Studio Code
- .NET SDK compatible with the project's target framework
- An Azure Blob Storage account and container with valid access credentials

## How to Run the Project

### Visual Studio 2022

1. Clone or download this repository.
2. Open the solution file:

   `SyncfusionASPNETCoreApplication3.sln`

3. Ensure that the Azure Blob Storage configuration in `Controllers/FileExplorerController.cs` points to a storage account and container that you are authorized to access.
4. Restore the NuGet packages by rebuilding the solution.
5. Set the startup project to:

   `SyncfusionASPNETCoreApplication3`

6. Build the solution.
7. Run the application using `Ctrl+F5`.
8. Open the application URL displayed by Visual Studio after launch.
9. Verify that the FileExplorer displays files and folders from the configured Azure Blob Storage container.

### Visual Studio Code

1. Clone or download this repository.
2. Open the repository folder in Visual Studio Code.
3. Open the integrated terminal.
4. Navigate to the project directory:

```bash
cd EJ1-ASP.NET-Core-File-explorer-to-access-azure-storage
```

5. Ensure that the Azure Blob Storage configuration in `Controllers/FileExplorerController.cs` points to a storage account and container that you are authorized to access.
6. Restore the NuGet packages:

```bash
dotnet restore
```

7. Build the project:

```bash
dotnet build
```

8. Run the project:

```bash
dotnet run
```

9. Open the local URL displayed in the terminal after the application starts.
10. Verify that the FileExplorer displays files and folders from the configured Azure Blob Storage container.

## Project Structure

- `Controllers/FileExplorerController.cs` — handles FileExplorer requests, configures the Azure Blob Storage connection, and delegates file operations to `AzureFileOperations`.
- `Views/FileExplorer/FileExplorerFeatures.cshtml` — configures and renders the Syncfusion EJ1 FileExplorer connected to Azure Blob Storage.

## Support and Feedback

- For general product questions, visit the [Syncfusion Community Forum](https://www.syncfusion.com/forums) or [Syncfusion Support](https://www.syncfusion.com/support).
- To report an issue specific to this sample, open a GitHub issue in this repository.
- Official documentation: [Getting Started with ASP.NET Core File Manager](https://help.syncfusion.com/file-manager-sdk/asp-net-core/getting-started)

## License

This is a Syncfusion sample project provided to demonstrate product usage. Review the [Syncfusion license terms](https://www.syncfusion.com/sales/pricing?category=ui-components) before using Syncfusion components in your own applications.
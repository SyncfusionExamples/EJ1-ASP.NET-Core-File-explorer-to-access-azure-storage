using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.JavaScript.DataVisualization.Models;
using Syncfusion.JavaScript.DataVisualization.Models.Diagram;
using Syncfusion.JavaScript.DataVisualization.DiagramEnums;
using System.Text.RegularExpressions;
using Syncfusion.JavaScript.DataVisualization.Models.Collections;
using Syncfusion.JavaScript;
using Microsoft.AspNetCore.Hosting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace SyncfusionASPNETCoreApplication3.Controllers.Diagram
{
    public partial class FileExplorerController : Controller
    {

        public AzureFileOperations operation;


        // GET: /<controller>/
        public ActionResult FileExplorerFeatures()
        {
            return View();
        }
        public FileExplorerController()
        {
            operation = new AzureFileOperations("filebrowsercontent", "rbAvmn82fmt7oZ7N/3SXQ9+d9MiQmW2i1FzwAtPfUJL9sb2gZ/+cC6Ei1mkwSbMA1iVSy9hzH1unWfL0fPny0A==", "blob1");
        }
        public ActionResult Download(FileExplorerParams args)
        {
            operation.Download(args.Path, args.Names);
            return Json("");
        }
        public ActionResult Upload(FileExplorerParams args)
        {
            operation.Upload(args.FileUpload, args.Path);
            return Json("");
        }
        public ActionResult GetImage(FileExplorerParams args)
        {
            operation.GetImage(args.Path);
            return Json("");
        }
        public ActionResult FileActionDefault([FromBody] FileExplorerParams args)
        {

            string startPath = "https://filebrowsercontent.blob.core.windows.net/blob1/";
            if (args.Path != null)
                args.Path = args.Path.Replace(startPath, "");
            if (args.LocationFrom != null)
                args.LocationFrom = args.LocationFrom.Replace(startPath, "");
            if (args.LocationTo != null)
                args.LocationTo = args.LocationTo.Replace(startPath, "");

            switch (args.ActionType)
            {
                case "Read":
                    return Json(operation.Read(args.Path, args.ExtensionsAllow));
                case "CreateFolder":
                    return Json(operation.CreateFolder(args.Path, args.Name));
                case "Paste":
                    return Json(operation.Paste(args.LocationFrom, args.LocationTo, args.Names, args.Action, args.CommonFiles, args.SelectedItems));
                case "Remove":
                    return Json(operation.Remove(args.Names, args.Path, args.SelectedItems));
                case "Rename":
                    return Json(operation.Rename(args.Path, args.Name, args.NewName, args.CommonFiles, args.SelectedItems));
                case "GetDetails":
                    return Json(operation.GetDetails(args.Path, args.Names, args.SelectedItems));
                case "Search":
                    return Json(operation.Search(args.Path, args.ExtensionsAllow, args.SearchString, args.CaseSensitive));
            }
            return Json("");
        }
    }
}

namespace Syncfusion.JavaScript
{

    public abstract class AzureBasicFileOperations
    {


        public abstract object Read(string path, string filter, IEnumerable<object> selectedItems = null);
        public abstract object CreateFolder(string path, string name, IEnumerable<object> selectedItems = null);
        public abstract object Remove(string[] names, string path, IEnumerable<object> selectedItems = null);

        public abstract object Rename(string path, string oldName, string newName, IEnumerable<CommonFileDetails> commonFiles, IEnumerable<object> selectedItems = null);
        public abstract object Paste(string sourceDir, string targetDir, string[] names, string option, IEnumerable<CommonFileDetails> commonFiles, IEnumerable<object> selectedItems = null, IEnumerable<object> targetFolder = null);

        public abstract void Upload(IEnumerable<IFormFile> files, string path, IEnumerable<object> selectedItems = null);
        public abstract void Download(string path, string[] names, IEnumerable<object> selectedItems = null);

        public abstract object GetDetails(string path, string[] names, IEnumerable<object> selectedItems = null);

        public abstract void GetImage(string path);

        public abstract object Search(string path, string filter, string searchString, bool caseSensitive, IEnumerable<object> selectedItems = null);
    }


    public class AzureFileOperations : AzureBasicFileOperations
    {
        List<FileExplorerDirectoryContent> Items = new List<FileExplorerDirectoryContent>();
        public CloudBlobContainer container;
        public AzureFileOperations(string accountName, string accountKey, string blobName)
        {
            StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);
            CloudBlobClient client = account.CreateCloudBlobClient();
            container = client.GetContainerReference(blobName);
        }

        public static async Task<BlobResultSegment> AsyncReadCall(string path, string oper)
        {
            AzureFileOperations AyncObject = new AzureFileOperations("filebrowsercontent", "rbAvmn82fmt7oZ7N/3SXQ9+d9MiQmW2i1FzwAtPfUJL9sb2gZ/+cC6Ei1mkwSbMA1iVSy9hzH1unWfL0fPny0A==", "blob1");
            CloudBlobDirectory sampleDirectory = AyncObject.container.GetDirectoryReference(path);
            BlobRequestOptions options = new BlobRequestOptions();
            OperationContext context = new OperationContext();
            dynamic Asyncitem = null;
            if (oper == "Read") Asyncitem = await sampleDirectory.ListBlobsSegmentedAsync(false, BlobListingDetails.Metadata, null, null, options, context);
            if (oper == "Paste") Asyncitem = await sampleDirectory.ListBlobsSegmentedAsync(false, BlobListingDetails.None, null, null, options, context);
            if (oper == "Rename") Asyncitem = await sampleDirectory.ListBlobsSegmentedAsync(true, BlobListingDetails.Metadata, null, null, options, context);
            if (oper == "Remove") Asyncitem = await sampleDirectory.ListBlobsSegmentedAsync(true, BlobListingDetails.None, null, null, options, context);
            if (oper == "HasChild") Asyncitem = await sampleDirectory.ListBlobsSegmentedAsync(false, BlobListingDetails.None, null, null, options, context);
            //return Asyncitem;
            return await Task.Run(() =>
            {
                return Asyncitem;
            });
        }

        public override object Read(string path, string filter, IEnumerable<object> selectedItems = null)
        {
            return ReadAsync(path, filter, selectedItems).GetAwaiter().GetResult();
        }
        public async Task<AjaxFileExplorerResponse> ReadAsync(string path, string filter, IEnumerable<object> selectedItems = null)
        {
            OperationContext context = new OperationContext();
            BlobRequestOptions options = new BlobRequestOptions();
            AjaxFileExplorerResponse ReadResponse = new AjaxFileExplorerResponse();
            List<FileExplorerDirectoryContent> details = new List<FileExplorerDirectoryContent>();

            FileExplorerDirectoryContent cwd = new FileExplorerDirectoryContent();
            try
            {
                filter = filter.Replace(" ", "");
                var extensions = (filter ?? "*").Split(",|;".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                CloudBlobDirectory sampleDirectory = container.GetDirectoryReference(path);

                cwd.name = sampleDirectory.Prefix.Replace("/", "");
                cwd.type = "File Folder";
                cwd.size = 0;
                cwd.isFile = sampleDirectory.Uri.IsFile;
                cwd.hasChild = await HasChildDirectory(path);
                cwd.dateModified = sampleDirectory.Container.Properties.LastModified.ToString();
                ReadResponse.cwd = cwd;

                string Oper = "Read";
                var items = await AsyncReadCall(path, Oper);
                foreach (var item in items.Results)
                {
                    bool canAdd = false;
                    if (extensions[0].Equals("*.*") || extensions[0].Equals("*"))
                        canAdd = true;
                    else if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob file = (CloudBlockBlob)item;
                        var names = file.Name.ToString().Trim().Split('.');
                        if (Array.IndexOf(extensions, "*." + names[names.Count() - 1]) >= 0)
                            canAdd = true;
                        else canAdd = false;
                    }
                    else
                        canAdd = true;
                    if (canAdd)
                    {
                        if (item.GetType() == typeof(CloudBlockBlob))
                        {
                            CloudBlockBlob file = (CloudBlockBlob)item;
                            FileExplorerDirectoryContent entry = new FileExplorerDirectoryContent();
                            entry.name = file.Name.Replace(path, "");
                            //entry.type = file.Properties.ContentType;
                            entry.type = "File";
                            entry.isFile = true;
                            entry.size = file.Properties.Length;
                            entry.dateModified = file.Properties.LastModified.Value.LocalDateTime.ToString();
                            entry.hasChild = false;
                            entry.filterPath = "";
                            details.Add(entry);
                        }
                        else if (item.GetType() == typeof(CloudBlobDirectory))
                        {
                            CloudBlobDirectory directory = (CloudBlobDirectory)item;
                            FileExplorerDirectoryContent entry = new FileExplorerDirectoryContent();
                            entry.name = directory.Prefix.Replace(path, "").Replace("/", "");
                            entry.type = "Directory";
                            entry.isFile = false;
                            entry.size = 0;
                            entry.hasChild = await HasChildDirectory(directory.Prefix);
                            entry.filterPath = "";
                            details.Add(entry);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return ReadResponse;
            }
            ReadResponse.files = (IEnumerable<FileExplorerDirectoryContent>)details;

            return ReadResponse;
        }


        public override object CreateFolder(string path, string name, IEnumerable<object> selectedItems = null)
        {

            CreateFolderAsync(path, name, selectedItems).GetAwaiter().GetResult();
            AjaxFileExplorerResponse CreateResponse = new AjaxFileExplorerResponse();
            FileExplorerDirectoryContent content = new FileExplorerDirectoryContent();
            content.name = name;
            var directories = new[] { content };
            CreateResponse.files = (IEnumerable<FileExplorerDirectoryContent>)directories;
            return CreateResponse;
        }

        public async Task CreateFolderAsync(string path, string name, IEnumerable<object> selectedItems = null)
        {
            CloudBlockBlob blob = container.GetBlockBlobReference(path + name + "/temp.$$$");
            await blob.UploadTextAsync(".");
        }


        public override object Paste(string sourceDir, string targetDir, string[] names, string option, IEnumerable<CommonFileDetails> commonFiles, IEnumerable<object> selectedItems = null, IEnumerable<object> targetFolder = null)
        {
            PasteAsync(sourceDir, targetDir, names, option, commonFiles, selectedItems).GetAwaiter().GetResult();
            return "success";
        }

        public async Task PasteAsync(string sourceDir, string targetDir, string[] names, string option, IEnumerable<CommonFileDetails> commonFiles, IEnumerable<object> selectedItems = null, IEnumerable<object> targetFolder = null)
        {
            foreach (var Fileitem in selectedItems)
            {
                Newtonsoft.Json.Linq.JToken myObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken>(Fileitem.ToString());
                AzureFileDirectoryContent s_item = myObject.ToObject<AzureFileDirectoryContent>();
                if (s_item.isFile)
                {
                    string sourceDir1 = sourceDir + s_item.name;
                    CloudBlob existBlob = container.GetBlobReference(sourceDir1);
                    CloudBlob newBlob = container.GetBlobReference(targetDir + s_item.name);
                    await newBlob.StartCopyAsync(existBlob.Uri);
                    if (option == "move")
                        await existBlob.DeleteIfExistsAsync();
                }
                else
                {
                    CloudBlobDirectory sampleDirectory = container.GetDirectoryReference(sourceDir + s_item.name);
                    string Oper = "Paste";
                    var items = await AsyncReadCall(sourceDir + s_item.name, Oper);

                    foreach (var item in items.Results)
                    {
                        string name = item.Uri.ToString().Replace(sampleDirectory.Uri.ToString(), "");
                        CloudBlob newBlob = container.GetBlobReference(targetDir + s_item.name + "/" + name);
                        await newBlob.StartCopyAsync(item.Uri);
                        if (option == "move")
                            await container.GetBlobReference(sourceDir + s_item.name + "/" + name).DeleteAsync();
                    }
                }
            }

        }

        public async Task RenameAsync(string path, string oldName, string newName, IEnumerable<CommonFileDetails> commonFiles, IEnumerable<object> selectedItems = null)
        {
            bool isFile = false;
            foreach (var Fileitem in selectedItems)
            {
                Newtonsoft.Json.Linq.JToken myObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken>(Fileitem.ToString());
                AzureFileDirectoryContent s_item = myObject.ToObject<AzureFileDirectoryContent>();
                isFile = s_item.isFile;
                break;
            }
            if (isFile)
            {
                CloudBlob existBlob = container.GetBlobReference(path + oldName);
                CloudBlob newBlob = container.GetBlobReference(path + newName);
                await newBlob.StartCopyAsync(existBlob.Uri);
                await existBlob.DeleteIfExistsAsync();
            }
            else
            {
                CloudBlobDirectory sampleDirectory = container.GetDirectoryReference(path + oldName);
                var items = await AsyncReadCall(path + oldName, "Rename");
                foreach (var item in items.Results)
                {
                    string name = item.Uri.AbsolutePath.Replace(sampleDirectory.Uri.AbsolutePath, "");
                    CloudBlob newBlob = container.GetBlobReference(path + newName + "/" + name);
                    await newBlob.StartCopyAsync(item.Uri);
                    await container.GetBlobReference(path + oldName + "/" + name).DeleteAsync();
                }

            }

        }

        public override object Rename(string path, string oldName, string newName, IEnumerable<CommonFileDetails> commonFiles, IEnumerable<object> selectedItems = null)
        {
            RenameAsync(path, oldName, newName, commonFiles, selectedItems).GetAwaiter().GetResult();
            return "success";
        }

        public override object Remove(string[] names, string path, IEnumerable<object> selectedItems = null)
        {
            RemoveAsync(names, path, selectedItems).GetAwaiter().GetResult();
            return "success";
        }

        public async Task RemoveAsync(string[] names, string path, IEnumerable<object> selectedItems = null)
        {
            CloudBlobDirectory sampleDirectory = container.GetDirectoryReference(path);
            foreach (var Fileitem in selectedItems)
            {
                Newtonsoft.Json.Linq.JToken myObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken>(Fileitem.ToString());
                AzureFileDirectoryContent s_item = myObject.ToObject<AzureFileDirectoryContent>();

                if (s_item.isFile)
                {
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(path + s_item.name);
                    await blockBlob.DeleteAsync();
                }
                else
                {
                    CloudBlobDirectory subDirectory = container.GetDirectoryReference(path + s_item.name);
                    var items = await AsyncReadCall(path + s_item.name, "Remove");
                    foreach (var item in items.Results)
                    {
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(path + s_item.name + "/" + item.Uri.ToString().Replace(subDirectory.Uri.ToString(), ""));
                        await blockBlob.DeleteAsync();
                    }
                }
            }

        }

        public override void Upload(IEnumerable<IFormFile> files, string path, IEnumerable<object> selectedItems = null)
        {
            UploadAsync(files, path, selectedItems).GetAwaiter().GetResult();
        }



        //public async Task UploadAsync(IEnumerable<IFormFile> files, string path, IEnumerable<object> selectedItems = null)
        //{
        //    string MyPath = path.Replace("https://filebrowsercontent.blob.core.windows.net/blob1/", "");

        //    try
        //    {
        //        foreach (var file in files)
        //        {
        //            CloudBlockBlob blockBlob = container.GetBlockBlobReference(MyPath + file.FileName);

        //            var localPath = @"D:\" + file.FileName;   // <-- Change your files location here 

        //            using (var fileStream = System.IO.File.OpenRead(localPath))
        //            {
        //                await blockBlob.UploadFromStreamAsync(fileStream);
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    { throw ex; }

        //}

        public async Task UploadAsync(IEnumerable<IFormFile> files, string path, IEnumerable<object> selectedItems = null)
        {
            try
            {
                string MyPath = path.Replace("https://filebrowsercontent.blob.core.windows.net/blob1/", "");
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.FileName);                
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(MyPath + file.FileName);
                    await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
                }
            }
            catch (Exception ex) { throw ex; }
        }


        public override void Download(string path, string[] names = null, IEnumerable<object> selectedItems = null)
        {
            DownloadAsync(path, names, selectedItems).GetAwaiter().GetResult();
        }

        public async Task DownloadAsync(string path, string[] names = null, IEnumerable<object> selectedItems = null)
        {
            string MyPath = path.Replace("https://filebrowsercontent.blob.core.windows.net/blob1/", "");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(MyPath + names[0]);
            var localPath = @"D:\" + names[0];  // <-- Change your download target path here
            using (var fileStream = System.IO.File.Create(localPath))
            {
                await blockBlob.DownloadToStreamAsync(fileStream);
            }
        }

        public override object Search(string path, string filter, string searchString, bool caseSensitive, System.Collections.Generic.IEnumerable<object> selectedItems = null)
        {
            Items.Clear();
            AjaxFileExplorerResponse data = (AjaxFileExplorerResponse)Read(path, filter, selectedItems);
            Items.AddRange(data.files);
            getAllFiles(path, data, filter);
            data.files = Items.Where(item => new Regex(WildcardToRegex(searchString), (caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase)).IsMatch(item.name));
            return data;
        }


        public virtual void getAllFiles(string path, AjaxFileExplorerResponse data, string filter)
        {
            AjaxFileExplorerResponse directoryList = new AjaxFileExplorerResponse();
            directoryList.files = (IEnumerable<FileExplorerDirectoryContent>)data.files.Where(item => item.isFile == false);
            for (int i = 0; i < directoryList.files.Count(); i++)
            {

                IEnumerable<FileExplorerDirectoryContent> selectedItem = new[] { directoryList.files.ElementAt(i) };
                AjaxFileExplorerResponse innerData = (AjaxFileExplorerResponse)Read(path + directoryList.files.ElementAt(i).name + "/", filter, selectedItem);
                innerData.files = innerData.files.Select(file => new FileExplorerDirectoryContent
                {
                    name = file.name,
                    type = file.type,
                    isFile = file.isFile,
                    size = file.size,
                    hasChild = file.hasChild,
                    filterPath = (directoryList.files.ElementAt(i).filterPath + directoryList.files.ElementAt(i).name + "\\")
                });
                Items.AddRange(innerData.files);
                getAllFiles(path + directoryList.files.ElementAt(i).name + "/", innerData, filter);
            }
        }


        public virtual string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }

        private async Task<bool> HasChildDirectory(string path)
        {
            CloudBlobDirectory sampleDirectory = container.GetDirectoryReference(path);
            var items = await AsyncReadCall(path, "HasChild");
            foreach (var item in items.Results)
            {
                if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    return true;
                }
            }
            return false;
        }


        public override object GetDetails(string path, string[] names, IEnumerable<object> selectedItems = null)
        {
            return GetDetailsAsync(path, names, selectedItems).GetAwaiter().GetResult();

        }

        public async Task<FileExplorerResponse> GetDetailsAsync(string path, string[] names, IEnumerable<object> selectedItems = null)
        {
            FileExplorerResponse DetailsResponse = new FileExplorerResponse();
            try
            {
                bool isFile = false;
                FileDetails[] fDetails = new FileDetails[names.Length];
                FileDetails fileDetails = new FileDetails();
                if (selectedItems != null)
                {
                    foreach (var Fileitem in selectedItems)
                    {
                        Newtonsoft.Json.Linq.JToken myObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken>(Fileitem.ToString());
                        AzureFileDirectoryContent s_item = myObject.ToObject<AzureFileDirectoryContent>();
                        isFile = s_item.isFile;
                        break;
                    }
                }
                if (isFile)
                {
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(path + names[0]);
                    await blockBlob.FetchAttributesAsync();
                    fileDetails.Name = blockBlob.Name;
                    fileDetails.Type = blockBlob.Name.Split('.')[1];
                    fileDetails.Location = blockBlob.Uri.ToString();
                    fileDetails.Size = blockBlob.Properties.Length;
                    fileDetails.Modified = blockBlob.Properties.LastModified.Value.LocalDateTime.ToString();
                }
                else
                {
                    CloudBlobDirectory sampleDirectory = container.GetDirectoryReference(path);
                    fileDetails.Name = names[0];
                    fileDetails.Location = sampleDirectory.Uri.ToString() + "/" + names[0];
                    fileDetails.Type = "File Folder";
                    fileDetails.Modified = sampleDirectory.Container.Properties.LastModified.ToString();
                }
                fDetails[0] = fileDetails;
                DetailsResponse.details = fDetails;

                return await Task.Run(() =>
                {
                    return DetailsResponse;
                });
            }
            catch (Exception ex) { throw ex; }
        }

        public override void GetImage(string path)
        {
            throw new NotImplementedException();
        }

        public class AjaxFileExplorerResponse
        {
            public FileExplorerDirectoryContent cwd { get; set; }
            public IEnumerable<FileExplorerDirectoryContent> files { get; set; }
            public IEnumerable<AzureFileDetails> details { get; set; }
            public object error { get; set; }
        }

        public class AzureFileDetails
        {
            public string CreationTime { get; set; }
            public string Extension { get; set; }
            public string Format { get; set; }
            public string FullName { get; set; }
            public string LastAccessTime { get; set; }
            public string LastWriteTime { get; set; }
            public long Length { get; set; }
            public string Name { get; set; }
        }

        public class AzureFileDirectoryContent
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string name { get; set; }
            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>The type.</value>
            public string type { get; set; }
            /// <summary>
            /// Gets or sets the size.
            /// </summary>
            /// <value>The size.</value>
            public string size { get; set; }
            /// <summary>
            /// Gets or sets the modified date.
            /// </summary>
            /// <value>The modified date.</value>
            public string dateModified { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether this instance has child.
            /// </summary>
            /// <value><c>true</c> if this instance has child; otherwise, <c>false</c>.</value>
            public bool hasChild { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether this instance is file.
            /// </summary>
            /// <value><c>true</c> if this instance is file; otherwise, <c>false</c>.</value>
            public bool isFile { get; set; }
            /// <summary>
            /// Gets or sets the filter path.
            /// </summary>
            /// <value>The filter path.</value>
            public string filterPath { get; set; }
            /// <summary>
            /// Gets or sets the access permission.
            /// </summary>
            /// <value>The access rule.</value>
            public FileAccessRules permission { get; set; }

            public long sizeInByte { get; set; }

            public string cssClass { get; set; }

        }
    }
}
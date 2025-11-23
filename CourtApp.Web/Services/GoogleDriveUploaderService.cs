using CourtApp.Application.Interfaces.Shared;
using CourtApp.Web.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Services
{
    public class GoogleDriveUploaderService : IDocumentUploadService
    {
        private readonly UploadSettings _settings;
        private readonly IWebHostEnvironment _environment;
        public GoogleDriveUploaderService(IOptions<UploadSettings> options,
     IWebHostEnvironment environment)
        {
            _settings = options.Value;
            _environment = environment;

        }
        public async Task<string> UploadFileAsync(Stream zipStream, string zipFileName, string documentType)
        {
            var folderName = documentType switch
            {
                "Draft" => _settings.Folders["DraftDocuments"],
                "Order" => _settings.Folders["OrderDocuments"],
                "ProfileImage" => _settings.Folders["ProfileImages"],
                _ => throw new ArgumentException("Invalid document type")
            };
            //var jsonKeyPath = Path.Combine(_environment.WebRootPath, "service-account-key.json");
            var jsonKeyPath = Environment.GetEnvironmentVariable("GOOGLE_SERVICE_ACCOUNT_KEY");
            GoogleCredential credential;
            if (string.IsNullOrEmpty(jsonKeyPath))
            {
                throw new InvalidOperationException("Environment variable 'GOOGLE_SERVICE_ACCOUNT_KEY' is not set.");
            }

             credential = GoogleCredential
                .FromJson(jsonKeyPath)
                .CreateScoped(DriveService.Scope.Drive);
            //using (var stream = new FileStream(jsonKeyPath, FileMode.Open, FileAccess.Read))
            //{
            //    credential = GoogleCredential.FromStream(stream)
            //        .CreateScoped(DriveService.Scope.Drive);
            //}

            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "DriveUploader"
            });

            var subFolderId = await GetOrCreateFolderAsync(service, folderName, _settings.GoogleDrive.BaseFolderId);
            if (subFolderId == null) throw new Exception("Failed to access or create target folder.");

            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = zipFileName,
                Parents = new List<string> { subFolderId }
            };

            string contentType = documentType == "ProfileImage"
                ? GetContentType(zipFileName)
                : "application/zip";


            var request = service.Files.Create(fileMetadata, zipStream, contentType);
            request.Fields = "id, name, webViewLink";

            var result = await request.UploadAsync();
            if (result.Status == Google.Apis.Upload.UploadStatus.Completed)
            {
                var uploadedFile = request.ResponseBody;
                string fileId = uploadedFile.Id;
                return fileId;
            }
            else
            {
                throw new Exception($"Upload failed: {result.Exception?.Message}");
            }
        }

        private async Task<string> GetOrCreateFolderAsync(DriveService service, string folderName, string parentFolderId)
        {
            try
            {
                // 🔍 Check if subfolder exists
                var listRequest = service.Files.List();
                listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{folderName}' and '{parentFolderId}' in parents and trashed = false";
                listRequest.Fields = "files(id, name)";
                var result = await listRequest.ExecuteAsync();

                if (result.Files.Count > 0)
                {
                    Console.WriteLine("📁 Subfolder exists.");
                    return result.Files.First().Id;
                }

                // 📁 Create subfolder
                var folderMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = folderName,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new List<string> { parentFolderId }
                };

                var createRequest = service.Files.Create(folderMetadata);
                createRequest.Fields = "id";
                var folder = await createRequest.ExecuteAsync();

                Console.WriteLine("✅ Subfolder created.");
                return folder.Id;
            }
            catch (Google.GoogleApiException gex)
            {
                Console.WriteLine($"Google API error: {gex.Message}");
                throw;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Folder existence check timed out.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }

        public async Task<bool> DeleteFileAsync(string fileId)
        {
            try
            {
                
                var jsonKeyPath = Environment.GetEnvironmentVariable("GOOGLE_SERVICE_ACCOUNT_KEY");
                GoogleCredential credential;
                if (string.IsNullOrEmpty(jsonKeyPath))
                {
                    throw new InvalidOperationException("Environment variable 'GOOGLE_SERVICE_ACCOUNT_KEY' is not set.");
                }

                credential = GoogleCredential
                   .FromJson(jsonKeyPath)
                   .CreateScoped(DriveService.Scope.Drive);
                //GoogleCredential credential;
                //using (var stream = new FileStream(jsonKeyPath, FileMode.Open, FileAccess.Read))
                //{
                //    credential = GoogleCredential.FromStream(stream)
                //        .CreateScoped(DriveService.Scope.Drive);
                //}

                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "DriveUploader"
                });
                var deleteRequest = service.Files.Delete(fileId);
                await deleteRequest.ExecuteAsync();
                return true;
            }
            catch (Google.GoogleApiException gex)
            {
                Console.WriteLine($"Google API error during delete: {gex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during delete: {ex.Message}");
                return false;
            }
        }
    }


}

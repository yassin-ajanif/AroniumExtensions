using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;

namespace AroniumFactures.Services;

public class GoogleDriveConnectionService : IGoogleDriveConnectionService
{
    private static readonly string TokenStorePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AroniumFactures",
        "google-token"
    );

    private static readonly string CredentialsPath = Path.Combine(
        AppContext.BaseDirectory,
        "secret_desktop_googledrive_key.json"
    );

    private static readonly string[] Scopes = { DriveService.Scope.DriveFile, "openid", "email" };
    private const string ApplicationName = "AroniumFactures";
    
    // IMPORTANT: Use a unique ID for the local store so 12,000 users don't overwrite each other
    private const string UserId = "default_user"; 
    
    private const string AuditFolderName = "Aronium Audit";
    private const string AuditFileName = "aronium-auditlog.csv.gz";
    private const string AuditFileMimeType = "application/gzip";

    private UserCredential? _credential;

    public async Task ConnectAsync()
    {
        _credential = await GetCredentialAsync(forceRefresh: true);
    }

    public async Task<bool> IsConnectedAsync()
    {
        try
        {
            var store = new Google.Apis.Util.Store.FileDataStore(TokenStorePath, fullPath: true);
            var token = await store.GetAsync<Google.Apis.Auth.OAuth2.Responses.TokenResponse>(UserId);
            return token != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetConnectedEmailAsync()
    {
        try
        {
            var credential = await GetCredentialAsync();
            if (credential == null) return null;

            var tokenResponse = credential.Token;
            if (string.IsNullOrEmpty(tokenResponse?.IdToken)) return null;

            var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(
                tokenResponse.IdToken,
                new Google.Apis.Auth.GoogleJsonWebSignature.ValidationSettings { ForceGoogleCertRefresh = false }
            );

            return payload?.Email;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GoogleDrive] Failed to get email: {ex.Message}");
            return null;
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            var store = new Google.Apis.Util.Store.FileDataStore(TokenStorePath, fullPath: true);
            await store.DeleteAsync<Google.Apis.Auth.OAuth2.Responses.TokenResponse>(UserId);
            _credential = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disconnecting: {ex.Message}");
        }
    }

    public async Task<string?> UploadCsvToDriveAsync(string localFilePath)
    {
        if (string.IsNullOrEmpty(localFilePath) || !File.Exists(localFilePath))
            return null;

        try
        {
            var credential = await GetCredentialAsync();
            if (credential == null) return null;

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
                HttpClientTimeout = TimeSpan.FromMinutes(30)
            });

            // 1. Resolve Folder
            var folderId = await GetOrCreateAuditFolderIdAsync(driveService);
            if (string.IsNullOrEmpty(folderId)) return null;

            // 2. Resolve File
            var existingFileId = await FindFileIdInFolderAsync(driveService, folderId, AuditFileName);
            
            using var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            if (!string.IsNullOrEmpty(existingFileId))
            {
                // UPDATE
                var updateRequest = driveService.Files.Update(new Google.Apis.Drive.v3.Data.File(), existingFileId, fileStream, AuditFileMimeType);
                var progress = await updateRequest.UploadAsync();

                if (progress.Status == UploadStatus.Failed)
                    throw progress.Exception;

                return existingFileId;
            }
            else
            {
                // CREATE
                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = AuditFileName,
                    Parents = new List<string> { folderId },
                    MimeType = AuditFileMimeType
                };

                var createRequest = driveService.Files.Create(fileMetadata, fileStream, AuditFileMimeType);
                createRequest.Fields = "id";
                var progress = await createRequest.UploadAsync();

                if (progress.Status == UploadStatus.Failed)
                    throw progress.Exception;

                // After UploadAsync, the ResponseBody is populated in the request object
                return createRequest.ResponseBody?.Id;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GoogleDrive] Upload failed: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> DownloadAuditFolderFileContentAsync(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return null;
        try
        {
            var credential = await GetCredentialAsync();
            if (credential == null) return null;

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
                HttpClientTimeout = TimeSpan.FromMinutes(5)
            });

            var folderId = await GetOrCreateAuditFolderIdAsync(driveService);
            if (string.IsNullOrEmpty(folderId)) return null;

            var fileId = await FindFileIdInFolderAsync(driveService, folderId, fileName);
            if (string.IsNullOrEmpty(fileId)) return null;

            await using var memoryStream = new MemoryStream();
            var getRequest = driveService.Files.Get(fileId);
            await getRequest.DownloadAsync(memoryStream);
            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream);
            return await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GoogleDrive] Download failed ({fileName}): {ex.Message}");
            return null;
        }
    }

    private static async Task<string?> GetOrCreateAuditFolderIdAsync(DriveService driveService)
    {
        var listRequest = driveService.Files.List();
        listRequest.Q = $"name = '{AuditFolderName}' and mimeType = 'application/vnd.google-apps.folder' and 'root' in parents and trashed = false";
        listRequest.Fields = "files(id, name)";
        listRequest.Spaces = "drive";
        
        var result = await listRequest.ExecuteAsync();
        var folder = result.Files?.FirstOrDefault(); // Q filter already handles the name
        
        if (folder != null)
            return folder.Id;

        var folderMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = AuditFolderName,
            MimeType = "application/vnd.google-apps.folder"
        };
        
        var createRequest = driveService.Files.Create(folderMetadata);
        createRequest.Fields = "id";
        var created = await createRequest.ExecuteAsync();
        return created?.Id;
    }

    private static async Task<string?> FindFileIdInFolderAsync(DriveService driveService, string folderId, string fileName)
    {
        var listRequest = driveService.Files.List();
        listRequest.Q = $"name = '{fileName}' and '{folderId}' in parents and trashed = false";
        listRequest.Fields = "files(id, name)";
        listRequest.Spaces = "drive";
        
        var result = await listRequest.ExecuteAsync();
        var file = result.Files?.FirstOrDefault();
        return file?.Id;
    }

    private async Task<UserCredential?> GetCredentialAsync(bool forceRefresh = false)
    {
        if (_credential != null && !forceRefresh)
            return _credential;

        if (!File.Exists(CredentialsPath))
            throw new FileNotFoundException("Google Drive credentials file missing.", CredentialsPath);

        using var stream = new FileStream(CredentialsPath, FileMode.Open, FileAccess.Read);
        
        _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            Scopes,
            UserId,
            CancellationToken.None,
            new Google.Apis.Util.Store.FileDataStore(TokenStorePath, fullPath: true)
        );

        return _credential;
    }
}
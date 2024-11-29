namespace PartyPic.ThirdParty.Impl
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using PartyPic.Contracts.Logger;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class BlobStorageManager : IBlobStorageManager
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;

        public BlobStorageManager(IConfiguration config,
                                    ILoggerManager logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<string> Download(string fileName, string containerName)
        {
            try
            {
                var cloudBlobContainer = this.GetCloudBlobContainer(containerName);

                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(fileName);

                SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy();

                policy.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("DownloadExpirationTimePolicyInMinutes"));

                policy.Permissions = SharedAccessBlobPermissions.Read;

                string signature = blob.GetSharedAccessSignature(policy);

                var signedUri = blob.Uri + signature;

                return signedUri;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when trying to download file to blob storage. Exception: " + ex.InnerException + ". Message: " + ex.Message);
                throw;
            }
        }

        public async Task RemoveDocument(string fileName, string containerName)
        {
            try
            {
                var cloudBlobContainer = this.GetCloudBlobContainer(containerName);

                var blob = cloudBlobContainer.GetBlobReference(fileName);

                await blob.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when trying to remove file to blob storage. Exception: " + ex.InnerException + ". Message: " + ex.Message);
                throw;
            }
        }

        public async Task<string> Upload(string strFileName, byte[] fileData, string fileMimeType, string containerName)
        {
            try
            {
                var cloudBlobContainer = this.GetCloudBlobContainer(containerName);

                string fileName = this.GenerateFileName(strFileName);

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (fileName != null && fileData != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);

                    cloudBlockBlob.Properties.ContentType = fileMimeType;

                    await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);

                    return cloudBlockBlob.Uri.AbsoluteUri;
                }
                else
                {
                    _logger.LogError($"Error when trying to upload file to blob storage. FileData or FileName is null");
                    throw new InvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when trying to upload file to blob storage. Exception: " + ex.InnerException + ". Message: " + ex.Message);
                throw;
            }
        }

        private string GenerateFileName(string fileName)
        {
            string strFileName = string.Empty;

            string strName = Path.GetFileNameWithoutExtension(fileName).Replace(' ', '_');
            string strExtension = Path.GetExtension(fileName);

            strFileName = strName + "_" + DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + strExtension;

            return strFileName;
        }

        private CloudBlobContainer GetCloudBlobContainer(string containerName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_config.GetValue<string>("BlobStorageSettings:BlobConnectionAccessKey"));

            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

            return cloudBlobContainer;
        }
    }
}

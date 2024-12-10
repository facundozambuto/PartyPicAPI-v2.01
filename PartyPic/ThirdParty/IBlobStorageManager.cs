using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartyPic.ThirdParty
{
    public interface IBlobStorageManager
    {
        Task<string> Download(string fileName, string containerName);
        Task RemoveDocument(string fileName, string containerName);
        Task<string> Upload(string strFileName, byte[] fileData, string fileMimeType, string containerName);
        Task<byte[]> DownloadAlbumAsync(List<string> imagePaths, string containerName);
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.BannedProfiles;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Images;
using PartyPic.ThirdParty;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPic.Contracts.Images
{
    public class SqlImagesRepository : IImagesRepository
    {
        private readonly ImagesContext _imageContext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IBannedProfileRepository _bannedProfileRepository;
        private readonly IBlobStorageManager _blobStorageManager;

        public SqlImagesRepository(ImagesContext imageContext, 
                                    IConfiguration config, 
                                    IMapper mapper, 
                                    IBannedProfileRepository bannedProfileRepository,
                                    IBlobStorageManager blobStorageManager)
        {
            _imageContext = imageContext;
            _config = config;
            _mapper = mapper;
            _bannedProfileRepository = bannedProfileRepository;
            _blobStorageManager = blobStorageManager;
        }

        public IEnumerable<Image> GetAllEventImages(int eventId, bool firstRequest, string requestTime)
        {
            if (firstRequest)
            {
                return _imageContext.Images.Where(image => image.EventId == eventId && image.DeletedDatetime == null);
            }
            else
            {
               return _imageContext.Images.Where(image => image.EventId == eventId && image.DeletedDatetime == null && image.CreatedDatetime > DateTime.Parse(requestTime));
            }
        }

        public IEnumerable<Image> GetAllRemovedEventImages(int eventId, string requestTime)
        {
            return _imageContext.Images.Where(image => image.EventId == eventId && image.DeletedDatetime != null && image.DeletedDatetime < DateTime.Parse(requestTime));
        }

        public Image GetImageById(int imageId)
        {
            var image = _imageContext.Images.FirstOrDefault(i => i.ImageId == imageId);

            if (image == null)
            {
                throw new NotImageFoundException();
            }

            return image;
        }

        public bool SaveChanges()
        {
            return (_imageContext.SaveChanges() >= 0);
        }

        public Image AddEventImage(Image image, string fileUri)
        {
            image.CreatedDatetime = DateTime.Now;

            image.Path = fileUri;

            _imageContext.Images.Add(image);

            this.SaveChanges();

            return _imageContext.Images.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();
        }

        public async Task UploadImage(ImageFile uploadImage)
        {
            var bannedProfile = _bannedProfileRepository.GetBannedProfileById(uploadImage.ProfileId);

            if (bannedProfile != null)
            {
                throw new UnableToUploadImageException();
            }

            try
            {
                var file = uploadImage.Image;

                if (file.Length > 0)
                {
                    var imageModel = _mapper.Map<Image>(uploadImage);

                    var fileUri = await this.SendImageToAzureAsync(file);

                    if (string.IsNullOrEmpty(fileUri))
                    {
                        throw new UnableToUploadImageException();
                    }

                    this.AddEventImage(imageModel, fileUri);
                }
                else
                { 
                    throw new UnableToUploadImageException();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> SendImageToAzureAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new InvalidOperationException("El archivo proporcionado no es válido.");
            }

            string fileUri;
            string fileName = Path.GetFileName(file.FileName);

            byte[] fileData;
            using (var target = new MemoryStream())
            {
                await file.CopyToAsync(target); // Usa la versión asíncrona
                fileData = target.ToArray();
            }

            string mimeType = Path.GetExtension(fileName).ToLower() switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".tiff" => "image/tiff",
                ".ico" => "image/x-icon",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };

            string containerName = _config.GetValue<string>("BlobStorageSettings:ImagesBlobContainer");

            try
            {
                fileUri = await _blobStorageManager.Upload(fileName, fileData, mimeType, containerName);

                if (string.IsNullOrEmpty(fileUri))
                {
                    throw new Exception("La URI de la imagen devuelta está vacía.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir la imagen a Azure: {ex.Message}");
                throw;
            }

            return fileUri;
        }


        public void DeleteImage(DeleteImageRequest deleteImageRequest)
        {
            try
            {
                if (deleteImageRequest.EventId == 0 || deleteImageRequest.ImageId == 0)
                {
                    throw new ArgumentException();
                }

                var retrievedImage = this.GetImageById(deleteImageRequest.ImageId);

                if (retrievedImage != null)
                {
                    _imageContext.Remove(retrievedImage);

                    _imageContext.SaveChanges();

                    string filePath = Path.Combine(_config.GetValue<string>("DirectoryEventImagesPath") + deleteImageRequest.EventId, deleteImageRequest.ImageId + ".jpg");

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    if (deleteImageRequest.BlockProfile && deleteImageRequest.UserId != 0)
                    {
                        _bannedProfileRepository.BlockProfile(new Models.BannedProfile.BannedProfile
                        {
                            EventId = deleteImageRequest.EventId,
                            ProfileId = deleteImageRequest.ProfileId,
                            UserId = deleteImageRequest.UserId,
                            BannedName = deleteImageRequest.ProfileName
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

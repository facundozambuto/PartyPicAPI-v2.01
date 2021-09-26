using AutoMapper;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.BannedProfiles;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Images;
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

        public SqlImagesRepository(ImagesContext imageContext, IConfiguration config, IMapper mapper, IBannedProfileRepository bannedProfileRepository)
        {
            _imageContext = imageContext;
            _config = config;
            _mapper = mapper;
            _bannedProfileRepository = bannedProfileRepository;
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

        public Image AddEventImage(Image image, string fileName)
        {
            image.CreatedDatetime = DateTime.Now;

            var path = Directory.GetCurrentDirectory();

            image.Path = _config.GetValue<string>("StaticEventImagesPath") + image.EventId + "/" + fileName;

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
                var eventId = uploadImage.EventId;

                if (file.Length > 0)
                {
                    var imageModel = _mapper.Map<Image>(uploadImage);

                    this.AddEventImage(imageModel, file.FileName);

                    string filePath = Path.Combine(_config.GetValue<string>("DirectoryEventImagesPath") + eventId, file.FileName);
                    
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                else
                { 
                    throw new UnableToUploadImageException();
                }
            }
            catch (Exception ex)
            {
                throw new UnableToUploadImageException();
            }
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
            catch (Exception ex)
            {
                throw new UnableToDeleteImageException();
            }
        }
    }
}

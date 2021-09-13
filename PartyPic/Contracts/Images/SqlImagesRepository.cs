using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Images;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace PartyPic.Contracts.Images
{
    public class SqlImagesRepository : IImagesRepository
    {
        private readonly ImagesContext _imageContext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public SqlImagesRepository(ImagesContext imageContext, IConfiguration config, IMapper mapper)
        {
            _imageContext = imageContext;
            _config = config;
            _mapper = mapper;
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

        public Image GetImageById(int id)
        {
            throw new NotImplementedException();
        }

        public bool SaveChanges()
        {
            return (_imageContext.SaveChanges() >= 0);
        }

        public Image AddEventImage(Image image, string fileName)
        {
            image.CreatedDatetime = DateTime.Now;

            var path = System.IO.Directory.GetCurrentDirectory();

            image.Path = _config.GetValue<string>("StaticEventImagesPath") + image.EventId + "/" + fileName;

            _imageContext.Images.Add(image);

            this.SaveChanges();

            return _imageContext.Images.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();
        }

        public async Task UploadImage(ImageFile uploadImage)
        {
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
    }
}

using PartyPic.Models.Images;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyPic.Contracts.Images
{
    public class SqlImagesRepository : IImagesRepository
    {
        private readonly ImagesContext _imageContext;

        public SqlImagesRepository(ImagesContext imageContext)
        {
            _imageContext = imageContext;
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
            throw new NotImplementedException();
        }


        public void AddEventImage(Image image)
        {
            throw new NotImplementedException();
        }
    }
}

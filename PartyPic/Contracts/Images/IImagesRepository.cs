using PartyPic.Models.Images;
using System;
using System.Collections.Generic;

namespace PartyPic.Contracts.Images
{
    public interface IImagesRepository
    {
        IEnumerable<Image> GetAllEventImages(int eventId, bool firstRequest, string requestTime);
        Image GetImageById(int id);
        void AddEventImage(Image user);
        bool SaveChanges();
        IEnumerable<Image> GetAllRemovedEventImages(int eventId, string requestTime);
    }
}

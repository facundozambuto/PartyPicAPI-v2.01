using Microsoft.AspNetCore.Mvc;
using PartyPic.Models.Images;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartyPic.Contracts.Images
{
    public interface IImagesRepository
    {
        IEnumerable<Image> GetAllEventImages(int eventId, bool firstRequest, string requestTime);
        Image GetImageById(int id);
        Image AddEventImage(Image image, string fileName);
        bool SaveChanges();
        IEnumerable<Image> GetAllRemovedEventImages(int eventId, string requestTime);
        Task UploadImage(ImageFile uploadImage);
    }
}

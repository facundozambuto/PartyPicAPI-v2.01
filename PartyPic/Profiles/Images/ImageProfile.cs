using AutoMapper;
using PartyPic.DTOs.Images;
using PartyPic.Models.Images;

namespace PartyPic.Profiles.Images
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            CreateMap<Image, ImageReadDTO>();
            CreateMap<ImageCreateDTO, Image>();
        }
    }
}

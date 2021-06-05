using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PartyPic.Contracts.Images;
using PartyPic.DTOs.Images;
using PartyPic.Models.Images;
using System.Collections.Generic;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/images")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesRepository _eventImagesRepository;
        private readonly IMapper _mapper;

        public ImagesController(IImagesRepository eventImagesRepository, IMapper mapper)
        {
            _eventImagesRepository = eventImagesRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Image>> GetAllImages(int eventId, bool firstRequest, string requestTime)
        {

            var imageItems = _eventImagesRepository.GetAllEventImages(eventId, firstRequest, requestTime);

            return Ok(_mapper.Map<IEnumerable<ImageReadDTO>>(imageItems));
        }

        [HttpGet]
        [Route("~/api/images/removed")]
        public ActionResult<IEnumerable<Image>> GetAllRemovedImages(int eventId, string requestTime)
        {

            var imageItems = _eventImagesRepository.GetAllRemovedEventImages(eventId, requestTime);

            return Ok(_mapper.Map<IEnumerable<ImageReadDTO>>(imageItems));
        }


        [HttpGet("{id}", Name = "GetImageById")]
        public ActionResult<Image> GetImageById(int id)
        {
            var imageItem = _eventImagesRepository.GetImageById(id);

            if (imageItem != null)
            {
                return Ok(_mapper.Map<ImageReadDTO>(imageItem));
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult<ImageReadDTO> AddImage(ImageCreateDTO imageCreateDTO)
        {
            var imageModel = _mapper.Map<Image>(imageCreateDTO);
            _eventImagesRepository.AddEventImage(imageModel);
            _eventImagesRepository.SaveChanges();

            var imageReadDTO = _mapper.Map<ImageReadDTO>(imageModel);

            return CreatedAtRoute(nameof(GetImageById), new { Id = imageReadDTO.ImageId }, imageReadDTO);
        }

    }
}


using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Events;
using PartyPic.Contracts.Images;
using PartyPic.DTOs.Images;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Images;
using PartyPic.Models.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/images")]
    [ApiController]
    public class ImagesController : PartyPicControllerBase
    {
        private readonly IImagesRepository _eventImagesRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IEventRepository _eventRepository;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public ImagesController(IImagesRepository eventImagesRepository, IMapper mapper, IConfiguration config, IEventRepository eventRepository, Contracts.Logger.ILoggerManager logger) : base(mapper, config, logger)
        {
            _eventImagesRepository = eventImagesRepository;
            _mapper = mapper;
            _config = config;
            _eventRepository = eventRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Image>>> GetAllImagesAsync(int eventId, bool firstRequest, string requestTime)
        {
            var imageItems = await _eventImagesRepository.GetAllEventImagesAsync(eventId, firstRequest, requestTime);

            return Ok(_mapper.Map<IEnumerable<ImageReadDTO>>(imageItems));
        }

        [HttpGet]
        [Route("~/api/images/removed")]
        [Authorize]
        public ActionResult<IEnumerable<Image>> GetAllRemovedImages(int eventId, string requestTime)
        {
            var imageItems = _eventImagesRepository.GetAllRemovedEventImages(eventId, requestTime);

            return Ok(_mapper.Map<IEnumerable<ImageReadDTO>>(imageItems));
        }

        [HttpGet("{id}", Name = "GetImageById")]
        [Authorize]
        public ActionResult<Image> GetImageById(int id)
        {
            var imageItem = _eventImagesRepository.GetImageById(id);

            if (imageItem != null)
            {
                return Ok(_mapper.Map<ImageReadDTO>(imageItem));
            }

            return NotFound();
        }

        [HttpPost("UploadImage", Name = "UploadImage")]
        public async Task<ActionResult> UploadImageAsync([FromForm] ImageFile uploadImage)
        {
            try
            {
                await _eventImagesRepository.UploadImage(uploadImage);

                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("DeleteImage", Name = "DeleteImage")]
        [Route("~/api/images/delete")]
        [Authorize]
        public async Task<ActionResult> DeleteImage(DeleteImageRequest deleteImageRequest)
        {
            try
            {
                var user = (User)HttpContext.Items["User"];

                deleteImageRequest.UserId = user.UserId;

                return ExecuteMethod<DeleteImageRequest>(() => _eventImagesRepository.DeleteImageAsync(deleteImageRequest).GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {
                throw new UnableToDeleteImageException();
            }
        }

        [HttpGet("DownloadAlbum", Name = "DownloadAlbum")]
        [Route("~/api/images/download")]
        public async Task<IActionResult> DownloadAlbum(int eventId)
        {
            try
            {
                var evt = _eventRepository.GetEventById(eventId);

                if (evt == null)
                {
                    throw new NotEventFoundException();
                }

                var zipFile = await _eventImagesRepository.DownloadImagesAsZipAsync(eventId);

                return File(zipFile, "application/octet-stream", evt.Name + ".zip");
            }
            catch (Exception ex)
            {
                throw new UnableToDownloadAlbumException();
            }
        }

        [HttpGet("DownloadAlbum", Name = "DownloadAlbum")]
        [Route("~/api/images/downloadByEventCode")]
        public async Task<IActionResult> DownloadAlbumByEventCode([FromQuery] string eventCode)
        {
            try
            {
                var evt = _eventRepository.GetEventByEventCode(eventCode);

                if (evt == null)
                {
                    throw new NotEventFoundException();
                }

                var zipFile = await _eventImagesRepository.DownloadImagesAsZipAsync(evt.EventId);

                return File(zipFile, "application/octet-stream", evt.Name + ".zip");
            }
            catch (Exception ex)
            {
                throw new UnableToDownloadAlbumException();
            }
        }
    }
}


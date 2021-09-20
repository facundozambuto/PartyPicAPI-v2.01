using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Events;
using PartyPic.Contracts.Images;
using PartyPic.DTOs.Images;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Images;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/images")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesRepository _eventImagesRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IEventRepository _eventRepository;

        public ImagesController(IImagesRepository eventImagesRepository, IMapper mapper, IConfiguration config, IEventRepository eventRepository)
        {
            _eventImagesRepository = eventImagesRepository;
            _mapper = mapper;
            _config = config;
            _eventRepository = eventRepository;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<Image>> GetAllImages(int eventId, bool firstRequest, string requestTime)
        {
            var imageItems = _eventImagesRepository.GetAllEventImages(eventId, firstRequest, requestTime);

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
        public ActionResult UploadImage([FromForm] ImageFile uploadImage)
        {
            try
            {
                _eventImagesRepository.UploadImage(uploadImage);

                return Ok();
            }
            catch (System.Exception ex)
            {
                throw new UnableToUploadImageException();
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

                var filePaths = Directory.GetFiles(Path.Combine(_config.GetValue<string>("DirectoryEventImagesPath") + eventId));

                var zipFileMemoryStream = new MemoryStream();

                using (ZipArchive archive = new ZipArchive(zipFileMemoryStream, ZipArchiveMode.Update, leaveOpen: true))
                {
                    foreach (var filePath in filePaths)
                    {
                        var fileName = Path.GetFileName(filePath);
                        var entry = archive.CreateEntry(fileName);
                        using (var entryStream = entry.Open())
                        using (var fileStream = System.IO.File.OpenRead(filePath))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }

                zipFileMemoryStream.Seek(0, SeekOrigin.Begin);
                
                return File(zipFileMemoryStream, "application/octet-stream", evt.Name + ".zip");            
            }
            catch (System.Exception ex)
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

                var filePaths = Directory.GetFiles(Path.Combine(_config.GetValue<string>("DirectoryEventImagesPath") + evt.EventId));

                var zipFileMemoryStream = new MemoryStream();

                using (ZipArchive archive = new ZipArchive(zipFileMemoryStream, ZipArchiveMode.Update, leaveOpen: true))
                {
                    foreach (var filePath in filePaths)
                    {
                        var fileName = Path.GetFileName(filePath);
                        var entry = archive.CreateEntry(fileName);
                        using (var entryStream = entry.Open())
                        using (var fileStream = System.IO.File.OpenRead(filePath))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }

                zipFileMemoryStream.Seek(0, SeekOrigin.Begin);

                return File(zipFileMemoryStream, "application/octet-stream", evt.Name + ".zip");
            }
            catch (System.Exception)
            {
                throw new UnableToDownloadAlbumException();
            }
        }
    }
}


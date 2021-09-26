using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.BannedProfiles;
using PartyPic.DTOs.BannedProfiles;
using PartyPic.Models.BannedProfile;
using PartyPic.Models.BannedProfiles;
using PartyPic.Models.Common;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/bannedProfiles")]
    [ApiController]
    public class BannedProfileController : PartyPicControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly Contracts.Logger.ILoggerManager _logger;
        private readonly IBannedProfileRepository _bannedProfileRepository;

        public BannedProfileController(IMapper mapper, IConfiguration config, Contracts.Logger.ILoggerManager logger, IBannedProfileRepository bannedProfileRepository) : base(mapper, config, logger)
        {
            _mapper = mapper;
            _config = config;
            _bannedProfileRepository = bannedProfileRepository;
        }

        [HttpGet]
        [Authorize]
        [Route("~/api/bannedProfiles/grid")]
        public ActionResult<BannedProfileGrid> GetAllCategoriesForGrid([FromQuery] int current, [FromQuery] int rowCount, [FromQuery] string searchPhrase, [FromQuery] string sortBy, string orderBy)
        {
            GridRequest gridRequest = new GridRequest
            {
                Current = current,
                RowCount = rowCount,
                SearchPhrase = searchPhrase,
                SortBy = sortBy,
                OrderBy = orderBy
            };

            return ExecuteMethod<BannedProfileController, GridBannedProfileApiResponse, BannedProfileGrid>(() => _bannedProfileRepository.GetAllBannedProfilesForGrid(gridRequest));
        }

        [HttpGet("{profileId}", Name = "GetProfileByProfileId")]
        [Authorize]
        public ActionResult<BannedProfileReadDTO> GetProfileByProfileId(string profileId)
        {
            return ExecuteMethod<BannedProfileController, BannedProfileResponse, BannedProfileReadDTO>(() => _bannedProfileRepository.GetBannedProfileById(profileId));
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAllBannedProfiles()
        {
            return ExecuteMethod<BannedProfileController, GetAllBannedProfileResponse, AllBannedProfileResponse>(() => _bannedProfileRepository.GetAllBannedProfiles());
        }

        [HttpDelete("{profileId}")]
        [Authorize]
        public ActionResult UnblockProfile(string profileId)
        {
            return ExecuteMethod<BannedProfileController>(() => _bannedProfileRepository.UnblockProfile(profileId));
        }
    }
}

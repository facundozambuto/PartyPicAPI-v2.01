using AutoMapper;
using PartyPic.Contracts.Events;
using PartyPic.Contracts.Users;
using PartyPic.DTOs.BannedProfiles;
using PartyPic.Helpers;
using PartyPic.Models.BannedProfile;
using PartyPic.Models.BannedProfiles;
using PartyPic.Models.Common;
using PartyPic.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyPic.Contracts.BannedProfiles
{
    public class SqlBannedProfileRepository : IBannedProfileRepository
    {
        private BannedProfileContext _bannedProfileContext;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public SqlBannedProfileRepository(BannedProfileContext bannedProfileContext, IUserRepository userRepository, IEventRepository eventRepository, IMapper mapper, Contracts.Logger.ILoggerManager logger)
        {
            _bannedProfileContext = bannedProfileContext;
            _eventRepository = eventRepository;
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
        }

        public BannedProfile BlockProfile(BannedProfile bannedProfile)
        {
            this.ThrowExceptionIfArgumentIsNull(bannedProfile);

            bannedProfile.BanDatetime = DateTime.Now;

            _bannedProfileContext.Add(bannedProfile);

            this.SaveChanges();

            var addedBannedProfile = _bannedProfileContext.BannedProfiles.OrderByDescending(bp => bp.BanDatetime).FirstOrDefault();

            return addedBannedProfile;
        }

        private void ThrowExceptionIfArgumentIsNull(BannedProfile bannedProfile)
        {
            if (bannedProfile == null)
            {
                throw new ArgumentNullException(nameof(bannedProfile));
            }

            if (bannedProfile.EventId == 0)
            {
                throw new ArgumentNullException(nameof(bannedProfile.EventId));
            }

            if (bannedProfile.UserId == 0)
            {
                throw new ArgumentNullException(nameof(bannedProfile.UserId));
            }

            if (string.IsNullOrEmpty(bannedProfile.ProfileId))
            {
                throw new ArgumentNullException(nameof(bannedProfile.ProfileId));
            }
        }

        public AllBannedProfileResponse GetAllBannedProfiles()
        {
            var bannedProfiles = _mapper.Map<List<BannedProfileReadDTO>>(_bannedProfileContext.BannedProfiles);

            return new AllBannedProfileResponse
            {
                BannedProfiles = bannedProfiles
            };
        }

        public BannedProfileGrid GetAllBannedProfilesForGrid(GridRequest gridRequest)
        {
            var bannedProfileRows = new List<BannedProfile>();

            bannedProfileRows = _bannedProfileContext.BannedProfiles.ToList();

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                bannedProfileRows = _bannedProfileContext.BannedProfiles.Where(bp => bp.BannedName.Contains(gridRequest.SearchPhrase)
                                                 || bp.ProfileId.Contains(gridRequest.SearchPhrase)).ToList();
            }

            if (gridRequest.RowCount != -1 && _bannedProfileContext.BannedProfiles.Count() > gridRequest.RowCount && gridRequest.Current > 0 && bannedProfileRows.Count > 0)
            {
                var offset = gridRequest.RowCount;
                var index = (gridRequest.Current - 1) * gridRequest.RowCount;

                if ((bannedProfileRows.Count % gridRequest.RowCount) != 0 && (bannedProfileRows.Count / gridRequest.RowCount) < gridRequest.Current)
                {
                    offset = bannedProfileRows.Count % gridRequest.RowCount;
                }

                bannedProfileRows = bannedProfileRows.GetRange(index, offset);
            }

            if (!string.IsNullOrEmpty(gridRequest.SortBy) && !string.IsNullOrEmpty(gridRequest.OrderBy))
            {
                gridRequest.SortBy = WordingHelper.FirstCharToUpper(gridRequest.SortBy);

                bannedProfileRows = bannedProfileRows
                                .OrderBy(m => m.GetType()
                                                .GetProperties()
                                                .First(n => n.Name == gridRequest.SortBy)
                                .GetValue(m, null))
                                .ToList();

                if (gridRequest.OrderBy.ToLowerInvariant() == "desc")
                {
                    bannedProfileRows.Reverse();
                }
            }

            var bannedProfiles = _mapper.Map<List<BannedProfileReadDTO>>(bannedProfileRows);

            var events = _eventRepository.GetAllEvents().Events;

            var users = _userRepository.GetAllUsers().Users;

            foreach (BannedProfileReadDTO bp in bannedProfiles)
            {
                var retrievedEvent = events.FirstOrDefault(ev => ev.EventId == bp.EventId);

                if (retrievedEvent != null)
                {
                    bp.EventName = retrievedEvent.Name;
                }

                var user = users.FirstOrDefault(u => u.UserId == bp.UserId);

                if (user != null)
                {
                    bp.UserName = user.Name;
                }
            }

            if (!string.IsNullOrEmpty(gridRequest.EventId))
            {
                bannedProfiles = bannedProfiles.Where(bp => bp.EventId == Convert.ToInt32(gridRequest.EventId)).ToList();
            }

            if (!string.IsNullOrEmpty(gridRequest.UserId))
            {
                bannedProfiles = bannedProfiles.FindAll(bp => bp.UserId == Convert.ToInt32(gridRequest.UserId));
            }

            var bannedProfileGrid = new BannedProfileGrid
            {
                Rows = bannedProfiles,
                Total = _bannedProfileContext.BannedProfiles.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return bannedProfileGrid;
        }

        public BannedProfileReadDTO GetBannedProfileById(string bannedProfileId)
        {
            var bannedProfile = _mapper.Map<BannedProfileReadDTO>(_bannedProfileContext.BannedProfiles.FirstOrDefault(bp => bp.ProfileId == bannedProfileId));

            var events = _eventRepository.GetAllEvents().Events;

            var users = _userRepository.GetAllUsers().Users;

            var retrievedEvent = events.FirstOrDefault(ev => ev.EventId == bannedProfile.EventId);

            if (retrievedEvent != null)
            {
                bannedProfile.EventName = retrievedEvent.Name;
            }

            var user = users.FirstOrDefault(u => u.UserId == bannedProfile.UserId);

            if (user != null)
            {
                bannedProfile.UserName = user.Name;
            }

            return bannedProfile;
        }

        public bool SaveChanges()
        {
            return (_bannedProfileContext.SaveChanges() >= 0);
        }

        public void UnblockProfile(string bannedProfileId)
        {
            var bannedProfile = _bannedProfileContext.BannedProfiles.FirstOrDefault(bp => bp.ProfileId == bannedProfileId);

            if (bannedProfile == null)
            {
                throw new NotBannedProfileFoundException();
            }

            _bannedProfileContext.BannedProfiles.Remove(bannedProfile);

            this.SaveChanges();
        }
    }
}


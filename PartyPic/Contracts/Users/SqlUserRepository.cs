﻿using AutoMapper;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using PartyPic.Contracts.SessionLogs;
using PartyPic.DTOs.Users;
using PartyPic.Helpers;
using PartyPic.Models.Common;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Users;
using PartyPic.ThirdParty;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PartyPic.Contracts.Users
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly UserContext _userContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ISessionLogsRepository _sessionLogsRepository;
        private readonly IEmailSender _emailSenderManager;

        public SqlUserRepository(UserContext userContext, IMapper mapper, IConfiguration config, ISessionLogsRepository sessionLogsRepository, IEmailSender emailSenderManager)
        {
            _userContext = userContext;
            _mapper = mapper;
            _config = config;
            _sessionLogsRepository = sessionLogsRepository;
            _emailSenderManager = emailSenderManager;
        }

        public User CreateUser(User user)
        {
            this.ThrowExceptionIfArgumentIsNull(user);
            this.ThrowExceptionIfPropertyAlreadyExists(user, true, 0);
            this.ThrowExceptionIfPropertyIsIncorrect(user, true, 0);

            user.CreatedDatetime = DateTime.Now;

            _userContext.Users.Add(user);
            this.SaveChanges();

            var addedUser = _userContext.Users.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();

            return addedUser;
        }

        public AllUsersResponse GetAllUsers()
        {
            return new AllUsersResponse
            {
                Users = _userContext.Users.ToList()
            };
        }

        public User GetUserById(int userId)
        {
            var user = _userContext.Users.FirstOrDefault(user => user.UserId == userId);

            if (user == null)
            {
                throw new NotUserFoundException();
            }

            return user;
        }

        public bool SaveChanges()
        {
            return (_userContext.SaveChanges() >= 0);
        }

        public void DeleteUser(int userId)
        {
            var user = this.GetUserById(userId);

            if (user == null)
            {
                throw new NotUserFoundException();
            }

            _userContext.Users.Remove(user);

            this.SaveChanges();
        }

        public void PartiallyUpdate(int userId, UserUpdateDTO user)
        {
            this.UpdateUser(userId, user);

            this.SaveChanges();
        }

        public User UpdateUser(int userId, UserUpdateDTO userUpdateDto)
        {
            var user = _mapper.Map<User>(userUpdateDto);

            var retrievedUser = this.GetUserById(userId);

            if (retrievedUser == null)
            {
                throw new NotUserFoundException();
            }

            this.ThrowExceptionIfArgumentIsNull(user);
            this.ThrowExceptionIfPropertyAlreadyExists(user, false, userId);
            this.ThrowExceptionIfPropertyIsIncorrect(user, false, userId);

            _mapper.Map(userUpdateDto, retrievedUser);

            _userContext.Users.Update(retrievedUser);

            this.SaveChanges();

            return this.GetUserById(userId);
        }

        public User UpdateCurrentUser(int userId, UserUpdateDTO userUpdateDto)
        {
            var user = _mapper.Map<User>(userUpdateDto);

            var retrievedUser = this.GetUserById(userId);

            if (retrievedUser == null)
            {
                throw new NotUserFoundException();
            }

            this.ThrowExceptionIfPropertyAlreadyExists(user, false, userId);

            _mapper.Map(userUpdateDto, retrievedUser);

            _userContext.Users.Update(retrievedUser);

            _userContext.Entry(retrievedUser).Property(x => x.CreatedDatetime).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.RoleId).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.IsActive).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.Password).IsModified = false;

            this.SaveChanges();

            return this.GetUserById(userId);
        }

        public AllUsersResponse GetAllVenueUsers()
        {
            return new AllUsersResponse
            {
                Users = _userContext.Users.Where(u => u.RoleId == 2).ToList()
            };
        }

        public UserGrid GetAllUsersForGrid(GridRequest gridRequest)
        {
            var userRows = new List<User>();

            userRows = _userContext.Users.ToList();

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                userRows = _userContext.Users.Where(u => u.Cuil.Contains(gridRequest.SearchPhrase)
                                                 || u.Email.Contains(gridRequest.SearchPhrase)
                                                 || u.Name.Contains(gridRequest.SearchPhrase)
                                                 || u.Address.Contains(gridRequest.SearchPhrase)
                                                 || u.MobilePhone.Contains(gridRequest.SearchPhrase)).ToList();
            }


            if (gridRequest.RowCount != -1 && _userContext.Users.Count() > gridRequest.RowCount && gridRequest.Current > 0 && userRows.Count > 0)
            {
                var offset = gridRequest.RowCount;
                var index = (gridRequest.Current - 1) * gridRequest.RowCount;

                if ((userRows.Count % gridRequest.RowCount) != 0 && (userRows.Count / gridRequest.RowCount) < gridRequest.Current)
                {
                    offset = userRows.Count % gridRequest.RowCount;
                }

                userRows = userRows.GetRange(index, offset);
            }

            if (!string.IsNullOrEmpty(gridRequest.SortBy) && !string.IsNullOrEmpty(gridRequest.OrderBy))
            {
                gridRequest.SortBy = WordingHelper.FirstCharToUpper(gridRequest.SortBy);

                userRows = userRows
                                .OrderBy(m => m.GetType()
                                                .GetProperties()
                                                .First(n => n.Name == gridRequest.SortBy)
                                .GetValue(m, null))
                                .ToList();

                if (gridRequest.OrderBy.ToLowerInvariant() == "desc")
                {
                    userRows.Reverse();
                }
            }

            var userGrid = new UserGrid
            {
                Rows = userRows,
                Total = _userContext.Users.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return userGrid;
        }

        public LoginReadtDTO Login(LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                throw new PropertyIncorrectException();
            }

            var user = _userContext.Users.FirstOrDefault(user => user.Email == loginRequest.Email && user.Password == loginRequest.Password);

            if (user == null)
            {
                throw new NotUserFoundException();
            }

            if (!user.IsActive)
            {
                throw new NotActiveUserException();
            }

            var token = this.GenerateJwtToken(user);

            _sessionLogsRepository.AddSessionLog(user.UserId, "LOGIN");

            return new LoginReadtDTO
            {
                Email = user.Email,
                Name = user.Name,
                UserId = user.UserId,
                Token = token
            };
        }

        public void ChangeUserPassword(ChangePasswordRequest changePasswordRequest)
        {
            var retrievedUser = this.GetUserById(changePasswordRequest.UserId);

            if (retrievedUser == null)
            {
                throw new NotUserFoundException();
            };

            if (string.IsNullOrEmpty(changePasswordRequest.Password))
            {
                throw new PropertyIncorrectException();
            }

            retrievedUser.Password = changePasswordRequest.Password;

            _userContext.Users.Update(retrievedUser);

            _userContext.Entry(retrievedUser).Property(x => x.CreatedDatetime).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.RoleId).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.IsActive).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.Name).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.Address).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.Phone).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.MobilePhone).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.Cuil).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.Email).IsModified = false;
            _userContext.Entry(retrievedUser).Property(x => x.UserId).IsModified = false;

            this.SaveChanges();
        }

        public void RecoverPassword(string email)
        {
            var user = _userContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                throw new NotUserFoundException();
            }

            if (!user.IsActive)
            {
                throw new NotActiveUserException();
            }

            _emailSenderManager.SendEmail(
                user.Name,
                user.Email,
                _config.GetValue<string>("RecoverPasswordEmailSubject"),
                _config.GetValue<string>("RecoverPasswordEmailTemplate").Replace("***userEmail***", user.Email).Replace("***userPassword***", user.Password),
                null
             );
        }

        private void ThrowExceptionIfArgumentIsNull(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentNullException(nameof(user.Email));
            }

            if (string.IsNullOrEmpty(user.Address))
            {
                throw new ArgumentNullException(nameof(user.Address));
            }

            if (string.IsNullOrEmpty(user.Cuil))
            {
                throw new ArgumentNullException(nameof(user.Cuil));
            }

            if (string.IsNullOrEmpty(user.MobilePhone))
            {
                throw new ArgumentNullException(nameof(user.MobilePhone));
            }

            if (string.IsNullOrEmpty(user.Name))
            {
                throw new ArgumentNullException(nameof(user.Name));
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                throw new ArgumentNullException(nameof(user.Password));
            }

            if (string.IsNullOrEmpty(user.Phone))
            {
                throw new ArgumentNullException(nameof(user.Phone));
            }

            if (string.IsNullOrEmpty(user.RoleId.ToString()))
            {
                throw new ArgumentNullException(nameof(user.RoleId));
            }

            if (string.IsNullOrEmpty(user.IsActive.ToString()))
            {
                throw new ArgumentNullException(nameof(user.IsActive));
            }

            if (user.CreatedDatetime == null)
            {
                throw new ArgumentNullException(nameof(user.CreatedDatetime));
            }
        }

        private void ThrowExceptionIfPropertyAlreadyExists(User user, bool isNew, int userId)
        {
            var allUsers = _userContext.Users.ToList();

            if (!isNew)
            {
                allUsers = allUsers.Where(u => u.UserId != userId).ToList();
            }

            if (allUsers.Any(u => u.Cuil == user.Cuil || u.Email == user.Email))
            {
                throw new AlreadyExistingElementException();
            }
        }

        private void ThrowExceptionIfPropertyIsIncorrect(User user, bool isNew, int userId)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(user.Email);

                if (addr.Address != user.Email)
                {
                    throw new PropertyIncorrectException();
                }
            }
            catch (Exception)
            {
                throw new PropertyIncorrectException();
            }

            if (!isNew)
            {
                if (user.CreatedDatetime.Date != _userContext.Users.FirstOrDefault(u => u.UserId == userId).CreatedDatetime.Date)
                {
                    throw new PropertyIncorrectException();
                }
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("SecretJWTKey"));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserId.ToString()) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }
    }
}

﻿using PartyPic.Models.Common;
using System;

namespace PartyPic.DTOs.Users
{
    public class UserApiResponse : ApiResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Cuil { get; set; }
        public string MobilePhone { get; set; }
    }
}

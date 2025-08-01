﻿using EcommerceBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceBackend.BusinessObject.dtos.UserDto
{
    public class UserDto
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Phone { get; set; }
        public string UserName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }

        public DateTime CreateDate { get; set; }

        public int Status { get; set; }

        public bool IsDelete { get; set; }

    }
}

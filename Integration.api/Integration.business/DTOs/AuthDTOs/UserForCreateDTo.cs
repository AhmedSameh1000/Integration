﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRepairPro.Business.DTO.AuthDTOs
{
    public class UserForCreateDTo
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get;set; }
        public string RoleId { get; set; }
        public string Password { get; set; }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRepairPro.Business.DTO.AuthDTOs
{
    public class UserForUpdateDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string phoneNumber { get; set; }

        public string Email { get; set; }
    } 

    public class UserForReturnDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string phoneNumber { get; set; }

        public string Email { get; set; }
    }
}

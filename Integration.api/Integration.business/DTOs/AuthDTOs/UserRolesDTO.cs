using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRepairPro.Business.DTO.AuthDTOs
{
    public class UserRolesDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public List<RolesDTo> Roles { get; set; }
    }
}

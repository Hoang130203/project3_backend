using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.User
{
    public class UserProfileDto
    {
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string Website { get; set; }
    }
}

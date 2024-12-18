using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.User
{
    public class UserSuggestionDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
    }
}

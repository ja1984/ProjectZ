using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class TeamMember : User
    {

        public TeamMember()
        {
            
        }

        public TeamMember(User user, Role role, bool isPageAdmin)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            GravatarEmail = user.GravatarEmail;
            Role = role;
            UserName = user.UserName;
            DisplayName = user.DisplayName;
            IsPageAdmin = isPageAdmin;
            Id = user.Id;
        }



        public Role Role { get; set; }
        public bool IsPageAdmin { get; set; }
    }

    public enum Role
    {
        Developer,
        Designer,
        Administrative
    }
}
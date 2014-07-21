using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectZ.Web.Helpers;

namespace ProjectZ.Web.Models
{
    public class TeamMember : UserBase
    {

        public TeamMember()
        {

        }

        public TeamMember(User user, Role role, bool isPageAdmin, bool isCreator = false)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            GravatarEmail = user.GravatarEmail;
            Role = role;
            UserName = user.UserName;
            Slug = user.UserName.GenerateSlug();
            IsPageAdmin = isPageAdmin;
            UserId = user.Id;
            Image = GetImage();
            IsCreator = isCreator;
        }

        public object SmallInfo()
        {
            return new
                       {
                           Image = GetImage(24),
                           Role = Role.ToString(),
                           UserName = UserName,
                           FirstName = FirstName,
                           LastName = LastName,
                           UserId = UserId
                       };
        }


        public Role Role { get; set; }
        public string Image { get; set; }
        public bool IsPageAdmin { get; set; }
        public string UserId { get; set; }
        public bool IsCreator { get; set; }
    }

    public enum Role
    {
        Developer = 0,
        Designer = 1,
        Administrative = 2,
        Support = 3,
        Other = 4
    }
}
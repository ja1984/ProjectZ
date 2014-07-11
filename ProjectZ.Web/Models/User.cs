using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class User : UserBase
    {
        public User()
        {
            Follows = new List<Follow>();
            Projects = new List<UserProject>();
        }

        public string Password { get; set; }
        public string GitHub { get; set; }
        public bool DisplayEmail { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public List<Follow> Follows { get; set; }
        public List<UserProject> Projects { get; set; }

    }


    public class Follow
    {

        public Follow()
        {
            
        }
        public Follow(Project project)
        {
            Id = project.Id;
            Name = project.Name;

        }

        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class UserProject
    {

        public UserProject()
        {
            
        }
        public UserProject(Project project)
        {
            Id = project.Id;
            Name = project.Name;

        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}
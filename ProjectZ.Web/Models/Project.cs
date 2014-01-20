using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class Project
    {
        public Project()
        {
            Admins = new List<TeamMember>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public List<TeamMember> Admins { get; set; }
    }
}
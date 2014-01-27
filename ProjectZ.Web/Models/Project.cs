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
            Issues = new List<Issue>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public List<TeamMember> Admins { get; set; }
        public List<Issue> Issues { get; set; }
    }
}
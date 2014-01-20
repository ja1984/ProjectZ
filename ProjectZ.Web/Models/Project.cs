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
            Admins = new List<string>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string UserId { get; set; }
        public List<string> Admins { get; set; }
    }
}
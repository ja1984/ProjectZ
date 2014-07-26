using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.ViewModels
{
    public class ProjectViewModel
    {
        public ProjectViewModel()
        {
            Events = new List<EventAction>();
            Followers = new List<User>();
        }
        public Project Project { get; set; }
        public string ProjectDescription { get; set; }
        public int NumberOfFollowers { get; set; }
        public bool IsPageAdmin { get; set; }
        public bool Following { get; set; }
        public IEnumerable<Release> Releases { get; set; }
        public string StartPage { get; set; }
        public IEnumerable<EventAction> Events { get; set; }
        public IEnumerable<User> Followers { get; set; }

    }
}
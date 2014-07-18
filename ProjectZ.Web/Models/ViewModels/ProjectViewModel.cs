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
        }
        public Project Project { get; set; }
        public string ProjectDescription { get; set; }
        public int Followers { get; set; }
        public bool IsPageAdmin { get; set; }
        public bool Following { get; set; }
        public IEnumerable<Release> Releases { get; set; }
        public string StartPage { get; set; }
        public IEnumerable<EventAction> Events { get; set; }

    }
}
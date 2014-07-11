using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectZ.Web.Models.ViewModels
{
    public class FeedViewModel
    {
        public FeedViewModel()
        {
            Events = new List<EventAction>();
            Following = new List<Follow>();
            Projects = new List<UserProject>();
        }

        public List<EventAction> Events { get; set; }
        public List<Follow> Following { get; set; }
        public List<UserProject> Projects { get; set; }

    }
}
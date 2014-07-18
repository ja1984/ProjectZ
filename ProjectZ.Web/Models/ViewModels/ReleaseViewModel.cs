using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.ViewModels
{
    public class ReleaseViewModel
    {
        public ReleaseViewModel()
        {
            SolvedIssues = new List<Issue>();
        }

        public string Version { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public List<Issue> SolvedIssues { get; set; }
        public Project Project { get; set; }

    }
}
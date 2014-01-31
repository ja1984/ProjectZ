using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.ViewModels
{
    public class ReleaseViewModel
    {
        public Project Project { get; set; }
        public int NumberOfIssues { get; set; }
        public List<Release> Releases { get; set; }
    }
}
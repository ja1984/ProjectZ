using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.ViewModels
{
    public class ProjectViewModel
    {
        public Project Project { get; set; }
        public List<Issue> Issues { get; set; }
        public bool IsPageAdmin { get; set; }
    }
}
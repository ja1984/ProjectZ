using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectZ.Web.Models.ViewModels
{
    public class EditProjectViewModel
    {
        public Project Project { get; set; }
        public IEnumerable<Release> Releases { get; set; }
        public string StartPage { get; set; }
    }
}
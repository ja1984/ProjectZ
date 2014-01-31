using System;
using System.Collections.Generic;

namespace ProjectZ.Web.Models
{
    public class Release
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string ProjectId { get; set; }
        public DateTime Created { get; set; }
        public IList<Change> Changes { get; set; }
        public IList<Issue> Issues { get; set; }


    }
}
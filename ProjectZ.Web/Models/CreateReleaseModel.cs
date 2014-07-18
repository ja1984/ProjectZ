using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class CreateReleaseModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string ProjectId { get; set; }
        public DateTime Created { get; set; }
        public int[] SolvedIssues { get; set; }
    }
}
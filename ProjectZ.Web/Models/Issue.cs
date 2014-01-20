using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class Issue
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IList<Vote> Votes { get; set; }
        public string UserId { get; set; }
        public string ProjectId { get; set; }
        public IssueType IssueType { get; set; }
    }

    public enum IssueType
    {
        Bug = 0,
        Feature = 1
    }

    public enum IssueStatus
    {
        Closed = 0,
        Open = 1,
        InProgress = 2,
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class EventAction
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public Action Action { get; set; }
        public DateTime Created { get; set; }
    }

    public enum Action
    {
        Comment, TeamMember, Release, Bug, Feature, Poll
    }
}
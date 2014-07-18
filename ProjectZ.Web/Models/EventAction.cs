using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectZ.Web.Helpers;

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
        public EventActionUser User { get; set; }
        public EventActionReference Reference { get; set; }

        public string EventIcon()
        {
            switch (Action)
            {
                case Action.Bug:
                    return "fa-bug";
                case Action.Comment:
                    return "fa-comment";
                case Action.Release:
                    return "fa-check-circle-o";
                case Action.TeamMember:
                    return "fa-users";
                //case Action.Poll:
                //    return "";
                case Action.Feature:
                    return "fa-lightbulb-o";
                default:
                    return "fa-bullhorn";

            }
        }

        private string GetUserLink()
        {
            if (User == null)
                return "Someone";

            return string.Format("<a href='/user/{0}'>{1}</a>", User.Username.GenerateSlug(), User.Username);
        }

        private string GetTeammemberLink()
        {
            if (User == null)
                return "Someone";

            return string.Format("<a href='{0}'>{1}</a>", Url, Title);
        }

        private string GetProjectLink()
        {
            return string.Format("<a href='/{0}/{1}'>{2}</a>", ProjectId, ProjectName.GenerateSlug(), ProjectName);
        }

        private string GetEventLink()
        {
            return string.Format("<a href='{0}'>{1}</a>", Url, Title);
        }

        private string GetReleaseLink()
        {
            return string.Format("<a href='{0}'>{1}</a>", Url, Title);
        }



        public string GetEvent()
        {
            if (Action == Action.Bug)
                return string.Format("{0} reported {1} in {2}", GetUserLink(), GetEventLink(), GetProjectLink());

            if (Action == Action.Poll)
                return string.Format("{0} created poll {1} in {2}", GetUserLink(), GetEventLink(), GetProjectLink());

            if(Action == Action.Feature)
                return string.Format("{0} requested {1} in {2}", GetUserLink(), GetEventLink(), GetProjectLink());

            if (Action == Action.Comment)
                return string.Format("{0} commented on {1} in {2}",GetUserLink(),GetEventLink(),GetProjectLink());

            if(Action == Action.TeamMember)
                return string.Format("{0} added new team member {1} to {2}", GetUserLink(), GetTeammemberLink(), GetProjectLink());

            if (Action == Action.Release)
                return string.Format("{0} added a new release {1} to {2}", GetUserLink(), GetReleaseLink(), GetProjectLink());

            return "";
        }

    }

    public class EventActionReference
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class EventActionUser
    {

        public EventActionUser() { }

        public EventActionUser(User user)
        {
            Id = user.Id;
            Username = user.UserName;
        }

        public string Id { get; set; }
        public string Username { get; set; }
    }

    public enum Action
    {
        Comment, TeamMember, Release, Bug, Feature, Poll
    }
}
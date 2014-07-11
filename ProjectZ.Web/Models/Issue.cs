using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectZ.Web.Helpers;

namespace ProjectZ.Web.Models
{
    public class Issue
    {

        public Issue()
        {
            Votes = new List<Vote>();
            Comments = new List<IssueComment>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Posted { get; set; }
        public IList<Vote> Votes { get; set; }
        public IssueType IssueType { get; set; }
        public List<IssueComment> Comments { get; set; }
        public IssueUser User { get; set; }

        public string IssueTypeIcon()
        {
            if (IssueType == IssueType.Feature)
                return "fa-lightbulb-o";

            return "fa-bug";
        }

        public string IssueTypeText()
        {
            if (IssueType == IssueType.Feature)
                return "Feature";

            return "Bug";
        }

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

    public class IssueUser
    {
        public IssueUser() { }

        public IssueUser(User user)
        {
            UserId = user.Id;
            DisplayName = user.UserName;
            Image = user.GetImage();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Image { get; set; }
    }
}
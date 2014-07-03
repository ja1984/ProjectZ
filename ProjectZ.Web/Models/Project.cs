using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class Project
    {
        public Project()
        {
            Admins = new List<TeamMember>();
            Followers = new List<string>();
            Issues = new List<Issue>();
        }

        public string GetShortDescription()
        {
            return Description.Length > 200 ? Description.Substring(0, 200) : Description;
        }
        public bool LongText()
        {
            return Description.Length > 200;
        }

        public string GetLogo(int size = 52, LogoSize imageType = LogoSize.Normal)
        {
            if (string.IsNullOrEmpty(Logo))
                return "/Content/Images/nologo.png";

            return imageType == LogoSize.Normal ? Logo : string.Format("icon_{0}", Logo);
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string Logo { get; set; }
        public string Header { get; set; }
        public List<TeamMember> Admins { get; set; }
        public List<String> Followers { get; set; }
        public List<Issue> Issues { get; set; }

        public enum LogoSize
        {
            Normal, Icon
        }
    }
}
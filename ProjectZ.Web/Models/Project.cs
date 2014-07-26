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
            Polls = new List<Poll>();
            Questions = new List<Question>();
            Images = new List<ProjectImage>();
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
            if (Image == null)
                return "/Content/Images/nologo.png";

            return imageType == LogoSize.Normal ? Image.Logo : Image.Icon;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public ProjectLogo Image { get; set; }
        public string Header { get; set; }
        public List<TeamMember> Admins { get; set; }
        public List<String> Followers { get; set; }
        public List<Issue> Issues { get; set; }
        public List<Poll> Polls { get; set; }
        public bool IsPrivate { get; set; }
        public List<Question> Questions { get; set; }
        public List<ProjectImage> Images { get; set; }

        public enum LogoSize
        {
            Normal, Icon
        }


        public class ProjectLogo
        {
            public string Logo { get; set; }
            public string Icon { get; set; }
            public string Banner { get; set; }
            public string Thumbnail { get; set; }

        }
        public class ProjectImage
        {
            public string Url { get; set; }
            public string Thumbnail { get; set; }
        }
    }
}
using System;
using System.Collections.Generic;

namespace ProjectZ.Web.Models
{
    public class Release
    {

        public Release()
        {
            Images = new List<Image>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string ProjectId { get; set; }
        public DateTime Created { get; set; }
        public IList<Issue> Issues { get; set; }
        public ReleaseType Type { get; set; }
        public IList<Image> Images { get; set; }


        public class Image
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Thumbnail { get; set; }
        }

        public enum ReleaseType
        {
            Beta = 0,
            Minor = 1,
            Major = 2,
        }

    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using ProjectZ.Web.Models;
using Raven.Client.Document;

namespace ProjectZ.Web.Helpers
{

    public static class ImageUtils
    {
        private const string NOIMAGE_URL = "/Content/Images/nologo.png";

        public static string GetProjectImage(string projectId, ImageSize imageType = ImageSize.Normal)
        {

            using (var session = MvcApplication.Store.OpenSession())
            {
                using (session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(1000)))
                {
                    var project = session.Load<Project>(projectId);

                    if (project == null)
                        return NOIMAGE_URL;

                    if (project.Image == null)
                        return NOIMAGE_URL;

                    switch (imageType)
                    {
                        case ImageSize.Normal:
                            return project.Image.Logo;
                        case ImageSize.Icon:
                            return project.Image.Icon;
                        case ImageSize.Banner:
                            return string.IsNullOrEmpty(project.Image.Banner) ? NOIMAGE_URL : project.Image.Banner;
                    }



                }
            }

            return NOIMAGE_URL;
        }



        public enum ImageSize
        {
            Normal, Icon, Banner
        }


    }
}
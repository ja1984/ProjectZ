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
        private const string NOIMAGE_BANNER = "/Content/Images/noimage_thumbnail.png";

        public static string GetNoImage(ImageSize imageType = ImageSize.Normal)
        {
            return imageType == ImageSize.Normal ? NOIMAGE_URL : NOIMAGE_BANNER;
        }

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
                            return string.IsNullOrEmpty(project.Image.Logo) ? NOIMAGE_URL : project.Image.Logo;
                        case ImageSize.Icon:
                            return string.IsNullOrEmpty(project.Image.Icon) ? NOIMAGE_URL : project.Image.Icon;
                        case ImageSize.Banner:
                            return string.IsNullOrEmpty(project.Image.Banner) ? NOIMAGE_URL : project.Image.Banner;
                        case ImageSize.BannerThumb:
                            return string.IsNullOrEmpty(project.Image.Banner) ? NOIMAGE_BANNER : project.Image.Thumbnail;
                    }
                }
            }

            return NOIMAGE_URL;
        }

        private static string GetUploadFolder(string projectId, string image)
        {
            return string.Format("/Uploads/{0}/{1}", StringHelper.GetProjectFolderName(projectId), image);
        }


        public enum ImageSize
        {
            Normal, Icon, Banner, BannerThumb
        }


    }
}
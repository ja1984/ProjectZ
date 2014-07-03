using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ProjectZ.Web.Helpers
{

    public class ImageUtils
    {
        private const string UPLOAD_FOLDER = "{0}logo_{1}";
        private const string UPLOAD_FOLDER_ICON = "{0}icon_{1}";

        public static string GetLogo(string projectId, LogoSize imageType = LogoSize.Normal)
        {

            var id = StringHelper.GetProjectId(projectId);

            if (string.IsNullOrEmpty(projectId))
                return "/Content/Images/nologo.png";


            if (imageType == LogoSize.Icon && !string.IsNullOrEmpty(GetImage(id, string.Format(UPLOAD_FOLDER_ICON, GetUploadFolder(), id))))
                return string.Format(UPLOAD_FOLDER_ICON, "/Uploads/", GetImage(id, string.Format(UPLOAD_FOLDER_ICON, GetUploadFolder(), id)));

            if (imageType == LogoSize.Normal &&  !string.IsNullOrEmpty(GetImage(id, string.Format(UPLOAD_FOLDER, GetUploadFolder(), id))))
                return string.Format(UPLOAD_FOLDER, "/Uploads/", GetImage(id, string.Format(UPLOAD_FOLDER_ICON, GetUploadFolder(), id)));

            return "/Content/Images/nologo.png";
        }

        public enum LogoSize
        {
            Normal, Icon
        }

        private static string GetUploadFolder()
        {
            var test = HostingEnvironment.MapPath("/Uploads") + "\\";
            return test;
        }

        public static string GetImage(string id, string url)
        {

            //string.Format(UPLOAD_FOLDER_ICON, GetUploadFolder(), id)

            if (File.Exists(url + ".jpg"))
                return (id + ".jpg");

            if (File.Exists(url + ".png"))
                return (id + ".png");

            if (File.Exists(url + ".gif"))
                return (id + ".gif");

            return "";
        }

    }
}
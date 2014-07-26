using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using ProjectZ.Web.Helpers;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.Controllers
{
    public class ImageController : RavenController
    {
        [HttpPost]
        public JsonResult SaveLogo(string projectId, string logo, string icon)
        {
            var project = RavenSession.Load<Project>(projectId);

            if (project.Image != null)
            {
                if (!string.IsNullOrEmpty(project.Image.Logo))
                    DeleteOldImage(projectId, project.Image.Logo);

                if (!string.IsNullOrEmpty(project.Image.Icon))
                    DeleteOldImage(projectId, project.Image.Icon);
            }

            project.Image = new Project.ProjectLogo
            {
                Logo = logo,
                Icon = icon
            };
            RavenSession.SaveChanges();

            return Json("");
        }

        private void DeleteOldImage(string projectId, string imageName)
        {
            var file = Server.MapPath(imageName);

            if (!System.IO.File.Exists(file))
                return;

            System.IO.File.Delete(file);
        }

        [HttpPost]
        public JsonResult RemoveLogo(string projectId, string url)
        {
            var project = RavenSession.Load<Project>(projectId);
            //project.Logo = string.Empty;
            RavenSession.SaveChanges();

            var id = StringHelper.GetProjectId(projectId);



            var uploadUrl = Server.MapPath("/Uploads") + "\\";

            var logoUrl = uploadUrl + "logo_";
            //var logo = ImageUtils.GetImage(id, logoUrl + id);
            //if (!string.IsNullOrEmpty(logo))
            //    System.IO.File.Delete(logoUrl + logo);

            var iconUrl = uploadUrl + "icon_";
            //var icon = ImageUtils.GetImage(id, iconUrl + id);
            //if (!string.IsNullOrEmpty(icon))
            //    System.IO.File.Delete(iconUrl + icon);

            return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UploadLogo(HttpPostedFileBase uploadedFile, string projectId)
        {
            // Validate the uploaded file
            if (uploadedFile == null || (uploadedFile.ContentType != "image/jpeg" && uploadedFile.ContentType != "image/png"))
            {
                // Return bad request error code
                return Json(new
                {
                    success = false,
                    statusCode = 400,
                    status = "Bad Request! Upload Failed",
                    file = string.Empty
                }, "text/html");
            }

            // Save the file to the server



            var path = Server.MapPath("/Uploads") + "\\" + projectId.Replace("/", "");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + "\\";

            var completePath = path + uploadedFile.FileName;
            var fileExtension = Path.GetExtension(completePath);
            var newFileName = StringHelper.GetProjectId(projectId) + fileExtension;
            completePath = path + newFileName;
            uploadedFile.SaveAs(completePath);

            var logo = Guid.NewGuid() + fileExtension;
            var icon = Guid.NewGuid() + fileExtension;

            Resize(completePath, path, logo);
            Resize(completePath, path, icon, 24, 24, completePath);


            var project = RavenSession.Load<Project>(projectId);

            if (project.Image == null)
                project.Image = new Project.ProjectLogo();

            project.Image.Icon = project.Image.Thumbnail = string.Format("/Uploads/{0}/{1}", StringHelper.GetProjectFolderName(projectId), icon);
            project.Image.Logo = project.Image.Thumbnail = string.Format("/Uploads/{0}/{1}", StringHelper.GetProjectFolderName(projectId), logo);
            RavenSession.SaveChanges();

            // Return success code
            return Json(new
            {
                success = true,
                statusCode = 200,
                status = "Image uploaded.",
                logo = project.Image.Logo,
            }, "text/html");
        }

        [HttpPost]
        public JsonResult UploadProjectImage(HttpPostedFileBase uploadedFile, string projectId)
        {
            //Validate the uploaded file
            if (uploadedFile == null || (uploadedFile.ContentType != "image/jpeg" && uploadedFile.ContentType != "image/png"))
            {
                // Return bad request error code
                return Json(new
                {
                    statusCode = 400,
                    status = "Bad Request! Upload Failed",
                    file = string.Empty
                }, "text/html");
            }

            // Save the file to the server



            var path = Server.MapPath("/Uploads") + "\\" + projectId.Replace("/", "");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + "\\";

            var completePath = path + uploadedFile.FileName;
            var fileExtension = Path.GetExtension(completePath);
            var newFileName = Guid.NewGuid() + fileExtension;
            completePath = path + newFileName;
            uploadedFile.SaveAs(completePath);

            var image = newFileName;
            var thumb = "thumb_" + image;


            var projectImage = new Project.ProjectImage
            {
                Thumbnail = string.Format("/Uploads/{0}/{1}", StringHelper.GetProjectFolderName(projectId), thumb),
                Url = string.Format("/Uploads/{0}/{1}", StringHelper.GetProjectFolderName(projectId), image)
            };

            ResizeImage(completePath, path, thumb);

            var project = RavenSession.Load<Project>(projectId);

            project.Images.Add(projectImage);
            RavenSession.SaveChanges();

            // Return success code
            return Json(new
            {
                success = true,
                statusCode = 200,
                image = projectImage,
                status = "Image uploaded.",

            }, "text/html");
        }

        [HttpPost]
        public JsonResult UploadPromoImage(HttpPostedFileBase uploadedFile, string projectId)
        {
            //Validate the uploaded file
            if (uploadedFile == null || (uploadedFile.ContentType != "image/jpeg" && uploadedFile.ContentType != "image/png"))
            {
                // Return bad request error code
                return Json(new
                {
                    success = false,
                    statusCode = 400,
                    status = "Bad Request! Upload Failed",
                    file = string.Empty
                }, "text/html");
            }

            // Save the file to the server
            var path = Server.MapPath("/Uploads") + "\\" + projectId.Replace("/", "");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = path + "\\";

            var completePath = path + uploadedFile.FileName;
            var fileExtension = Path.GetExtension(completePath);
            var newFileName = Guid.NewGuid() + fileExtension;
            completePath = path + newFileName;
            uploadedFile.SaveAs(completePath);

            var image = newFileName;
            var thumbnail = "thumb_" + image;

            Resize(completePath, path, thumbnail, 173, 358);


            var project = RavenSession.Load<Project>(projectId);

            if (project.Image == null)
                project.Image = new Project.ProjectLogo();

            project.Image.Banner = string.Format("/Uploads/{0}/{1}", StringHelper.GetProjectFolderName(projectId), image);
            project.Image.Thumbnail = string.Format("/Uploads/{0}/{1}", StringHelper.GetProjectFolderName(projectId), thumbnail);
            RavenSession.SaveChanges();

            // Return success code
            return Json(new
            {
                success = true,
                statusCode = 200,
                image = project.Image,
                status = "Image uploaded.",

            }, "text/html");
        }

        private void Resize(String image, string location, string fileName, int height = 104, int width = 104, string original = null)
        {
            var srcImage = Image.FromFile(image);
            var imageJob = new ImageJob(srcImage, string.Format("{0}{1}", location, fileName), new ResizeSettings(width, height, FitMode.Max, null));
            try
            {
                imageJob.Build();
                srcImage.Dispose();

                if (!string.IsNullOrEmpty(original))
                    System.IO.File.Delete(original);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void ResizeImage(String image, string location, string fileName)
        {
            var width = 250;
            var height = 150;
            var srcImage = Image.FromFile(image);

            if (srcImage.Height > srcImage.Width)
            {
                width = 150;
                height = 250;
            }

            srcImage.Dispose();

            Resize(image, location, fileName, height, width);

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string GravatarEmail { get; set; }
        public string Password { get; set; }
        public string GitHub { get; set; }
        public bool DisplayEmail { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }


        public string GetImage()
        {
            using (var md5Hash = MD5.Create())
            {
                return string.Format("http://www.gravatar.com/avatar/{0}?s=26&d=mm", GetMd5Hash(md5Hash, GravatarEmail));
            }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            if (string.IsNullOrEmpty(input)) return "";
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ProjectZ.Web.Helpers
{
    public static class StringHelper
    {

        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static string GetProjectId(string projectId)
        {
            var id = projectId.Split('/');
            return id[1];
        }

        public static string GetProjectFolderName(string projectId)
        {
            return projectId.Replace("/", "");
        }
    }


}

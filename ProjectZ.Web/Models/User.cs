using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class User : UserBase
    {
        public string Password { get; set; }
        public string GitHub { get; set; }
        public bool DisplayEmail { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }

    }
}
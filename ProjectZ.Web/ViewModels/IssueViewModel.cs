﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.ViewModels
{
    public class IssueViewModel
    {
        public Project Project { get; set; }
        public string UserId { get; set; }
    }
}
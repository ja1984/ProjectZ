using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class Poll
    {

        public Poll()
        {
            Options = new List<Option>();
        }
        public string Title { get; set; }
        public List<Option> Options { get; set; }
    }


    public class Option
    {
        public Option()
        {
            Votes = new List<string>();
        }
        public string Title { get; set; }
        public List<string> Votes { get; set; }
    }
}
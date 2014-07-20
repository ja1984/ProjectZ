using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectZ.Web.Models
{
    public class Question
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Answer { get; set; }

    }


    public class CreateQuestionModel : Question
    {
        public string ProjectId { get; set; }
    }
}
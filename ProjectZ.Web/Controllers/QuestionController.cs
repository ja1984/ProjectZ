﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.Controllers
{
    public class QuestionController : RavenController
    {
        public JsonResult Create(CreateQuestionModel question)
        {
            var project = RavenSession.Load<Project>(question.ProjectId);

            var _question = new Question();

            if (question.Id > 0)
            {
                _question = project.Questions.FirstOrDefault(x => x.Id == question.Id);
                _question.Title = question.Title;
                _question.Answer = question.Answer;
            }
            else
            {

                _question = new Question()
                                    {
                                        Answer = question.Answer,
                                        Id = project.Questions.Count() + 1,
                                        Title = question.Title
                                    };
                project.Questions.Add(_question);
            }
            RavenSession.SaveChanges();

            return Json(new { Success = true, Question = _question });
        }
    }
}
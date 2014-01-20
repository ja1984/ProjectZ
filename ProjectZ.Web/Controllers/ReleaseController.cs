using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectZ.Web.Models;

namespace ProjectZ.Web.Controllers
{
    public class ReleaseController : RavenController
    {
        //
        // GET: /Release/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Release/Details/5
        public ActionResult Details(string projectName, int id)
        {
            return View();
        }

        //
        // GET: /Release/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Release/Create
        [HttpPost]
        public ActionResult Create(string projectName, Release release)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Release/Edit/5
        public ActionResult Edit(string projectName, int id)
        {
            return View();
        }

        //
        // POST: /Release/Edit/5
        [HttpPost]
        public ActionResult Edit(string projectName, int id, Release release)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Release/Delete/5
        public ActionResult Delete(string projectName, int id)
        {
            return View();
        }

        //
        // POST: /Release/Delete/5
        [HttpPost]
        public ActionResult Delete(string projectName, int id, Release release)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

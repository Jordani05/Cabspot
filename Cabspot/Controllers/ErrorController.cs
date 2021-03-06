﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cabspot.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;  //you may want to set this to 200
            return View("NotFound");
        }

        public ViewResult Forbidden()
        {
            Response.StatusCode = 403;  //you may want to set this to 200
            return View("Forbidden");
        }
    }
}
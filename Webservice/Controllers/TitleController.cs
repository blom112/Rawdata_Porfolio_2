﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webservice.Controllers
{
    public class TitleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;

namespace MSIT155Site.Controllers
{
    public class HomeWorkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Card()
        {
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WaterAPI.Controllers
{
    public class HomeController : Controller
    {
       
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Customers()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public String Test()
        {

            
            return "Hello";
        }

    }
}

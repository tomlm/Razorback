using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RazorbackSample.Controllers
{
    [Route("api/values")]
    public class ValuesController : Controller
    {
        // GET: Values
        public ActionResult Text()
        {
            return View();
        }

        [Route("card")]
        public ActionResult Card()
        {
            return View();
        }
    }
}
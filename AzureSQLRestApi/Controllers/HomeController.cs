using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using AzureSQLRestApi.Infrastructure;
using AzureSQLRestApi.Models;

namespace AzureSQLRestApi.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetAzureServerList(string subscriptionid)
        {
            AzureRestCaller restCall = new AzureRestCaller();
            var model = restCall.getAzureList(subscriptionid);
            return PartialView("_GetServerList",model);
        }

        public ActionResult createServer([FromBody]CreateServerModel objparams)
        {
            AzureRestCaller restCall = new AzureRestCaller();
            var model = restCall.CreateServer(objparams);
            return PartialView("_NewServrList", model);
        }

    }
}

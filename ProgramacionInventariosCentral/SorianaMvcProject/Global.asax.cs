using Inspinia_MVC5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebMvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Enabling Bundling and Minification
            BundleTable.EnableOptimizations = true;

            ViewEngines.Engines.Clear();

            ViewEngines.Engines.Add(new CustomRazorViewEngine());

            AreaRegistration.RegisterAllAreas();

            RouteConfig.RegisterRoutes(RouteTable.Routes);
    
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception lastErrorInfo = Server.GetLastError();
            Exception errorInfo = null;

            bool isNotFuund = false;
            if (lastErrorInfo != null)
            {
                errorInfo = lastErrorInfo.GetBaseException();
                var error = errorInfo as HttpException;
                if (error != null)
                    isNotFuund = error.GetHttpCode() == (int)HttpStatusCode.NotFound;
            }
            //if (isNotFuund)
            //{
            if (errorInfo != null)
            { System.Web.HttpContext.Current.Session["errorInfo"] = errorInfo.Message; }
            Server.ClearError();
            Response.Redirect("~/Error/NotFound");// Do what you need to render in view

            //}
            //else
            //{
            //    {
            //        Server.ClearError();

            //        if (errorInfo!=null)
            //        { System.Web.HttpContext.Current.Session["errorInfo"] = errorInfo.Message; }

            //        Response.Redirect("~/Error/ErrorNotFound");// Do what you need to render in view
            //    }
            //} 
        }
    }
}

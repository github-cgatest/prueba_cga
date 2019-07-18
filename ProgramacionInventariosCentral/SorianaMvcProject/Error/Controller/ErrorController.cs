using Soriana.FWK.Datos.Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApartadosSoriana.ErrorController.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NotFound()
        {
            try
            {
                //string srvName = HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["srvrName"].ToString());
                //srvName += HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["NombreSitio"].ToString()) +"/";
                //srvName += HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["defaultPage"].ToString());
                //ViewBag.Server = srvName;
                return View();
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                return View();
            }


        }

        public ActionResult NotAccess()
        {
            try
            {
                string srvName = HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["srvrName"].ToString())+"Login";
              
              
                ViewBag.Server = srvName;
                return View();
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                return View();
            }


        }

    }
}
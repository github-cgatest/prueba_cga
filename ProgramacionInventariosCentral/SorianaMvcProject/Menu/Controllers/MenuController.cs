
using Soriana.FWK.Common;
using Soriana.FWK.Datos.Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using sqlHelper = Soriana.FWK.Datos.SQL.SqlHelper;
using System.Web;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
//using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Data.SqlClient;
//using System.DirectoryServices;

namespace WebMvc.Menu.Controllers
{
    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }

    public class UsuarioADModels
    {
        public string DisplayName { get; set; }
        public string ApellidoP { get; set; }
        public string ApellidoM { get; set; }
        public string puesto { get; set; }
        public string NomUsuario { get; set; }
        public string departamento { get; set; }
        public string employeeID { get; set; }
        public string ou { get; set; }
        public string groupName { get; set; }

    }

    public class MenuModels
    {
        public string Usuario { get; set; }
        public string Id_Sistema { get; set; }
        public string Id_Usuario { get; set; }
        public string Id_Perfil { get; set; }
        public string Nombre { get; set; }
        public string Id_Parent { get; set; }
        public string Id_Recurso { get; set; }
        public string Id_Menu { get; set; }
        public string URL { get; set; }
        public string Posicion { get; set; }
        public string controlador { get; set; }
        public string accion { get; set; }

        //public List<MenuModels> subMenus { get; set; }
        public Dictionary<int, List<MenuModels>> subM { get; set; }
    }

    public class TransaccionPC
    {
        public int IdSistema { get; set; }
        public string NombreSistema { get; set; }
        public string url { get; set; }
    }

    public class MenuController : Controller
    {
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;
        //WindowsImpersonationContext impersonationContext;

        [DllImport("advapi32.dll")]
        public static extern int LogonUserA(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
        int impersonationLevel,
        ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        //[DllImport("advapi32.dll", SetLastError = true)]
        //public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,
        //int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        // GET: User
        //[CheckSessionOut]
        public ActionResult Index()
        {
           List<Soriana.FWK.Common.MenuModels> list = new List<Soriana.FWK.Common.MenuModels>();

            //list = Soriana.FWK.Common.Helper.GetMenuBySystemId(System.Web.HttpContext.Current.Session["SystemUserId"].ToString().Trim(), System.Web.HttpContext.Current.Session["SystemId"].ToString().Trim());

            ViewBag.listMenu = list;

            return View();
        }

        public ActionResult Login() {


            return View();
        }

        public ActionResult Comprobar(string usuario, string password)
        {
            Session["flagComprobar"] = "Y";

            return RedirectToAction("Index", "Menu");
            //////////try
            //////////{
            //////////    //Session["CredentialsExpire"] = "Se recomienda cambiar su contraseña porque expira en " + (DateTime.Now.AddDays(8).DayOfYear - DateTime.Now.DayOfYear).ToString() + " dias";

            //////////    Boolean flagImpersionate = false;
            //////////    string gp = System.Configuration.ConfigurationSettings.AppSettings["InitialCatalog_cs"].ToLower().Replace("tda", string.Empty).Replace("proddb", string.Empty);

            //////////    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("--------------------------------------------------");
            //////////    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Inicio de Sesion Nueva");


            //////////    #region Validacion de ActiveDirectory

            //////////    String domainAndUsername = ConfigurationManager.AppSettings["Domain"].ToString() + @"\" + usuario;
            //////////    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Domainuser :" + domainAndUsername);

            //////////    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Valida Active Directory");
            //////////    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("LDAP : " + ConfigurationManager.AppSettings["LDAP"].ToString());
            //////////    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("USER : " + domainAndUsername);

            //////////    DirectoryEntry entry = new DirectoryEntry(ConfigurationManager.AppSettings["LDAP"].ToString(), domainAndUsername, password);
            //////////    DirectorySearcher search = new DirectorySearcher(entry);

            //////////    search.Filter = "(SAMAccountName=" + usuario + ")";
            //////////    search.PropertiesToLoad.Add("cn");
            //////////    search.PropertiesToLoad.Add("accountExpires");
            //////////    search.PropertiesToLoad.Add("PasswordExpirationDate");

            //////////    SearchResult result = search.FindOne();
            //////////    var results = search.FindOne().GetDirectoryEntry();

            //////////    if (null != result)
            //////////    {
            //////////        DateTime expiresPass = (DateTime)results.InvokeGet("PasswordExpirationDate");

            //////////        Int64 numericDate = (Int64)result.Properties["accountExpires"][0];
            //////////        DateTime expires = (DateTime)results.InvokeGet("PasswordExpirationDate");// DateTime.FromFileTime(numericDate);

            //////////        if (expiresPass.DayOfYear <= DateTime.Now.DayOfYear && expiresPass.Year <= DateTime.Now.Year)//expires <= DateTime.Today)
            //////////        {
            //////////            Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Error - El password ha expirado.");
            //////////            Session["Nombre"] = "Error - El password ha expirado.";
            //////////            return RedirectToAction("Index", "Login");
            //////////        }
            //////////        else
            //////////        {
            //////////            if (expiresPass.Year == DateTime.Now.Year)
            //////////            {
            //////////                if ((expiresPass.DayOfYear - DateTime.Now.DayOfYear) <= Convert.ToInt32(ConfigurationManager.AppSettings["flagDaysPassToExpired"]))
            //////////                {
            //////////                    Session["CredentialsExpire"] = "Se recomienda cambiar su contraseña porque expira en " + (expiresPass.DayOfYear - DateTime.Now.DayOfYear).ToString() + " dias";
            //////////                }
            //////////            }
            //////////        }

            //////////        Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Usuario si existe en AD");
            //////////        Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Inicio de Validacion a Seguridad");

            //////////        Soriana.FWK.Datos.SQL.SqlHelper.connection_Name(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"]]);

            //////////        System.Collections.Hashtable parameters = new System.Collections.Hashtable();

            //////////        #region Impersionate
            //////////        if (ConfigurationManager.AppSettings["flagTypeConection"].Equals("Y"))
            //////////        {
            //////////            Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Usuario : " + usuario);
            //////////            Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Pass : " + Soriana.FWK.Seguridad.HelperEncriptar.Encriptar(password));
            //////////            Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Off Impersionate : " + WindowsIdentity.GetCurrent().Name);

            //////////            flagImpersionate = impersonateValidUser(usuario, System.Configuration.ConfigurationManager.AppSettings["Domain"], password);

            //////////            if (flagImpersionate)
            //////////            {
            //////////                Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Response Impersionate : " + WindowsIdentity.GetCurrent().Name);
            //////////            }
            //////////            else
            //////////            {
            //////////                Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Response Impersionate : " + GetLastError().ToString());
            //////////                if (GetLastError() != 0)
            //////////                {
            //////////                    if (GetLastError().Equals(1385) || GetLastError().Equals(1326))
            //////////                    {
            //////////                        Session["Nombre"] = "Error -" + System.Configuration.ConfigurationManager.AppSettings[GetLastError().ToString().Trim()] + "," + GetLastError().ToString();
            //////////                        return RedirectToAction("Index", "Login");
            //////////                    }
            //////////                    else
            //////////                    {
            //////////                        Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Response Impersionate : " + GetLastError().ToString());

            //////////                        Session["Nombre"] = "Error -Error de Impersonalizacion - Codigo: " + GetLastError().ToString();
            //////////                    }

            //////////                }


            //////////            }
            //////////        }
            //////////        else
            //////////        { flagImpersionate = true; }
            //////////        #endregion

            //////////        #region Carga HeaderSistema

            //////////        DataSet dsIni = new System.Data.DataSet();
            //////////        System.Collections.Hashtable parametersIni = new System.Collections.Hashtable();
            //////////        //Soriana.FWK.Datos.SQL.SqlHelper.connection_Name(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"]]);

            //////////        parametersIni.Add("@user", usuario);
            //////////        dsIni = Soriana.FWK.Datos.SQL.SqlHelper.ExecuteDataSet(System.Data.CommandType.StoredProcedure, "sp_fmk_GetUsuarioId", false, parametersIni);

            //////////        string usuarioId = string.Empty;

            //////////        Boolean flagDatosIn = false;
            //////////        if (dsIni != null) { if (dsIni.Tables.Count > 0) { if (dsIni.Tables[0].Rows.Count > 0) { flagDatosIn = true; } } }
            //////////        if (flagDatosIn)
            //////////        {
            //////////            usuarioId = dsIni.Tables[0].Rows[0][0].ToString();
            //////////            Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Obtencion de UsuarioId en Seguridad : " + usuarioId);
            //////////        }

            //////////        Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Carga del Encabezado de la aplicacion");

            //////////        CargaHeaderInfoSystem(usuarioId, ConfigurationManager.AppSettings["IdNumAplicacion"].ToString().Trim());

            //////////        #endregion

            //////////        parameters.Add("@email", usuario);
            //////////        parameters.Add("@N_Empleado", password);

            //////////        System.Data.DataSet ds = Soriana.FWK.Datos.SQL.SqlHelper.ExecuteDataSet(System.Data.CommandType.StoredProcedure, "sp_GetAccesoSistemas", false, parameters);

            //////////        if (Convert.ToInt32(ds.Tables[0].Rows[0][0]) > 0)
            //////////        {
            //////////            Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Usuario si existe en Seguridad");
            //////////            Session["Nombre"] = (String)result.Properties["cn"][0];
            //////////            TempData["usuario"] = usuario;
            //////////            TempData["pass"] = password;

            //////////            string u = Soriana.FWK.Seguridad.HelperEncriptar.Encriptar(usuario);
            //////////            string p = Soriana.FWK.Seguridad.HelperEncriptar.Encriptar(password);

            //////////            Session["userLine"] = usuario;
            //////////            Session["passLine"] = password;

            //////////            return RedirectToAction("Menu_Principal", "Login", new { user = u, pass = p });
            //////////        }
            //////////        else
            //////////        {

            //////////            if (ConfigurationManager.AppSettings["flagValidaGrupoAD"].Equals("Y"))
            //////////            {
            //////////                if (!ValidaUsuarioGrupo(usuario))
            //////////                {
            //////////                    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Error - Usuario sin Grupo de seguridad asignado");
            //////////                    Session["Nombre"] = "Error - Usuario sin Grupo de seguridad asignado";
            //////////                    return RedirectToAction("Index", "Login");
            //////////                }
            //////////            }


            //////////            Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Error - Usuario no existe en Seguridad");
            //////////            Session["Nombre"] = "Error - Usuario no existe en seguridad";
            //////////            return RedirectToAction("Index", "Login");
            //////////        }
            //////////    }
            //////////    else
            //////////    {
            //////////        Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Error - Usuario si no existe en AD , respuesta Nula");

            //////////        Session["Nombre"] = "NO TIENE ACCESO POR PARTE DE ACTIVE DIRECTORY-001";
            //////////        return RedirectToAction("Index", "Login");
            //////////    }

            //////////    #endregion       

            //////////}
            //////////catch (Exception ex)
            //////////{
            //////////    if (ex.Message.Contains("Logon failure: unknown user name or bad password"))
            //////////    { Session["Nombre"] = "Error - Usuario o password incorrectos"; }
            //////////    else
            //////////    { Session["Nombre"] = "Error - " + ex.Message; }
            //////////    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Error - " + ex.Message);

            //////////    return RedirectToAction("Index", "Login");
            //////////}

        }

        private Boolean ValidaUsuarioGrupo(string userId)
        {
            string ou = string.Empty;
            string groupName = string.Empty;

            UsuarioADModels usuario = new UsuarioADModels();

            string gp = System.Configuration.ConfigurationSettings.AppSettings["InitialCatalog_cs"].ToLower().Replace("tda", string.Empty).Replace("proddb", string.Empty);

            usuario.employeeID = userId;
            usuario.ou = ou.Trim();
            usuario.groupName = "Suc0" + gp.Trim();//groupName.Trim();

            string json2 = string.Empty;
            JavaScriptSerializer js = new JavaScriptSerializer();
            json2 = js.Serialize(usuario);
            js = null;

            var client = new WebMvc.Content.RestClient();


            client.EndPoint = System.Configuration.ConfigurationSettings.AppSettings["ApiValidaAGrupo"];
            client.Method = HttpVerb.POST;
            client.PostData = json2;

            var json = client.MakeRequest();

            if (json.Contains("1")) { return true; }

            return false;
        }

        public ActionResult Busqueda(string origen, string paramBusqueda, string valorBusqueda)
        {
            switch (origen)
            {
                //Busqueda Apartado
                case "1":
                    TempData["@srchApartadoParam"] = paramBusqueda;
                    TempData["@srchApartadoVal"] = valorBusqueda;

                    break;
                case "2":
                    TempData["@opBusquedaCliente"] = paramBusqueda;
                    TempData["@busquedaCliente"] = valorBusqueda;

                    break;

                case "3":
                    TempData["@srchUbicacionParamalm"] = paramBusqueda;
                    TempData["@srchUbicacionValalm"] = valorBusqueda;
                    break;
                default:
                    break;
            }
            //ACTION NAME , NOMBRE CARPETA
            return Json(new { status = "Ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PruebaCatch(string origen, string paramBusqueda, string valorBusqueda)
        {
            try
            {
                throw new Exception();
                //return Json(new { status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.errMesagge = "NG002-Error en la capa de datos no existe el SKU";
                return new HttpStatusCodeResult(500, "NG001-Error en la capa de datos no existe el SKU");
                //return Json(new { status = ex.Message }, JsonRequestBehavior.AllowGet); 
            }

        }
        
        #region seguridad

        [CheckSessionOut]
        public JsonResult validaPermiosBotones(string nombreBoton)
        {
            try
            {
                string nombreRecurso = "Senalizacion";
                //string cookie = string.Empty;
                //if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("CookieLogin"))
                //{
                //    cookie = this.ControllerContext.HttpContext.Request.Cookies["CookieLogin"].Value;
                //}

                //if (string.IsNullOrEmpty(cookie))
                //{
                //    string srvName = ConfigurationManager.AppSettings["srvrName"].ToString() + "login";
                //    var result = new { Success = true, Message = "OFF", urlRedirect = srvName };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}

                if (Helper.ObtenerPermisosRecursoPorUsuario(nombreRecurso, nombreBoton,Convert.ToInt32(System.Web.HttpContext.Current.Session["SystemUserId"])))
                {
                    Session["@usuaioAutoriza"] = System.Web.HttpContext.Current.Session["SystemId"].ToString();// Helper.UsuarioId(cookie).ToString();
                    var result = new { Success = true, Message = "OK", urlRedirect = "" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new { Success = true, Message = "NO", urlRedirect = "" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var result = new { Success = false, Message = ex.Message, urlRedirect = "" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        [CheckSessionOut]
        public JsonResult PermisoAccion(string usuario, string password, string nombreBoton)
        {
            try
            {

                string nombreRecurso = "Senalizacion";
                #region Valida Coockie
                //string cookie = string.Empty;
                //if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("CookieLogin"))
                //{
                //    cookie = this.ControllerContext.HttpContext.Request.Cookies["CookieLogin"].Value;
                //}

                //if (string.IsNullOrEmpty(cookie))
                //{
                //    string srvName = HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["srvrName"].ToString()) + "login";
                //    var result = new { Success = true, Message = "OFF", urlRedirect = srvName };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}


                #endregion


                if (Helper.ValidarUsuarioRecurso(usuario, password, nombreRecurso, nombreBoton))
                {
                    string idAutorizo = Helper.UsuarioIdXUser(usuario).ToString();
                    Session["@usuaioAutoriza"] = idAutorizo;
                    var result = new { Success = true, Message = "OK", urlRedirect = "", autorizado = idAutorizo };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new { Success = true, Message = "NO", urlRedirect = "", autorizado = "0" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var result = new { Success = false, Message = ex.Message, urlRedirect = "", autorizado = "" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        //[CheckSessionOut]
        public ActionResult logOut()
        {
            try
            {

                this.ControllerContext = Helper.LogOutSistema(this.ControllerContext);


                return new RedirectResult(ConfigurationManager.AppSettings["srvrName"].ToString() + ConfigurationManager.AppSettings["Login"].ToString());

            }
            catch (Exception)
            {
                throw;
            }
        }


        [CheckSessionOut]
        public JsonResult UsuarioPerfiles()
        {
            try
            {
                int Id_Usuario = 0;
                string nombreUsuario = ""
                    , perfiles = "";
                string cookie = string.Empty;
                if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("CookieLogin"))
                {
                    cookie = this.ControllerContext.HttpContext.Request.Cookies["CookieLogin"].Value;
                }

                if (string.IsNullOrEmpty(cookie))
                {
                    string srvName = HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["srvrName"].ToString()) + "login";
                    var result = new { Success = true, Message = "NO", urlRedirect = srvName, Usuario = "", Perfiles = "", MisApps = "" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Helper.ObtieneNombrePerfilesUsuario(cookie, ref nombreUsuario, ref perfiles, ref Id_Usuario);
                    var misApps = HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["srvrName"].ToString()) + "Login/login/menu_principal";
                    var result = new { Success = true, Message = "OK", urlRedirect = "", Usuario = nombreUsuario, Perfiles = perfiles, MisApps = misApps };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                var result = new { Success = false, Message = ex.Message, urlRedirect = "", Usuario = "", Perfiles = "", MisApps = "" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        //public ActionResult RedirectOption(string option)
        //{
        //    //option = option.ToUpper();

        //    //XmlDocument xDoc = new XmlDocument();

        //    //xDoc.Load(@ConfigurationManager.AppSettings["pathSiteMap"]);

        //    //XmlNodeList options = xDoc.GetElementsByTagName("siteMapNode");

        //    //int index = options.Count;
        //    //foreach (XmlElement nodo in options)
        //    //{
        //    //    if (nodo.GetAttribute("id").Equals(option) && nodo.GetAttribute("resourceKey").Equals(System.Web.HttpContext.Current.Session["App"].ToString().Trim()))
        //    //    {
        //    //        //return RedirectToAction(nodo.GetAttribute("action").Trim(), nodo.GetAttribute("controller").Trim());

        //    //        var urlBuilder = new UrlHelper(Request.RequestContext);
        //    //        var url = urlBuilder.Action(nodo.GetAttribute("action").Trim(), nodo.GetAttribute("controller").Trim());
        //    //        return Json(new { status = "success", url = url, action = nodo.GetAttribute("action").Trim(), controller = nodo.GetAttribute("controller").Trim() }, JsonRequestBehavior.AllowGet);

        //    //    }
        //    //}





        //    try
        //    {
        //        string variable = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"]];
        //        sqlHelper.connection_Name(variable);
        //        DataSet dts = new DataSet();

        //        int pIdUsuario = int.Parse(System.Web.HttpContext.Current.Session["SystemUserId"].ToString().Trim());
        //        System.Collections.Hashtable parametros = new System.Collections.Hashtable();
        //        parametros.Add("@pIdUsuario", pIdUsuario);
        //        parametros.Add("@pOpcion", option);

        //        dts = sqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "up_fmk_ValidaTransaccionPC", false, parametros);


        //        TransaccionPC item = new TransaccionPC();              

        //        foreach (DataTable table in dts.Tables)
        //        {
        //            foreach (DataRow dr in table.Rows)
        //            {
        //                item.IdSistema = int.Parse(dr["IdSistema"].ToString());
        //                item.NombreSistema = (dr["NombreSistema"].ToString());
        //                item.url = (dr["url"].ToString());
        //            }
        //        }


        //        string srvName = ConfigurationManager.AppSettings["srvrName"].ToString() + item.NombreSistema;
        //        var result = new { Success = true, datos = item, Message = "OK" };
        //        if (item.IdSistema == 0)
        //        {

        //            result = new { Success = false, datos = item, Message = "Transaccion invalida o usted no tiene acceso!!!" };
        //        }
        //        else
        //        {
        //            HttpContext.Request.Cookies["CookieLogin"]["IdSistema"] = item.IdSistema.ToString();
        //            HttpContext.Response.Cookies["CookieLogin"]["IdSistema"] = item.IdSistema.ToString();
        //            item.url = srvName + "/" + item.url;
        //        }
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {

        //        var result = new { Success = false, Message = ex.Message };
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }


        //}
        
        #region redirccion


        public class TransaccionPC
        {
            public int IdSistema { get; set; }
            public string NombreSistema { get; set; }
            public string url { get; set; }
        }
        [AuthorizationPrivilegeFilter]
        public ActionResult RedirectOption(string option)
        {
            //option = option.ToUpper();

            //XmlDocument xDoc = new XmlDocument();

            //xDoc.Load(@ConfigurationManager.AppSettings["pathSiteMap"]);

            //XmlNodeList options = xDoc.GetElementsByTagName("siteMapNode");

            //int index = options.Count;
            //foreach (XmlElement nodo in options)
            //{
            //    if (nodo.GetAttribute("id").Equals(option) && nodo.GetAttribute("resourceKey").Equals(System.Web.HttpContext.Current.Session["App"].ToString().Trim()))
            //    {
            //        //return RedirectToAction(nodo.GetAttribute("action").Trim(), nodo.GetAttribute("controller").Trim());

            //        var urlBuilder = new UrlHelper(Request.RequestContext);
            //        var url = urlBuilder.Action(nodo.GetAttribute("action").Trim(), nodo.GetAttribute("controller").Trim());
            //        return Json(new { status = "success", url = url, action = nodo.GetAttribute("action").Trim(), controller = nodo.GetAttribute("controller").Trim() }, JsonRequestBehavior.AllowGet);

            //    }
            //}





            try
            {
                string variable = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"]];
                sqlHelper.connection_Name(variable);
                DataSet dts = new DataSet();

                int pIdUsuario = int.Parse(System.Web.HttpContext.Current.Session["SystemUserId"].ToString().Trim());
                System.Collections.Hashtable parametros = new System.Collections.Hashtable();
                parametros.Add("@pIdUsuario", pIdUsuario);
                parametros.Add("@pOpcion", option);

                dts = sqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "up_fmk_ValidaTransaccionPC", false, parametros);


                TransaccionPC item = new TransaccionPC();

                foreach (DataTable table in dts.Tables)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        item.IdSistema = int.Parse(dr["IdSistema"].ToString());
                        item.NombreSistema = (dr["NombreSistema"].ToString());
                        item.url = (dr["url"].ToString());
                    }
                }


                string srvName = ConfigurationManager.AppSettings["srvrName"].ToString() + item.NombreSistema;
                var result = new { Success = true, datos = item, Message = "OK" };
                if (item.IdSistema == 0)
                {

                    result = new { Success = false, datos = item, Message = "Transaccion invalida o usted no tiene acceso!!!" };
                }
                else
                {
                    this.ControllerContext.HttpContext.Response.Cookies.Clear();

                    HttpCookie cookie = new HttpCookie("CookieLogin");

                    cookie["Created"] = DateTime.Now.ToShortTimeString();
                    cookie["UserId"] = Request.Cookies["CookieLogin"]["UserId"];
                    cookie["User"] = Request.Cookies["CookieLogin"]["User"];
                    cookie["Pass"] = Request.Cookies["CookieLogin"]["Pass"];
                    cookie["IdSistema"] = item.IdSistema.ToString();


                    cookie.Expires = DateTime.Now.AddHours(8);
                    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
                    item.url = srvName + item.url;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                var result = new { Success = false, Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        private bool impersonateValidUser(String userName, String domain, String password)
        {
            SafeTokenHandle safeTokenHandle;
            try
            {

                // Get the user token for the specified user, domain, and password using the
                // unmanaged LogonUser method.
                // The local machine name can be used for the domain name to impersonate a user on this machine.

                const int LOGON32_PROVIDER_DEFAULT = 0;
                //This parameter causes LogonUser to create a primary token.
                const int LOGON32_LOGON_INTERACTIVE = 2;

                // Call LogonUser to obtain a handle to an access token.
                bool returnValue = LogonUser(userName, domain, password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeTokenHandle);

                Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("LogonUser called.");

                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("LogonUser failed with error code : " + ret);
                    throw new System.ComponentModel.Win32Exception(ret);
                }
                using (safeTokenHandle)
                {
                    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Did LogonUser Succeed? " + (returnValue ? "Yes" : "No"));
                    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Value of Windows NT token: " + safeTokenHandle);

                    // Check the identity.
                    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Before impersonation: " + WindowsIdentity.GetCurrent().Name);
                    // Use the token handle returned by LogonUser.
                    using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                    {
                        using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                        {

                            // Check the identity.
                            Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("After impersonation: " + WindowsIdentity.GetCurrent().Name);
                            return true;
                        }
                    }
                    // Releasing the context object stops the impersonation
                    // Check the identity.
                    Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("After closing the context: " + WindowsIdentity.GetCurrent().Name);
                }
            }
            catch (Exception ex)
            {
                Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Exception occurred. " + ex.Message);
                return false;
            }

        }

        private void CargaHeaderInfoSystem(string userId, string idSistema)
        {
            try
            {
                DataSet ds = new DataSet();

                try
                {
                    System.Collections.Hashtable parameters = new System.Collections.Hashtable();

                    parameters.Add("@in_Id_Usuario", userId);
                    parameters.Add("@in_Id_Sistema", idSistema);
                    ds = Soriana.FWK.Datos.SQL.SqlHelper.ExecuteDataSet(System.Data.CommandType.StoredProcedure, "sp_GetHeaderSystemInfo", false, parameters);

                    Boolean flagDatosIn = false;
                    if (ds != null) { if (ds.Tables.Count > 0) { if (ds.Tables[0].Rows.Count > 0) { flagDatosIn = true; } } }

                    if (flagDatosIn)
                    {
                        System.Web.HttpContext.Current.Session["Tienda"] = ds.Tables[0].Rows[0][1].ToString().Trim();
                    }
                    else
                    {
                        System.Web.HttpContext.Current.Session["Tienda"] = string.Empty;
                    }
                    flagDatosIn = false;
                    if (ds != null) { if (ds.Tables.Count > 0) { if (ds.Tables[1].Rows.Count > 0) { flagDatosIn = true; } } }

                    if (flagDatosIn)
                    {
                        System.Web.HttpContext.Current.Session["sNombre"] = ds.Tables[1].Rows[0][6].ToString().Trim();
                        System.Web.HttpContext.Current.Session["Profile"] = ds.Tables[1].Rows[0][5].ToString().Trim();
                        System.Web.HttpContext.Current.Session["App"] = ds.Tables[1].Rows[0][4].ToString().Trim();
                    }
                    else
                    {
                        System.Web.HttpContext.Current.Session["sNombre"] = string.Empty;
                        System.Web.HttpContext.Current.Session["Profile"] = string.Empty;
                        System.Web.HttpContext.Current.Session["App"] = string.Empty;
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {

                    throw ex;
                }

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

    }
}
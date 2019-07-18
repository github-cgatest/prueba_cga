
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Soriana.FWK;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data;
using SqlHelper = Soriana.FWK.Datos.SQL.SqlHelper;
using Soriana.FWK.Seguridad;

namespace WebMvc.Menu
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSessionOutAttribute : ActionFilterAttribute
    {
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;
        WindowsImpersonationContext impersonationContext;

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

        //string srvName = Soriana.FWK.Seguridad.HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["srvrName"].ToString());
        string srvName = ConfigurationManager.AppSettings["srvrName"].ToString();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            //#region Creacion de cookie
            //if (filterContext.HttpContext.Request.Cookies.AllKeys.Contains("CookieLogin"))
            //{

            //    filterContext.HttpContext.Response.Cookies.Clear();
            //    filterContext.HttpContext.Request.Cookies.Clear();
            //}


            //DataSet ds = new System.Data.DataSet();
            //System.Collections.Hashtable parameters = new System.Collections.Hashtable();
            //Soriana.FWK.Datos.SQL.SqlHelper.connection_Name(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["Ambiente"]]);

            //parameters.Add("@user", "900500614");
            //ds = Soriana.FWK.Datos.SQL.SqlHelper.ExecuteDataSet(System.Data.CommandType.StoredProcedure, "sp_fmk_GetUsuarioId", false, parameters);

            //string usuarioId = string.Empty;
            //if (ds.Tables.Count > 0)
            //{
            //    if (ds.Tables[0].Rows.Count > 0)
            //    {
            //        usuarioId = ds.Tables[0].Rows[0][0].ToString();
            //        HttpCookie cookieO = new HttpCookie("CookieLogin");
            //        //cookieO.Value = "Created:" + DateTime.Now.ToShortTimeString() + "," + "UserId:" + usuarioId + "," + "User:t_octaviomr";


            //        //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //        //string json = serializer.Serialize(System.Security.Principal.WindowsIdentity.GetCurrent());

            //        cookieO["Created"] = DateTime.Now.ToShortTimeString();
            //        cookieO["UserId"] = usuarioId;
            //        cookieO["User"] = "dABfAG8AYwB0AGEAdgBpAG8AbQByAA==";
            //        cookieO["Pass"] = "TwBjAHQAQAB2AGkAbwA4ADQA";
            //        cookieO["IdSistema"] = "4";


            //        cookieO.Expires = DateTime.Now.AddHours(8);
            //        filterContext.HttpContext.Response.Cookies.Add(cookieO);

            //        //return Redirect(System.Configuration.ConfigurationManager.AppSettings[urlDestino.ToString().Trim()]);
            //    }
            //    else
            //    {
            //        //if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("CookieLogin"))
            //        //{
            //        //    HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["CookieLogin"];
            //        //    cookie.Expires = DateTime.Now.AddDays(-1);
            //        //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            //        //}
            //    }
            //}


            //#endregion


            //////////////var cookie = string.Empty;
            //////////////if (!filterContext.HttpContext.Request.Cookies.AllKeys.Contains("CookieLogin"))
            //////////////{
            //////////////    filterContext.Result = new RedirectResult(srvName + ConfigurationManager.AppSettings["Login"]);
            //////////////    return;
            //////////////}
            //////////////else
            //////////////{
            //////////////    cookie = filterContext.HttpContext.Request.Cookies["CookieLogin"].Value;
            //////////////    if (string.IsNullOrEmpty(cookie))
            //////////////    {
            //////////////        filterContext.Result = new RedirectResult(srvName + ConfigurationManager.AppSettings["Login"]);
            //////////////        return;
            //////////////    }
            //////////////    else
            //////////////    {
            //////////////        //WindowsIdentity response = Newtonsoft.Json.JsonConvert.DeserializeObject<WindowsIdentity>(filterContext.HttpContext.Request.Cookies["CookieLogin"]["Impersionate"]);
            //////////////        //System.Web.HttpContext.Current.Session["ImpersionateUser"] = response;
            //////////////        string userId = filterContext.HttpContext.Request.Cookies["CookieLogin"]["UserId"];
            //////////////        string idSistema = filterContext.HttpContext.Request.Cookies["CookieLogin"]["IdSistema"];

            //////////////        //Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("User" + filterContext.HttpContext.Request.Cookies["CookieLogin"]["User"]);
            //////////////        //Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("Pass" + filterContext.HttpContext.Request.Cookies["CookieLogin"]["Pass"]);
            //////////////        Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("User : "+ filterContext.HttpContext.Request.Cookies["CookieLogin"]["User"] + " - Respuesta de Impersionate - " + GetLastError().ToString());

            //////////////        string user = Soriana.FWK.Seguridad.HelperEncriptar.Desencriptar(filterContext.HttpContext.Request.Cookies["CookieLogin"]["User"]);
            //////////////        string pass = Soriana.FWK.Seguridad.HelperEncriptar.Desencriptar(filterContext.HttpContext.Request.Cookies["CookieLogin"]["Pass"]);

            //////////////        //user = "900500025";
            //////////////        //pass = "Vale1203";
            //////////////        System.Web.HttpContext.Current.Session["SystemUser"] = user;
            //////////////        System.Web.HttpContext.Current.Session["SystemUserId"] = userId;
            //////////////        System.Web.HttpContext.Current.Session["SystemId"] = idSistema;
            //////////////        System.Web.HttpContext.Current.Session["flagSecurity"] = true;

            //////////////        System.Web.HttpContext.Current.Session["urlLogin"] = srvName + ConfigurationManager.AppSettings["HomeLogin"]+ string.Format("?user={0}&pass={1}", filterContext.HttpContext.Request.Cookies["CookieLogin"]["User"].ToString(), HelperEncriptar.Encriptar(filterContext.HttpContext.Request.Cookies["CookieLogin"]["Pass"].ToString()));


            //////////////        if (System.Configuration.ConfigurationManager.AppSettings["flagTypeConection"].Trim().Equals("Y"))
            //////////////        {

            //////////////            if (impersonateValidUser(user, System.Configuration.ConfigurationManager.AppSettings["Domain"], pass))
            //////////////            {
            //////////////                Soriana.FWK.Datos.SQL.SqlHelper.connection_Name(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"]]);
            //////////////            }
            //////////////            else
            //////////////            {
            //////////////                Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("User : " + filterContext.HttpContext.Request.Cookies["CookieLogin"]["User"] + " - Respuesta de Impersionate - " + GetLastError().ToString());

            //////////////                //srvName += Soriana.FWK.Seguridad.HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["NombreSitio"].ToString()) + "/Error/NotAccess";
            //////////////                srvName += ConfigurationManager.AppSettings["NombreSitio"].ToString() + "/Error/NotAccess";
            //////////////                filterContext.Result = new RedirectResult(srvName);
            //////////////                return;
            //////////////            }
            //////////////        }
            //////////////        else
            //////////////        {
            //////////////            //Soriana.FWK.Log.clsLogManagerFWK.WriteMessage_Bitacora("User : " + filterContext.HttpContext.Request.Cookies["CookieLogin"]["User"] + " - Respuesta de Impersionate - " + GetLastError().ToString());

            //////////////            Soriana.FWK.Datos.SQL.SqlHelper.connection_Name(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"]]);
            //////////////        }

            //////////////        CargaHeaderInfoSystem(userId, idSistema);

            //////////////        ///cuando este la configuracion de seguridad este lista descomentar
            //////////////        if (!ValidaUsuarioSistema(userId, idSistema))
            //////////////        {
            //////////////            //filterContext.Result = new RedirectResult(srvName + "/Menu_Principal?msg=1");
            //////////////            //srvName += HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["NombreSitio"].ToString()) + "/Error/NotAccess";
            //////////////            srvName += ConfigurationManager.AppSettings["NombreSitio"].ToString() + "/Error/NotAccess";
            //////////////            filterContext.Result = new RedirectResult(srvName);
            //////////////            return;
            //////////////        }
            //////////////        else
            //////////////        {
            //////////////            var defaultPage = ConfigurationManager.AppSettings["defaultPage"].ToString();// HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["defaultPage"].ToString());
            //////////////            var path = HttpContext.Current.Request.FilePath.ToLower();
            //////////////            if (path.Length == 1)
            //////////////            {
            //////////////                path = (path + defaultPage).ToLower();
            //////////////            }
            //////////////            else
            //////////////            {
            //////////////                //path = path.Replace("/" + HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["NombreSitio"].ToString()).ToLower(), "");
            //////////////                path = path.Replace("/" + ConfigurationManager.AppSettings["NombreSitio"].ToString().ToLower(), "");
            //////////////            }
            //////////////            if (path.Length == 0)
            //////////////            {
            //////////////                path = ("/" + defaultPage).ToLower();
            //////////////            }
            //////////////            if (!ValidaUsuarioPagina(userId, idSistema, path))
            //////////////            {
            //////////////                //srvName += HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["NombreSitio"].ToString()) + "/Error/NotAccess?" + cookie;
            //////////////                //srvName += HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["NombreSitio"].ToString()) + "/Error/NotAccess?";
            //////////////                srvName += ConfigurationManager.AppSettings["NombreSitio"].ToString() + "/Error/NotAccess?";
            //////////////                //filterContext.Result = new RedirectResult(srvName + "/Menu_Principal?msg=1");
            //////////////                filterContext.Result = new RedirectResult(srvName);
            //////////////                return;
            //////////////            }
            //////////////        }
            //////////////    }
            //////////////}



            base.OnActionExecuting(filterContext);
        }

        private bool impersonateValidUser(String userName, String domain, String password)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (RevertToSelf())
            {
                if (LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
                LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                {
                    if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        impersonationContext = tempWindowsIdentity.Impersonate();
                        if (impersonationContext != null)
                        {
                            CloseHandle(token);
                            CloseHandle(tokenDuplicate);
                            return true;
                        }
                    }
                }
            }
            if (token != IntPtr.Zero)
                CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero)
                CloseHandle(tokenDuplicate);
            return false;
        }

        public static bool ValidaUsuarioPagina(string userId, string idSistema, string url)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new System.Exception("El usuario no existe, favor de volver a iniciar sesion");
                }

                if (url.Substring(url.Length - 1, 1) == "/" || url.Substring(url.Length - 1, 1) == @"\")
                {
                    url = url.Remove(url.Length - 1);
                }

                int id_Usuario = 0;


                //string[] parametros = cookieUsuario.Split(',');
                //string[] rowUsuario = parametros[1].Split(':');
                id_Usuario = int.Parse(userId);

                int id_Sistema = int.Parse(idSistema);
                DataSet ds = new DataSet();

                try
                {
                    System.Collections.Hashtable parameters = new System.Collections.Hashtable();

                    parameters.Add("@Id_Usuario", id_Usuario);
                    parameters.Add("@Id_Sistema", idSistema);
                    parameters.Add("@url", url);
                    ds = SqlHelper.ExecuteDataSet(System.Data.CommandType.StoredProcedure, "SP_SHW_USURIOSISTEMA_POR_USUARIO_ID_SISTEMA_V2", false, parameters);

                }
                catch (SqlException ex)
                { throw ex; }
                catch (System.Exception ex)
                { throw ex; }

                bool permiso = false;
                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        permiso = Convert.ToBoolean(dr["result"]);
                    }
                }

                return permiso;
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

        /// <summary>
        /// Valida si el usuario tiene acceso al sistema
        /// </summary>
        /// <param name="cookieUsuario">string con la informacion del usuario</param>
        /// <returns></returns>
        private bool ValidaUsuarioSistema(string userId, string idSistema)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new System.Exception("El usuario no existe, favor de volver a iniciar sesion");
                }

                int id_Usuario = 0;

                id_Usuario = int.Parse(userId);

                int id_Sistema = int.Parse(idSistema);

                DataSet ds = new DataSet();

                try
                {
                    System.Collections.Hashtable parameters = new System.Collections.Hashtable();

                    parameters.Add("@Id_Usuario", id_Usuario);
                    parameters.Add("@Id_Sistema", id_Sistema);
                    ds = SqlHelper.ExecuteDataSet(System.Data.CommandType.StoredProcedure, "[SP_SHW_USURIOSISTEMA_POR_USUARIO_ID_SISTEMA]", false, parameters);

                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (System.Exception ex)
                {

                    throw ex;
                }


                bool permiso = false;
                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        permiso = Convert.ToBoolean(dr["result"]);
                    }
                }

                return permiso;
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
                    ds = SqlHelper.ExecuteDataSet(System.Data.CommandType.StoredProcedure, "sp_GetHeaderSystemInfo", false, parameters);

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

        public void logOut()
        {
            try
            {
                #region Valida Coockie
                //string cookie2 = string.Empty;
                //if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("CookieLogin"))
                //{
                //    cookie2 = this.ControllerContext.HttpContext.Request.Cookies["CookieLogin"].Value;
                //}

                //if (string.IsNullOrEmpty(cookie2))
                //{
                //    string srvName2 = HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["srvrName"].ToString()) + "login";
                //    var result = new { Success = true, Message = "OFF", urlRedirect = srvName2 };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
                //int Id_Usuario = 0;
                //string nombreUsuario = ""
                //    , perfiles = "";
                //Helper.ObtieneNombrePerfilesUsuario(cookie2, ref nombreUsuario, ref perfiles, ref Id_Usuario);

                #endregion

                //string[] myCookies = Request.Cookies.AllKeys;
                //foreach (string cookie in myCookies)
                //{
                //    Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
                //}
                //string srvName = HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["srvrName"].ToString()) + "Login";
                //return new RedirectResult(srvName); ;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }  


    public class AuthorizationPrivilegeFilter : ActionFilterAttribute
    {
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;
        WindowsImpersonationContext impersonationContext;

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

        string srvName = ConfigurationManager.AppSettings["srvrName"].ToString();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["flagTypeConection"].Trim().Equals("Y"))
            {
                string userIni = Soriana.FWK.Seguridad.HelperEncriptar.Desencriptar(filterContext.HttpContext.Request.Cookies["CookieLogin"]["User"]);
                string passIni = Soriana.FWK.Seguridad.HelperEncriptar.Desencriptar(filterContext.HttpContext.Request.Cookies["CookieLogin"]["Pass"]);

                if (impersonateValidUser(userIni, System.Configuration.ConfigurationManager.AppSettings["Domain"], passIni))
                {
                    Soriana.FWK.Datos.SQL.SqlHelper.connection_Name(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"]]);
                }
                else
                {
                    //srvName += Soriana.FWK.Seguridad.HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["NombreSitio"].ToString()) + "/Error/NotAccess";
                    srvName += ConfigurationManager.AppSettings["NombreSitio"].ToString() + "/Error/NotAccess";
                    filterContext.Result = new RedirectResult(srvName);
                    return;
                }
            }
            else
            {
                Soriana.FWK.Datos.SQL.SqlHelper.connection_Name(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"]]);
            }

            base.OnActionExecuting(filterContext);
        }

        private bool impersonateValidUser(String userName, String domain, String password)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (RevertToSelf())
            {
                if (LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
                LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                {
                    if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        impersonationContext = tempWindowsIdentity.Impersonate();
                        if (impersonationContext != null)
                        {
                            CloseHandle(token);
                            CloseHandle(tokenDuplicate);
                            return true;
                        }
                    }
                }
            }
            if (token != IntPtr.Zero)
                CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero)
                CloseHandle(tokenDuplicate);
            return false;
        }
    }

}
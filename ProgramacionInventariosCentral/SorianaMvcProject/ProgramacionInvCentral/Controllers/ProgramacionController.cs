using System;
using System.Collections.Generic;
using System.Web.Mvc;

using EDSorianaMvcProject.ProgramacionInvCentral;
//using DTSorianaMvcProject.ProgramacionInvCentral;
using NGSorianaMvcProject.ProgramacionInvCentral;

using System.Linq;
using System.IO;
using WebMvc.Menu;
using System.Configuration;
//using Newtonsoft.Json;
using System.Data;
using System.Web;
//using System.Data.SqlClient;


namespace WebMvc.ProgramacionInvCentral.Controllers
{
    public class ProgramacionInvCentralController : Controller
    {
        //int verificador = -1;

        //[CheckSessionOut]
        public ActionResult ValidaEtiquetasInsertos()
        {

            FmkTools.SqlHelper.connection_Name(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["Ambiente"]]);
            DataSet ds = new DataSet();
            System.Collections.Hashtable parametros = new System.Collections.Hashtable();
            /*parametros.Add("@keydown ", keydown);*/
            parametros.Add("@IdNumTipoProm", "15");
            parametros.Add("@IdNumFmtoPapel", "1");

            try
            {
                ds = FmkTools.SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "upTdaAe_Sel_ValidaCantEtiq", false, parametros);
                Boolean flagDatosIn = false;
                if (ds != null) { if (ds.Tables.Count > 0) { if (ds.Tables[0].Rows.Count > 0) { flagDatosIn = true; } } }

                if (flagDatosIn)
                {
                    return Json(ds.Tables[0].Rows[0][0].ToString(), JsonRequestBehavior.AllowGet);
                }
                else
                { return Json("0", JsonRequestBehavior.AllowGet); }
            }
            catch (Exception ex)
            { throw ex; }


        }

        public ActionResult Index()
        {

            TempData["PageTitle"] = "Programación Inventarios Rotativos";
            return View();
        }

		public ActionResult ProgramacionRotativo(string opcion)
        {

            List<EDRInventario> listInv = new List<EDRInventario>();
            ViewBag.ListProducts = listInv;
            ViewBag.BtnValidar = "disabled";

            return View();
        }


        [HttpPost]
        public ActionResult ProgramacionRotativo(HttpPostedFileBase excelfile)
        {
            List<EDRInventario> listInv = new List<EDRInventario>();
            ViewBag.ListProducts = listInv;

            //ViewBag.PersonList = new SelectList(llenarTipoInv(), "Id_TipoInv", "Desc_TipoInv", "Pre");
            try
            {
                if (excelfile == null || excelfile.ContentLength == 0)
                {
                    ViewBag.Error = "Por favor seleccione un archivo con extensión CSV.";
                    return View("ProgramacionRotativo");
                }
                else
                {
                    if (excelfile.FileName.EndsWith("csv"))
                    {
                        string path = Server.MapPath("~/UploadedFiles/" + excelfile.FileName);
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);

                        excelfile.SaveAs(path);
                        //SaveToDbExcel(path);

                        Session["PathFileCSV"] = path;

                        var contents = System.IO.File.ReadAllText(path).Split('\n');
                        var csv = from line in contents
                                  select line.Split(',').ToList();
                        listInv = new List<EDRInventario>();

                        int headerRows = 1;
                        foreach (var row in csv.Skip(headerRows)
                            .TakeWhile(r => r.Count > 1 && r.Last().Trim().Length > 0))
                        {
                            //String zerothColumnValue = row[0]; // leftmost column
                            EDRInventario pR = new EDRInventario();
                            pR.Ids_Num_Un = Convert.ToInt32(row[0]);
                            pR.Desc_UN = row[1];
                            pR.Desc_Form = row[2];
                            pR.Desc_Tipo_Inv = row[3];
                            pR.Desc_Prov = row[4];
                            pR.Desc_Categ = row[5];
                            pR.Desc_Linea = row[6];
                            pR.Desc_Codigo = row[7];
                            pR.Desc_Frec = row[8];
                            pR.Fecx_IniInv = row[9];
                            pR.Fecx_FinInv = row[10];
                            listInv.Add(pR);
                        }

                        ViewBag.ListProducts = listInv;
                        ViewBag.btnValidar = "";
                        Session["listInv"] = listInv;

                        //return Json(listInv, JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("ProgramaInventarioo");
                    }
                    else
                    {
                        ViewBag.BtnValidar = "disabled";
                        ViewBag.Error = "El tipo de archivo es incorrecto.";

                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(500, ex.Message);
            }

            return View();
        }

        [HttpGet]
        public ActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file, string vista)
        {
            try
            {
                HttpFileCollectionBase archivo = Request.Files;
                HttpPostedFileBase excelfile = archivo[0];

                if (excelfile.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(excelfile.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), Guid.NewGuid().ToString() + ".csv");// _FileName);
                    excelfile.SaveAs(_path);
                    SaveToDbExcel(_path);

                }

                ViewBag.Message = "File Uploaded Successfully!!";
                return RedirectToAction(vista, "ProgramacionInvCentral");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
                return RedirectToAction(vista, "ProgramacionInvCentral");
            }
        }


        private bool SaveToDbExcel(string _path)
        {
            System.Collections.Hashtable listErrors = new System.Collections.Hashtable();

            using (StreamReader fielRead = new StreamReader(_path))//@"c:\carpeta\usuario.txt"))
            {
                String line;
                int inde = 0;
                bool flagValidaciones = true;
                while ((line = fielRead.ReadLine()) != null)
                {
                    string[] lines = line.Split(',');
                    if (inde > 0)
                    {
                        if (lines[3].Equals("ROT"))
                        {
                            System.Collections.Hashtable result = ValidaRotativo(line);

                            if (result.Count > 0)
                            {
                                flagValidaciones = false;
                                if (FmkTools.Converters.CheckNullOrEmpty(TempData["listErrores"]))
                                {
                                    TempData["listErrores"] = result;
                                }
                            }
                        }
                        else
                        {
                            System.Collections.Hashtable result = ValidaGral(line);

                            if (result.Count > 0)
                            {
                                flagValidaciones = false;
                                if (FmkTools.Converters.CheckNullOrEmpty(TempData["listErrores"]))
                                {
                                    TempData["listErrores"] = result;
                                }
                            }
                        }
                    }
                    inde++;
                }

                if (flagValidaciones)
                {

                    return true;
                }
                else
                { return false; }
            }


        }

        private bool IsDate(string inputDate)
        {
            DateTime dt;
            bool isdate = DateTime.TryParse(inputDate, out dt);
            return isdate;
        }

        private System.Collections.Hashtable ValidaGral(string line)
        {

            System.Collections.Hashtable listTipoInvetario = new System.Collections.Hashtable();

            listTipoInvetario.Add("MEN", "Mensual");
            listTipoInvetario.Add("GEN", "General");
            listTipoInvetario.Add("PRE", "Preconteo");

            System.Collections.Hashtable listErrors = new System.Collections.Hashtable();

            string[] lines = line.Split(',');

            DataSet ds = Tiendas();

            bool flagExiste = false;

            //valida que sea una tienda valida
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                if (Convert.ToInt32(r[0]).Equals(Convert.ToInt32(lines[0])))
                {
                    flagExiste = true;
                    break;
                }
            }

            if (!flagExiste)
            {
                //listErrors.Add("La tienda no existe"
                //    , FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                //    + ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));

                listErrors.Add("La tienda no existe", "La tienda no existe " + lines[0].ToString() + " " + lines[1].ToString());
             }

            //valida tipo de inventario
            if (!listTipoInvetario.ContainsKey(lines[3]))
            {
                //listErrors.Add("Tipo de Inventario no valido"
                //        , FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                //        + ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                listErrors.Add("Tipo de Inventario no valido", 
                               "Tipo de Inventario no valido " + lines[0].ToString() + " " + lines[1].ToString());
            }


            //Alcance de Inventario Nombre  Fecha Inicio    Fecha Fin   Fecha Inicio    Fecha Fin   Valida Valida
            //GES General Externo Soriana       1               1           N / A           N / A   VERDADERO N/ A
            //GEI General Externo Integradas    1               2           N / A           N / A   VERDADERO N/ A
            //PRE Preconteos                    1               16          N / A           N / A   VERDADERO N/ A
            //GI1 General Interno  1 día        1               2           N / A           N / A   VERDADERO N/ A
            //GI2 General Interno 2 días        1               2           2               3       VERDADERO VERDADERO
            //M1S Mensual 1 día Soriana         1               1           N / A           N / A   VERDADERO N/ A
            //M2I Mensual 2 días Integradas     1               2           2               3       VERDADERO VERDADERO
            //M2N Mensual 2 días Nuevo Total    1               1           2               2       VERDADERO VERDADERO

            //valida el tipo de alcance
            switch (lines[4].ToString().Trim())
            {
                case "GES":
                    //valida fechas dia 1 diferente de null
                    if (FmkTools.Converters.CheckNullOrEmpty(lines[5]) || FmkTools.Converters.CheckNullOrEmpty(lines[6]))
                    {
                        //listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1", 
                                       "Hace falta definir una fecha Inicio o Fin del Dia 1 " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    if (!IsDate(lines[5]))
                    {
                        listErrors.Add("Fecha Inicio 1 no es una fecha valida",
                                       "Fecha Inicio 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    if (!IsDate(lines[6]))
                    {
                        listErrors.Add("Fecha Fin 1 no es una fecha valida",
                                       "Fecha Fin 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    //valida fechas dia 2 diferente de null
                    if (lines[7] != "--" && lines[8] != "--")
                    {
                        if (!FmkTools.Converters.CheckNullOrEmpty(lines[7]) || !FmkTools.Converters.CheckNullOrEmpty(lines[8]))
                        {
                            //listErrors.Add("Las fechas del dia 2 no deben definirse para este tipo de alcance"
                            //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                            //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                            listErrors.Add("Las fechas del dia 2 no deben definirse para este tipo de alcance",
                                           "Las fechas del dia 2 no deben definirse para este tipo de alcance " + lines[0].ToString() + " " + lines[1].ToString());
                        }
                    }
                    

                    //valida la diferencia de dias en fecha inicio y fin
                    if ((Convert.ToDateTime(lines[6]).DayOfYear - Convert.ToDateTime(lines[5]).DayOfYear) == 0)
                    {
                        //listErrors.Add("La fecha fin del dia 1 esta mal definida"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("La fecha fin del dia 1 esta mal definida", 
                                       "La fecha fin del dia 1 esta mal definida " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    break;
                case "GEI":
                case "GI1":
                    //valida fechas dia 1 diferente de null
                    if (FmkTools.Converters.CheckNullOrEmpty(lines[5]) || FmkTools.Converters.CheckNullOrEmpty(lines[6]))
                    {
                        //listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1", 
                                       "Hace falta definir una fecha Inicio o Fin del Dia 1 " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    if (!IsDate(lines[5]))
                    {
                        listErrors.Add("Fecha Inicio 1 no es una fecha valida",
                                       "Fecha Inicio 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    if (!IsDate(lines[6]))
                    {
                        listErrors.Add("Fecha Fin 1 no es una fecha valida",
                                       "Fecha Fin 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    //valida fechas dia 2 diferente de null
                    if (!FmkTools.Converters.CheckNullOrEmpty(lines[7]) || !FmkTools.Converters.CheckNullOrEmpty(lines[8]))
                    {
                        //listErrors.Add("Las fechas del dia 2 no deben definirse para este tipo de alcance"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Las fechas del dia 2 no deben definirse para este tipo de alcance", 
                                       "Las fechas del dia 2 no deben definirse para este tipo de alcance " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    //valida la diferencia de dias en fecha inicio y fin
                    if ((Convert.ToDateTime(lines[6]).DayOfYear - Convert.ToDateTime(lines[5]).DayOfYear) == 1)
                    {
                        //listErrors.Add("La fecha fin del dia 1 esta mal definida"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("La fecha fin del dia 1 esta mal definida", 
                                       "La fecha fin del dia 1 esta mal definida " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    break;
                case "PRE":
                    //valida fechas dia 1 diferente de null
                    if (FmkTools.Converters.CheckNullOrEmpty(lines[5]) || FmkTools.Converters.CheckNullOrEmpty(lines[6]))
                    {
                        //listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1", 
                                       "Hace falta definir una fecha Inicio o Fin del Dia 1 " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    if (!IsDate(lines[5]))
                    {
                        listErrors.Add("Fecha Inicio 1 no es una fecha valida",
                                       "Fecha Inicio 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    if (!IsDate(lines[6]))
                    {
                        listErrors.Add("Fecha Fin 1 no es una fecha valida",
                                       "Fecha Fin 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    //valida fechas dia 2 diferente de null
                    if (!FmkTools.Converters.CheckNullOrEmpty(lines[7]) || !FmkTools.Converters.CheckNullOrEmpty(lines[8]))
                    {
                        //listErrors.Add("Las fechas del dia 2 no deben definirse para este tipo de alcance"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Las fechas del dia 2 no deben definirse para este tipo de alcance", 
                                       "Las fechas del dia 2 no deben definirse para este tipo de alcance " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    //valida la diferencia de dias en fecha inicio y fin
                    if ((Convert.ToDateTime(lines[6]).DayOfYear - Convert.ToDateTime(lines[5]).DayOfYear) == 15)
                    {
                        //listErrors.Add("La fecha fin del dia 1 esta mal definida"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("La fecha fin del dia 1 esta mal definida", 
                                       "La fecha fin del dia 1 esta mal definida " + lines[0].ToString() + " " + lines[1].ToString());
                    }
                    break;
                case "GI2":
                case "M2I":
                    //valida fechas dia 1 diferente de null
                    if (FmkTools.Converters.CheckNullOrEmpty(lines[5]) || FmkTools.Converters.CheckNullOrEmpty(lines[6]))
                    {
                        //listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1", 
                                       "Hace falta definir una fecha Inicio o Fin del Dia 1 " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    if (!IsDate(lines[5]))
                    {
                        listErrors.Add("Fecha Inicio 1 no es una fecha valida",
                                       "Fecha Inicio 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    if (!IsDate(lines[6]))
                    {
                        listErrors.Add("Fecha Fin 1 no es una fecha valida",
                                       "Fecha Fin 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    //valida fechas dia 2 diferente de null
                    if (FmkTools.Converters.CheckNullOrEmpty(lines[7]) || FmkTools.Converters.CheckNullOrEmpty(lines[8]))
                    {
                        //listErrors.Add("Las fechas del dia 2 no deben definirse para este tipo de alcance"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 2",
                                       "Hace falta definir una fecha Inicio o Fin del Dia 2 " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    //valida la diferencia de dias en fecha inicio y fin 1
                    if ((Convert.ToDateTime(lines[6]).DayOfYear - Convert.ToDateTime(lines[5]).DayOfYear) == 0)
                    {
                        //listErrors.Add("La fecha fin del dia 1 esta mal definida"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("La fecha fin del dia 1 esta mal definida", 
                                       "La fecha fin del dia 1 esta mal definida " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    if (!IsDate(lines[7]))
                    {
                        listErrors.Add("Fecha Inicio 2 no es una fecha valida",
                                       "Fecha Inicio 2 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    if (!IsDate(lines[8]))
                    {
                        listErrors.Add("Fecha Fin 2 no es una fecha valida",
                                       "Fecha Fin 2 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    //valida la diferencia de dias en fecha inicio y fin 2
                    if ((Convert.ToDateTime(lines[8]).DayOfYear - Convert.ToDateTime(lines[7]).DayOfYear) == 0)
                    {
                        //listErrors.Add("La fecha fin del dia 1 esta mal definida"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("La fecha fin del dia 2 esta mal definida", 
                                       "La fecha fin del dia 2 esta mal definida " + lines[0].ToString() + " " + lines[1].ToString());
                    }
                    break;
                case "M1S":
                    //valida fechas dia 1 diferente de null
                    if (FmkTools.Converters.CheckNullOrEmpty(lines[5]) || FmkTools.Converters.CheckNullOrEmpty(lines[6]))
                    {
                        //listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1", 
                                       "Hace falta definir una fecha Inicio o Fin del Dia 1 " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    if (!IsDate(lines[5]))
                    {
                        listErrors.Add("Fecha Inicio 1 no es una fecha valida",
                                       "Fecha Inicio 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    if (!IsDate(lines[6]))
                    {
                        listErrors.Add("Fecha Fin 1 no es una fecha valida",
                                       "Fecha Fin 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    //valida fechas dia 2 diferente de null
                    if (!FmkTools.Converters.CheckNullOrEmpty(lines[7]) || !FmkTools.Converters.CheckNullOrEmpty(lines[8]))
                    {
                        //listErrors.Add("Las fechas del dia 2 no deben definirse para este tipo de alcance"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Las fechas del dia 2 no deben definirse para este tipo de alcance", 
                                       "Las fechas del dia 2 no deben definirse para este tipo de alcance " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    //valida la diferencia de dias en fecha inicio y fin
                    if ((Convert.ToDateTime(lines[6]).DayOfYear - Convert.ToDateTime(lines[5]).DayOfYear) == 0)
                    {
                        //listErrors.Add("La fecha fin del dia 1 esta mal definida"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("La fecha fin del dia 1 esta mal definida", 
                                       "La fecha fin del dia 1 esta mal definida " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    break;
                case "M2N":

                    //valida fechas dia 1 diferente de null
                    if (FmkTools.Converters.CheckNullOrEmpty(lines[5]) || FmkTools.Converters.CheckNullOrEmpty(lines[6]))
                    {
                        //listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 1", 
                                       "Hace falta definir una fecha Inicio o Fin del Dia 1 " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    if (!IsDate(lines[5]))
                    {
                        listErrors.Add("Fecha Inicio 1 no es una fecha valida",
                                       "Fecha Inicio 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    if (!IsDate(lines[6]))
                    {
                        listErrors.Add("Fecha Fin 1 no es una fecha valida",
                                       "Fecha Fin 1 no es una fecha valida " + lines[0].ToString() + " " + lines[1].ToString());
                        break;
                    }

                    //valida fechas dia 2 diferente de null
                    if (FmkTools.Converters.CheckNullOrEmpty(lines[7]) || FmkTools.Converters.CheckNullOrEmpty(lines[8]))
                    {
                        //listErrors.Add("Las fechas del dia 2 no deben definirse para este tipo de alcance"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("Hace falta definir una fecha Inicio o Fin del Dia 2",
                                       "Hace falta definir una fecha Inicio o Fin del Dia 2 " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    //valida la diferencia de dias en fecha inicio y fin dia 1
                    if ((Convert.ToDateTime(lines[6]).DayOfYear - Convert.ToDateTime(lines[5]).DayOfYear) != 1)
                    {
                        //listErrors.Add("La fecha fin del dia 1 esta mal definida"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("La fecha fin del dia 1 esta mal definida", 
                                       "La fecha fin del dia 1 esta mal definida " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    //valida la diferencia de dias en fecha inicio y fin dia 2
                    if ((Convert.ToDateTime(lines[8]).DayOfYear - Convert.ToDateTime(lines[7]).DayOfYear) != 1)
                    {
                        //listErrors.Add("La fecha fin del dia 1 esta mal definida"
                        //, FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        //+ ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                        listErrors.Add("La fecha fin del dia 2 esta mal definida", 
                                       "La fecha fin del dia 2 esta mal definida " + lines[0].ToString() + " " + lines[1].ToString());
                    }

                    break;
                default:
                    //listErrors.Add("El alcance no esta en el catalogo de alcances"
                    //    , FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                    //    + ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                    listErrors.Add("El alcance no esta en el catalogo de alcances", 
                                   "El alcance no esta en el catalogo de alcances " + lines[0].ToString() + " " + lines[1].ToString());
                    break;
            }

            //valida  alcance por grupo 


            return listErrors;
        }

        private System.Collections.Hashtable ValidaRotativo(string line)
        {
            NGRProgramaInventario _inventario = new NGRProgramaInventario();
            System.Collections.Hashtable listErrors = new System.Collections.Hashtable();
            string respuesta = "";
            string[] lines = line.Split(',');
            DataTable dt = ConvertCSVtoDataTable(line);
            string[] tiendas = dt.AsEnumerable().Select(s => s.Field<string>("Tienda")).ToArray<string>();
            respuesta = _inventario.ValidarTienda(tiendas);
            DataSet ds = Tiendas();

            bool flagExiste = false;
            foreach (DataRow r in ds.Tables[0].Rows)
            {

                if (Convert.ToInt32(r[0]).Equals(Convert.ToInt32(lines[0]))) { flagExiste = true; }

                if (!flagExiste)
                { TempData["MsgError"] = "La tienda no existe" + lines[0]; break; }

            }

            if (respuesta != "")
            {
                respuesta = "La Tienda " + respuesta + " no Existe";
                throw new Exception(respuesta);
            }
            else
            {
                string[] tipoInv = dt.AsEnumerable().Select(s => s.Field<string>("tipo de Inventario")).ToArray<string>();
                respuesta = _inventario.ValidarTipoInv(tipoInv);
                if (respuesta != "")
                {
                    respuesta = "El tipo de inventario " + respuesta + " no Existe";
                    throw new Exception(respuesta);
                }
                else
                {
                    string[] tipoAlc = dt.AsEnumerable().Select(s => s.Field<string>("Alcance de Inventario")).ToArray<string>();
                    respuesta = _inventario.ValidarAlcance(tipoAlc);
                    if (respuesta != "")
                    {
                        respuesta = "El tipo de alcance de inventario " + respuesta + " no Existe";
                        throw new Exception(respuesta);
                    }
                }
            }


            if (!lines[3].Equals("ROT")) { TempData["MsgError"] = "Tienda: " + line[0] + "-Tipo de Inventario no valido" + lines[3]; }

            if (!FmkTools.Converters.CheckNullOrEmpty(lines[4]))
            {
                if (!FmkTools.Converters.CheckNullOrEmpty(lines[7]))
                {
                    listErrors.Add("No se puede definir a nivel codigo si se elige proveedor"
                        , FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        + ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                    TempData["MsgError"] = listErrors;
                }
            }

            if (!FmkTools.Converters.CheckNullOrEmpty(lines[7]))
            {
                if (!(FmkTools.Converters.CheckNullOrEmpty(lines[5]) && FmkTools.Converters.CheckNullOrEmpty(lines[6])))
                {
                    listErrors.Add("No se puede definir nivel categoria o linea si se elige a nivel Codigo"
                        , FmkTools.Converters.CheckNullOrEmpty<string>(lines[0])
                        + ";" + FmkTools.Converters.CheckNullOrEmpty<string>(lines[1]));
                    TempData["MsgError"] = listErrors;
                }
            }

            return listErrors;
        }

        //private bool SaveDBFile(string _path)
        //{
        //    using (StreamReader fielRead = new StreamReader(_path))//@"c:\carpeta\usuario.txt"))
        //    {
        //        String line;
        //        int inde = 0;
        //        bool flagValidaciones = true;
        //        while ((line = fielRead.ReadLine()) != null)
        //        {
        //            string[] lines = line.Split(',');
        //            if (inde > 0)
        //            {

        //            }
        //            inde++;
        //        }

        //    }

        //    return true;
        //}

        private DataSet Tiendas()
        {

            FmkTools.SqlHelper.connection_Name(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["Ambiente"]]);
            DataSet ds = new DataSet();

            ds = FmkTools.SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_GetTiendaValida", false);

            return ds;
        }

        private Boolean VaidaFrecuencia(string frec, string fechaIni, string fechaFin)
        {
            if (frec.Equals("SEM"))
            {
                //Frecuencia de Inventario Nombre  Veces Periodicidad    Fecha Inicio    Fecha Fin
                //SEM Semestral   6   1 c / 30  12 - mar  8 - sep
                //UNI Unica   1   1   12 - mar  12 - mar

                //En semestral en la fecha fin debe sumar en automático 180 días 
                //    a la fecha inicial y el artículo lo programará 1 vez cada 30 días por seis ocasiones 
                //    y si es día domingo se recorre al lunes posterior


            }

            return true;

        }

        private Boolean ValidaAlcance(string prov, string cat = "", string lin = "", string cod = "")
        {
            //Alcance de Inventario Nombre
            //PRO Proveedor       En caso de que se elija Proveedor el usuario podrá definir el detalle a nivel línea o categoría
            //CAT Categoría
            //LIN Línea
            //COD Código


            return true;
        }

        public ActionResult IndexCoorp()
        {
            return View();
        }

        public ActionResult ProgramaPreconteo()
        {
            return View();
        }

        public ActionResult ProgramaInventarioGM(string opcion)
        {
            //TempData["PageTitle"] = "Programacion Inventarios Gen. y Mensual";

            List<EDRInventario> listInv = new List<EDRInventario>();
            ViewBag.ListProducts = listInv;
            ViewBag.PersonList = new SelectList(llenarTipoInv(), "Id_TipoInv", "Desc_TipoInv", opcion);
            ViewBag.BtnValidar = "disabled";

            return View();
        }

        public ActionResult ProgramaInventario()
        {
            NGRProgramaInventario _inventario = new NGRProgramaInventario();
            var tiendas = new SelectList(_inventario.ListTiendas(), "Id", "Descripcion");
            ViewData["ddlTiendas"] = tiendas;

            var tipoinv = new SelectList(_inventario.ListTipoInventario(), "Id", "Descripcion");
            ViewData["ddlTipoInv"] = tipoinv;

            ViewData["ddlConsecutivo"] = _inventario.TraerConsecutivo();

            return View();
        }

        [HttpPost]
        public JsonResult GuardarInventarioGeneral()
        {
            string _path = System.Web.HttpContext.Current.Session["PathFileCSV"].ToString();
            NGRProgramaInventario _inventario = new NGRProgramaInventario();
            DataSet ds = new DataSet();
            DataTable listInv = new DataTable();

            try
            {
                using (StreamReader fielRead = new StreamReader(_path))//@"c:\carpeta\usuario.txt"))
                {
                    String line;
                    int inde = 0;
                    listInv = new DataTable();
                    listInv.Columns.Add("Ids_Num_Un");
                    listInv.Columns.Add("Desc_UN");
                    listInv.Columns.Add("Desc_Form");
                    listInv.Columns.Add("Desc_Tipo_Inv");
                    listInv.Columns.Add("Desc_Tipo_Alc");
                    listInv.Columns.Add("Fecx_IniInv");
                    listInv.Columns.Add("Fecx_FinInv");
                    listInv.Columns.Add("Fecx_IniInv2");
                    listInv.Columns.Add("Fecx_FinInv2");

                    while ((line = fielRead.ReadLine()) != null)
                    {
                        string[] lines = line.Split(',');
                        if (inde > 0)
                        {
                            DataRow row = listInv.NewRow();

                            row["Ids_Num_Un"] = Convert.ToInt32(lines[0]);
                            row["Desc_UN"] = lines[1].ToString();
                            row["Desc_Form"] = lines[2].ToString();
                            row["Desc_Tipo_Inv"] = lines[3].ToString();
                            row["Desc_Tipo_Alc"] = lines[4].ToString();
                            row["Fecx_IniInv"] = lines[5].ToString();
                            row["Fecx_FinInv"] = lines[6].ToString();
                            row["Fecx_IniInv2"] = lines[7].ToString() == "--" ? string.Empty : lines[7].ToString();
                            row["Fecx_FinInv2"] = lines[8].ToString() == "--" ? string.Empty : lines[8].ToString();
                            listInv.Rows.Add(row);
                        }
                        inde++;
                    }
                }

                ds = _inventario.GuardarInventarioGeneral(listInv);

                var resultado = new { Success = true, Message = "Inventario guardado." };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var resultado = new { Success = false, Message = ex.Message };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }

        //GuardaInventarioRotativo...
        [HttpPost]
        public JsonResult GuardaInventarioRotativo(string file)
        {
            string _path = System.Web.HttpContext.Current.Session["PathFileCSV"].ToString();
            NGRProgramaInventario _inventarioR = new NGRProgramaInventario();
            DataSet ds = new DataSet();
            //List<EDRInventario> listInv = new List<EDRInventario>();
            DataTable listInv = new DataTable();

            try
            {

                using (StreamReader fielRead = new StreamReader(_path))
                {
                    String line;
                    int inde = 0;
                    //Encabezado de la tabla..
                    listInv = new DataTable();
                    listInv.Columns.Add("Ids_Num_Un");
                    listInv.Columns.Add("Desc_UN");
                    listInv.Columns.Add("Desc_Form");
                    listInv.Columns.Add("Desc_Tipo_Inv");
                    listInv.Columns.Add("Desc_Prov");
                    listInv.Columns.Add("Desc_Categ");
                    listInv.Columns.Add("Desc_Linea");
                    listInv.Columns.Add("Desc_Codigo");
                    listInv.Columns.Add("Desc_Frec");
                    listInv.Columns.Add("Fecx_IniInv");
                    listInv.Columns.Add("Fecx_FinInv");

                    while ((line = fielRead.ReadLine()) != null)
                    {
                        string[] lines = line.Split(',');
                        if (inde > 0)
                        {
                            DataRow row = listInv.NewRow();
                            //Aqui va el llamado al sp para grabar en BD
                            //EDRInventario p = new EDRInventario();
                            row["Ids_Num_Un"] = Convert.ToInt32(lines[0]);
                            row["Desc_UN"] = lines[1].ToString();
                            row["Desc_Form"] = lines[2].ToString();
                            row["Desc_Tipo_Inv"] = lines[3].ToString();
                            row["Desc_Prov"] = lines[4].ToString();
                            row["Desc_Categ"] = lines[5].ToString();
                            row["Desc_Linea"] = lines[6].ToString();
                            row["Desc_Codigo"] = lines[7].ToString();
                            row["Desc_Frec"] = lines[8].ToString();
                            row["Fecx_IniInv"] = lines[9].ToString();
                            row["Fecx_FinInv"] = lines[10].ToString();
                            listInv.Rows.Add(row);

                            //ds = _inventario.GuardarInventarioGeneral(line[0].ToString(), line[5].ToString());
                        }
                        inde++;
                    }

                }

                ds = _inventarioR.GuardaInventarioRotativo(listInv);

                var resultado = new { Success = true, Message = "Inventario guardado." };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var resultado = new { Success = false, Message = ex.Message };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizationPrivilegeFilter]
        public ActionResult GuardarProgramaInventario(int TipoInv, int Anio, int Cons, string FechaIn, string FechaFin, int Tienda, int Alcance)
        {
            try
            {
                EDRInventario _Inv = new EDRInventario()
                {
                    Id_Tipo_Inv = TipoInv,
                    Id_Num_AnioInv = Anio,
                    Ids_Cnsc_Inv = Cons,
                    Fecx_IniInv = FechaIn,
                    Fecx_FinInv = FechaFin,
                    Ids_Num_Un = Tienda,
                    Id_Alcance = Alcance,
                    //Fec_Movto = new DateTime(),
                    Bit_AplicaInv = 1,
                    Id_Cnsc_Inv = Cons,
                    Id_Num_InvStat = 1
                };
                NGRProgramaInventario _inventario = new NGRProgramaInventario();
                bool result = _inventario.Guardar(_Inv);
                var message = new { Id = result ? "Ok" : "Error", message = result ? "Inventario Programado" : "Ha ocurrido un problema al guardar" };
                return Json(message, JsonRequestBehavior.AllowGet);
                //return View();
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        public JsonResult LlenarListaInventarios()
        {
            NGRProgramaInventario _inventario = new NGRProgramaInventario();
            try
            {
                return Json(_inventario.TraerInvProgramados(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new { Success = false, Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ProgramaInventarioGM(HttpPostedFileBase excelfile)
        {
            List<EDRInventario> listInv = new List<EDRInventario>();
            ViewBag.ListProducts = listInv;


            ViewBag.PersonList = new SelectList(llenarTipoInv(), "Id_TipoInv", "Desc_TipoInv", "Pre");
            try
            {
                if (excelfile == null || excelfile.ContentLength == 0)
                {
                    ViewBag.Error = "Por favor seleccione un archivo con extensión CSV.";
                    return View("ProgramaInventarioGM");
                }
                else
                {
                    if (excelfile.FileName.EndsWith("csv"))
                    {
                        string path = Server.MapPath("~/UploadedFiles/" + excelfile.FileName);
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);

                        excelfile.SaveAs(path);
                        //SaveToDbExcel(path);

                        Session["PathFileCSV"] = path;

                        var contents = System.IO.File.ReadAllText(path).Split('\n');
                        var csv = from line in contents
                                  select line.Split(',').ToList();
                        listInv = new List<EDRInventario>();

                        int headerRows = 1;
                        foreach (var row in csv.Skip(headerRows)
                            .TakeWhile(r => r.Count > 1 && r.Last().Trim().Length > 0))
                        {
                            //String zerothColumnValue = row[0]; // leftmost column
                            EDRInventario p = new EDRInventario();
                            p.Ids_Num_Un = Convert.ToInt32(row[0]);
                            p.Desc_UN = row[1];
                            p.Desc_Form = row[2];
                            p.Desc_Tipo_Inv = row[3];
                            p.Desc_Tipo_Alc = row[4];
                            p.Fecx_IniInv = row[5];
                            p.Fecx_FinInv = row[6];
                            p.Fecx_IniInv2 = row[7];
                            p.Fecx_FinInv2 = row[8];
                            listInv.Add(p);
                        }

                        ViewBag.ListProducts = listInv;
                        ViewBag.BtnValidar = "";
                        //Session["listInv"] = listInv;
                    }
                    else
                    {
                        ViewBag.BtnValidar = "disabled";
                        ViewBag.Error = "El tipo de archivo es incorrecto.";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(500, ex.Message);
            }

            return View();
        }

        public ActionResult InventarioPreconteo()
        {
            //ViewBag.PersonList = new SelectList(hj(), "Id_TipoInv", "Desc_TipoInv", "Pre");
            //return View();
            return View();
        }

        public ActionResult InventarioMensual()
        {
            ViewData["opcion"] = "Men";
            ViewBag.PersonList = new SelectList(llenarTipoInv(), "Id_TipoInv", "Desc_TipoInv", ViewData["opcion"]);
            return View();

            //return View();
        }

        public ActionResult InventarioGeneral()
        {
            ViewData["opcion"] = "Gen";
            ViewBag.PersonList = new SelectList(llenarTipoInv(), "Id_TipoInv", "Desc_TipoInv", ViewData["opcion"]);
            return View();
        }

        public ActionResult InventarioCiclico()
        {
            ViewData["columnas"] = 11;
            //ViewData["titulo"] = "Inventario Ciclico";
            return View("ProgramaPreconteo");
        }

        private List<EDRTipoInventario> llenarTipoInv()
        {
            var options = new List<EDRTipoInventario>();
            options.Add(new EDRTipoInventario() { Id_TipoInv = "Pre", Desc_TipoInv = "Preconteo" });
            options.Add(new EDRTipoInventario() { Id_TipoInv = "Men", Desc_TipoInv = "Mensual" });
            options.Add(new EDRTipoInventario() { Id_TipoInv = "Gen", Desc_TipoInv = "General" });
            return options;
        }

        EDRArchivo excel = new EDRArchivo();

        [HttpPost]
        public JsonResult ValidaArchivoGeneral()
        {
            string _path = System.Web.HttpContext.Current.Session["PathFileCSV"].ToString();
            bool flagValidaciones = true;
            string listaErrores = "";

            try
            {
                using (StreamReader fielRead = new StreamReader(_path))//@"c:\carpeta\usuario.txt"))
                {
                    String line;
                    int inde = 0;
                    
                    while ((line = fielRead.ReadLine()) != null)
                    {
                        string[] lines = line.Split(',');
                        if (inde > 0)
                        {
                            System.Collections.Hashtable result = ValidaGral(line);

                            if (result.Count > 0)
                            {
                                flagValidaciones = false;
                                if (FmkTools.Converters.CheckNullOrEmpty(TempData["listErrores"]))
                                {
                                    foreach (string key in result.Keys)
                                    {
                                        listaErrores = listaErrores + (string)result[key] + "<br />";
                                    }
                                }
                            }
                        }
                        inde++;
                    }
                }

                var resultado = new { Success = true, Message = "Archivo validado." };
                if (!flagValidaciones)
                {
                    resultado = new { Success = false, Message = listaErrores };
                }
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var resultado = new { Success = false, Message = ex.Message };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }
		
		[HttpPost]
        public JsonResult ValidaArchivoRot(string file)
        {
            string _path = System.Web.HttpContext.Current.Session["PathFileCSV"].ToString();
            bool flagValidaciones = true;
            string listaErrores = "";
           
            try
            {
                using (StreamReader fielRead = new StreamReader(_path))//@"c:\carpeta\usuario.txt"))
                {
                    String line;
                    int inde = 0;

                    while ((line = fielRead.ReadLine()) != null)
                    {
                        string[] lines = line.Split(',');
                        if (inde > 0)
                        {
                            System.Collections.Hashtable result = ValidaRotativo(line);

                            if (result.Count > 0)
                            {
                                flagValidaciones = false;
                                if (FmkTools.Converters.CheckNullOrEmpty(TempData["listErrores"]))
                                {
                                    foreach (string key in result.Keys)
                                    {
                                        listaErrores = listaErrores + (string)result[key] + "<br />";
                                    }
                                }
                            }
                        }
                        inde++;
                    }
                }

                var resultado = new { Success = true, Message = "Archivo validado." };
                if (!flagValidaciones)
                {
                    resultado = new { Success = false, Message = listaErrores };
                }
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var resultado = new { Success = false, Message = ex.Message };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        //[AuthorizationPrivilegeFilter]
        public ActionResult import(string vista = "")
        {
            List<EDRInventario> listInv = new List<EDRInventario>();

            HttpFileCollectionBase archivo = Request.Files;
            HttpPostedFileBase excelfile = archivo[0];

            string path = Server.MapPath("~/UploadedFiles/" + excelfile.FileName);
            excel.Ruta = path;

            try
            {
                IEnumerable<List<string>> csv = null;
                string error = ValidarArchivo(excelfile, vista, path, ref csv);

                if (error == "")
                {
                    //var contents = System.IO.File.ReadAllText(path, System.Text.Encoding.Default).Split('\n');
                    //var csv = from line in contents select line.Split(',').ToList();
                    int headerRows = 1;
                    foreach (var row in csv.Skip(headerRows)
                        .TakeWhile(r => r.Count > 1 && r.Last().Trim().Length > 0))
                    {
                        //int count = row.Count;
                        EDRInventario p = new EDRInventario();
                        p.Ids_Num_Un = Convert.ToInt32(row[0]);
                        p.Desc_UN = row[1];
                        p.Desc_Form = row[2];
                        p.Desc_Tipo_Inv = row[3];
                        if (vista == "Gen")
                        {
                            p.Desc_Tipo_Alc = row[4];
                            p.Fecx_IniInv = row[5];
                            p.Fecx_FinInv = row[6];
                        }
                        else
                        {
                            p.Desc_Prov = row[4];
                            p.Desc_Categ = row[5];
                            p.Desc_Linea = row[6];
                            p.Desc_Codigo = row[7];
                            p.Desc_Frec = row[8];
                            p.Fecx_IniInv = row[9];
                            p.Fecx_FinInv = row[10];
                        }
                        listInv.Add(p);
                    }
                }
                else
                {
                    //throw new Exception(error);
                    ViewBag.Error = error;
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
            finally
            {
                //if (System.IO.File.Exists(path))
                //{
                //    System.IO.File.Delete(path);
                //}
            }
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(listInv);
            //DataTable pDt = JsonConvert.DeserializeObject<DataTable>(json);
            return Json(listInv, JsonRequestBehavior.AllowGet);
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath, System.Text.Encoding.Default))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Trim().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        private string ValidarArchivo(HttpPostedFileBase excelfile, string vista, string path, ref IEnumerable<List<string>> csv)
        {
            string error = "";
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                error = "Seleccione un archivo Excel.";
            }
            else
            {
                if (excelfile.FileName.EndsWith("csv"))
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    excelfile.SaveAs(path);

                    var contents = System.IO.File.ReadAllText(path, System.Text.Encoding.Default).Split('\n');
                    csv = from line in contents select line.Split(',').ToList();

                    int numCol = csv.FirstOrDefault().Count();
                    int totalcol = vista == "Gen" ? 9 : 11;
                    if (numCol != totalcol)
                    {
                        error = "El archivo no coincide, numero de columnas incorrecto.";
                    }
                }
                else
                {
                    error = "Archivo Incorrecto.";
                }
            }

            return error;
        }

        public ActionResult ValidarDatos(string archivo)
        {
            string respuesta = "";
            try
            {
                string path = Server.MapPath("~/Content/" + archivo);
                NGRProgramaInventario _inventario = new NGRProgramaInventario();
                DataTable dt = ConvertCSVtoDataTable(path);
                string[] tiendas = dt.AsEnumerable().Select(s => s.Field<string>("Tienda")).ToArray<string>();
                respuesta = _inventario.ValidarTienda(tiendas);
                if (respuesta != "")
                {
                    respuesta = "La Tienda " + respuesta + " no Existe";
                    throw new Exception(respuesta);
                }
                else
                {
                    string[] tipoInv = dt.AsEnumerable().Select(s => s.Field<string>("tipo de Inventario")).ToArray<string>();
                    respuesta = _inventario.ValidarTipoInv(tipoInv);
                    if (respuesta != "")
                    {
                        respuesta = "El tipo de inventario " + respuesta + " no Existe";
                        throw new Exception(respuesta);
                    }
                    else
                    {
                        string[] tipoAlc = dt.AsEnumerable().Select(s => s.Field<string>("Alcance de Inventario")).ToArray<string>();
                        respuesta = _inventario.ValidarAlcance(tipoAlc);
                        if (respuesta != "")
                        {
                            respuesta = "El tipo de alcance de inventario " + respuesta + " no Existe";
                            throw new Exception(respuesta);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
            }
            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult ConsultaProgramacion()
        //{
        //    return View();
        //}

        public ActionResult ConsultaProgramacion()
        {

            return View();

        }

        //[AuthorizationPrivilegeFilter]
        //public ActionResult TipoInventarios(string pFechaIni, string pFechaFin, string region, string tienda)
        //{
        //    try
        //    {
        //     //   List<Product_Custodia> items = new List<Product_Custodia>();
        //        DataSet dts = NGConsultasInv.NG_sp_tipoInv(DateTime.Parse(pFechaIni), DateTime.Parse(pFechaFin), region, tienda);
        //        int id_custodiaTmp = 0;
        //        string id_membresia = "";
        //        string total = "";
        //        string sku = "";


        //        foreach (DataTable table in dts.Tables)
        //        {
        //            foreach (DataRow dr in table.Rows)
        //            {
        //                {
        //                    if (id_custodiaTmp != int.Parse(dr["id_custodia"].ToString()) || id_membresia != dr["id_membresia"].ToString() || sku != dr["sku"].ToString())
        //                    {
        //                        int requiere_flete = 0;
        //                        id_custodiaTmp = int.Parse(dr["id_custodia"].ToString());
        //                        id_membresia = dr["id_membresia"].ToString();
        //                        sku = dr["sku"].ToString();
        //                        total = table.Compute("Sum(monto_en_custodia)", "id_custodia=" + id_custodiaTmp.ToString() + " AND  id_membresia='" + id_membresia + "' AND sku='" + sku + "'").ToString();

        //                        if (total == "")
        //                            total = "0";

        //                        if (dr["requiere_flete"].ToString() == "True")
        //                            requiere_flete = 1;

        //                        items.Add(new Product_Custodia()
        //                        {
        //                            id_region = dr["id_region"].ToString(),
        //                            id_un = int.Parse(dr["id_un"].ToString()),
        //                            id = int.Parse(dr["id_custodia"].ToString()),
        //                            monto_custodia = decimal.Parse(total),
        //                            nombre_socio = dr["nombre_socio"].ToString(),
        //                            id_membresia = dr["id_Membresia"].ToString(),
        //                            fecha_creacion = DateTime.Parse(dr["fecha_creacion"].ToString()),
        //                            nombre_usuario = dr["Colaborador"].ToString(),
        //                            nombre_usuario_autorizo = dr["Autorizo"].ToString(),
        //                            nombre_club = dr["nombre"].ToString(),
        //                            SKU = dr["sku"].ToString(),
        //                            desc_producto = dr["desc_producto"].ToString(),
        //                            cant_custodia = decimal.Parse(dr["cant_custodia"].ToString()),
        //                            fecha_entrega = DateTime.Parse(dr["fecha_entrega"].ToString()),
        //                            requiere_flete = requiere_flete

        //                        });

        //                    }
        //                }


        //            }
        //        }
        //        var result = new { Success = true, Message = "ok", Custodias = items };

        //        return Json(result, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        var result = new { Success = false, Message = ex.Message };
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }

        //}

        [HttpPost]
        public JsonResult ObtineInv()
        {

            try
            {
                var ListCInv = new List<EDRInventario>();

                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1001, Desc_UN = "Pachuca", Ids_Num_Un = 1, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1002, Desc_UN = "Saltillo", Ids_Num_Un = 2, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1003, Desc_UN = "Cumbres", Ids_Num_Un = 3, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1004, Desc_UN = "Lincon", Ids_Num_Un = 4, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1005, Desc_UN = "Tlaxcala", Ids_Num_Un = 5, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1006, Desc_UN = "La silla", Ids_Num_Un = 6, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1007, Desc_UN = "san pedro", Ids_Num_Un = 7, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1001, Desc_UN = "Pachuca", Ids_Num_Un = 1, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1002, Desc_UN = "Saltillo", Ids_Num_Un = 2, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1003, Desc_UN = "Cumbres", Ids_Num_Un = 3, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1004, Desc_UN = "Lincon", Ids_Num_Un = 4, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1005, Desc_UN = "Tlaxcala", Ids_Num_Un = 5, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1006, Desc_UN = "La silla", Ids_Num_Un = 6, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1007, Desc_UN = "san pedro", Ids_Num_Un = 7, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1001, Desc_UN = "Pachuca", Ids_Num_Un = 1, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1002, Desc_UN = "Saltillo", Ids_Num_Un = 2, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1003, Desc_UN = "Cumbres", Ids_Num_Un = 3, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1004, Desc_UN = "Lincon", Ids_Num_Un = 4, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1005, Desc_UN = "Tlaxcala", Ids_Num_Un = 5, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1006, Desc_UN = "La silla", Ids_Num_Un = 6, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1007, Desc_UN = "san pedro", Ids_Num_Un = 7, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1001, Desc_UN = "Pachuca", Ids_Num_Un = 1, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1002, Desc_UN = "Saltillo", Ids_Num_Un = 2, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1003, Desc_UN = "Cumbres", Ids_Num_Un = 3, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1004, Desc_UN = "Lincon", Ids_Num_Un = 4, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1005, Desc_UN = "Tlaxcala", Ids_Num_Un = 5, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1006, Desc_UN = "La silla", Ids_Num_Un = 6, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1007, Desc_UN = "san pedro", Ids_Num_Un = 7, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1001, Desc_UN = "Pachuca", Ids_Num_Un = 1, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1002, Desc_UN = "Saltillo", Ids_Num_Un = 2, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1003, Desc_UN = "Cumbres", Ids_Num_Un = 3, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1004, Desc_UN = "Lincon", Ids_Num_Un = 4, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1005, Desc_UN = "Tlaxcala", Ids_Num_Un = 5, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1006, Desc_UN = "La silla", Ids_Num_Un = 6, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1007, Desc_UN = "san pedro", Ids_Num_Un = 7, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1001, Desc_UN = "Pachuca", Ids_Num_Un = 1, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1002, Desc_UN = "Saltillo", Ids_Num_Un = 2, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1003, Desc_UN = "Cumbres", Ids_Num_Un = 3, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1004, Desc_UN = "Lincon", Ids_Num_Un = 4, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1005, Desc_UN = "Tlaxcala", Ids_Num_Un = 5, Desc_Tipo_Inv = "MEN", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1006, Desc_UN = "La silla", Ids_Num_Un = 6, Desc_Tipo_Inv = "GRAL", Fecx_IniInv = "06/06/2019" });
                ListCInv.Add(new EDRInventario() { Id_Tipo_Inv = 1007, Desc_UN = "san pedro", Ids_Num_Un = 7, Desc_Tipo_Inv = "PRE", Fecx_IniInv = "06/06/2019" });

                string sTabla = string.Empty;
                sTabla += "<thead>";
                sTabla += "<tr>";

                sTabla += "<th class='col-sm-2 col-md-2 col-lg-2' >No. Tienda</th>";
                sTabla += "<th class='col-sm-2 col-md-2 col-lg-2' >Tienda</th>";
                sTabla += "<th class='col-sm-2 col-md-2 col-lg-2' style='display:none;'>Id Inversion</th>";
                sTabla += "<th class='col-sm-2 col-md-2 col-lg-2' >Tipo Inventario</th>";
                sTabla += "<th class='col-sm-2 col-md-2 col-lg-2' >Fecha Inventario</th>";
                sTabla += "<th class='col-sm-1 col-md-1 col-lg-1 text-center' ><i class='fa fa-trash' style='font-size:16px;'></i></th>";
                sTabla += "</tr>";
                sTabla += "</thead>";

                //sTabla += "<tbody>";
                //int nCont = 1;

                sTabla += "<tbody>";

                foreach (EDRInventario itemCInv in ListCInv)
                {
                    sTabla += "<tr>";
                    sTabla += $"<td>{itemCInv.Id_Tipo_Inv.ToString()} </td>";
                    sTabla += $"<td>{itemCInv.Desc_UN} </td>";
                    sTabla += $"<td style='display:none;'>{itemCInv.Ids_Num_Un} </td>";
                    sTabla += $"<td>{itemCInv.Desc_Tipo_Inv} </td>";
                    sTabla += $"<td>{itemCInv.Fecx_IniInv} </td>";
                    sTabla += $"<td class='text-center'><button type='button' class='btn btn-danger btn-xs'>Cancelar</button></td>";
                    sTabla += $"</tr>";
                }

                sTabla += "</tbody>";

                var result = new { Success = true, Message = "ok", sTable = sTabla };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new { Success = false, Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }

    }
}

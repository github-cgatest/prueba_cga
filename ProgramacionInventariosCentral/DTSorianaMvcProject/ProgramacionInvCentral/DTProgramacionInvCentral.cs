using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Configuration;
using System.Data;
using System.Web;
using System.Data.SqlClient;
using EDSorianaMvcProject.ProgramacionInvCentral;

namespace DTSorianaMvcProject.ProgramacionInvCentral
{
    public class DTProgramacionInvCentral
    {
    }

    public class DTRProgramaInventario
    {
        string cadena = string.Empty;
        public DTRProgramaInventario()
        {
            //if (ConfigurationManager.AppSettings["flagTypeConection"].ToString().Equals("Y"))
            //{
            //    cadena = string.Format(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"].ToString()].ToString(), ConfigurationManager.AppSettings["DataSoruce_cs"].ToString(), ConfigurationManager.AppSettings["InitialCatalog_cs"].ToString());
            //}
            //else
            //{
            //    cadena = string.Format(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"].ToString()].ToString(), ConfigurationManager.AppSettings["DataSoruce_cs"].ToString(), ConfigurationManager.AppSettings["InitialCatalog_cs"].ToString(), ConfigurationManager.AppSettings["User_cs"].ToString(), Soriana.FWK.Seguridad.HelperEncriptar.Desencriptar(ConfigurationManager.AppSettings["Pass_cs"].ToString()));
            //}
            cadena = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["Ambiente"].ToString()];
            FmkTools.SqlHelper.connection_Name(cadena);
        }

        public DataTable ObtenerTiendas()
        {
            DataTable dt = new DataTable();
            try
            {

                string consulta = "SELECT id_Num_un, Cast(id_Num_un as Varchar(6)) + ' - ' + Cast(Desc_UN as Varchar(200)) AS descripcion "
                                + "FROM UN ORDER BY id_Num_un;";

                DataSet ds = FmkTools.SqlHelper.ExecuteDataSet(CommandType.Text, consulta, false);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public int ObtenerConsecutivo()
        {
            int id = 1;
            try
            {
                string consulta = "SELECT max(Id_Cnsc_Inv) AS consecutivo FROM Tda024ProdDB.dbo.Inv;";
                DataSet tabla = FmkTools.SqlHelper.ExecuteDataSet(CommandType.Text, consulta, false);
                if (tabla.Tables.Count > 0)
                {
                    id = Convert.ToInt32(tabla.Tables[0].Rows[0].ItemArray[0]) + 1;
                }
                // return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return id;
        }

        public DataTable ObtenerTipoInventario()
        {
            DataTable dt = new DataTable();
            try
            {

                string consulta = "SELECT Id_Tipo_Inv, Abrev_TipoInv FROM Tda024ProdDB.dbo.TipoInv  ORDER BY Id_Tipo_Inv;";

                DataSet ds = FmkTools.SqlHelper.ExecuteDataSet(CommandType.Text, consulta, false);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataSet GuardarInventarioGeneral(DataTable listInv)
        {
            DataSet ds = new DataSet();

            string cons = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["Ambiente"]];

            using (SqlConnection con = new SqlConnection(cons))
            {
                using (SqlCommand sqlComm = new SqlCommand("ProgramacionInventarios_iUP_CGA", con))
                {
                    sqlComm.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("@pTabla", SqlDbType.Structured)
                    {
                        TypeName = "dbo.param_Temp_invInvProg_GM",
                        Value = listInv
                    };
                    sqlComm.Parameters.Add(param);

                    con.Open();
                    //sqlComm.ExecuteReader();

                    var adapter = new SqlDataAdapter(sqlComm);
                    adapter.Fill(ds);

                }
            }

            return ds;

        }

        //GuardaInventarioRotativo..
        public DataSet GuardaInventarioRotativo(DataTable listInv)
        {
            DataSet ds = new DataSet();

            string cons = "Data Source=Desaoper; Initial Catalog=Tda024ProdDB; User Id=dbotienda; Password=grabar;";

            using (SqlConnection con = new SqlConnection(cons))
            {
                using (SqlCommand sqlComm = new SqlCommand("ProgramacionInventarios_iUP_CGA", con))
                {
                    sqlComm.CommandType = CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("@pTabla", SqlDbType.Structured)
                    {
                        TypeName = "dbo.param_Temp_invInvProg_GM",
                        Value = listInv
                    };
                    sqlComm.Parameters.Add(param);

                    con.Open();
                    //sqlComm.ExecuteReader();

                    var adapter = new SqlDataAdapter(sqlComm);
                    adapter.Fill(ds);

                }
            }

            return ds;

        }

        public bool GuardarInv(EDRInventario _Inv)
        {
            bool reg = false;
            try
            {
                string consulta = "INSERT INTO dbo.Inv (Ids_Cnsc_Inv, Ids_Num_UN, Id_Tipo_Inv, Id_Num_AnioInv, Id_Cnsc_Inv,Id_Num_InvStat,Id_Num_InvEvto,Fecx_IniInv,Fecx_FinInv, Stipo_Alcance, Id_Num_MtvoCancel, Fec_Movto, Bit_AplicaInv) "
                                + $"VALUES('{_Inv.Ids_Cnsc_Inv}','{_Inv.Ids_Num_Un}', '{_Inv.Id_Tipo_Inv}', '{_Inv.Id_Num_AnioInv}', '{_Inv.Id_Cnsc_Inv}','{_Inv.Id_Num_InvStat}','{_Inv.Id_Num_InvEvto}','{_Inv.Fecx_IniInv}','{_Inv.Fecx_FinInv}','{ _Inv.Id_Alcance}','{ _Inv.Id_Num_MtvoCancel}','{_Inv.Fec_Movto}','{ _Inv.Bit_AplicaInv}');";

                // reg = Convert.ToBoolean( sqlHelper.ExecuteNonQuery(CommandType.Text, consulta, true));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reg;
        }

        public DataTable ConsultaInvProgramados()
        {
            DataTable dt = new DataTable();
            try
            {

                string consulta = "SELECT inventario.Ids_Num_UN,tienda.Desc_UN, CASE WHEN inventario.Stipo_Alcance = 1 THEN 'Departamento' ELSE 'Categoria' END as Stipo_Alcance, inventario.Fecx_IniInv, inventario.Fecx_FinInv, 1 as Alcance "
                                    + "from Tda024ProdDB.dbo.Inv inventario "
                                    + "inner join un tienda on inventario.Ids_Num_UN = tienda.Id_Num_UN "
                                    + "where inventario.Id_Num_AnioInv = 2018; ";

                DataSet ds = FmkTools.SqlHelper.ExecuteDataSet(CommandType.Text, consulta, false);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable validarTiendas(string tiendas)
        {
            DataTable dt = new DataTable();
            try
            {
                string consulta = "SELECT id_Num_un "
                                + $"FROM UN WHERE id_Num_un = {tiendas};";

                DataSet ds = FmkTools.SqlHelper.ExecuteDataSet(CommandType.Text, consulta, false);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable ValidarTipoInv(string tipoInv)
        {
            DataTable dt = new DataTable();
            try
            {
                string consulta = "SELECT id_tipo_Inv "
                                + $"FROM TipoInv WHERE abrev_tipoinv like'%{tipoInv}%';";
                DataSet ds = FmkTools.SqlHelper.ExecuteDataSet(CommandType.Text, consulta, false);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable ValidarAlcance(string tipoAlc)
        {
            DataTable dt = new DataTable();
            try
            {
                string consulta = "SELECT id_tipo_Alcance "
                                + $"FROM invInvTipoAlcance WHERE abrev_tipoAlcance like'%{tipoAlc}%';";
                DataSet ds = FmkTools.SqlHelper.ExecuteDataSet(CommandType.Text, consulta, false);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
    }
}

using System.Data;
using sqlHelper = Soriana.FWK.Datos.SQL.SqlHelper;
using System.Configuration;
using System.Data.SqlClient;
using System;

namespace DTSenalizacionSoriana.CodigoSeleccionado
{
    public class DALCategoria
    {
        public DALCategoria()
        {
            string var = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"]];

            sqlHelper.connection_Name(ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["AmbienteSC"]]);
        }

        public static DataSet DAL_GpoCategList(bool keydown, int tipo, int? codigo, string desc, int IdNumTipoEtiqProm, int IdNumVigProm,int? IndDEPorImp, DateTime? FechaProc,int? IdNumDiv, ref int verificador)
        {
            DataSet ds = new DataSet();
            try
            {
                System.Collections.Hashtable parametros = new System.Collections.Hashtable();
                parametros.Add("@keydown ", keydown);
                parametros.Add("@tipo ", tipo);
                parametros.Add("@codigo ", codigo);
                parametros.Add("@desc ", desc);
                parametros.Add("@IdNumTipoEtiqProm", IdNumTipoEtiqProm);
                parametros.Add("@IdNumVigProm", IdNumVigProm);
                parametros.Add("@IndDEPorImp", IndDEPorImp);
                parametros.Add("@FechaProc", FechaProc);
                parametros.Add("@IdNumDiv", IdNumDiv);

                ds = sqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "upTdaAe_dd_GpoCateg_v2", false, parametros);
                verificador = 1;
                return ds;
            }
            catch (SqlException ex)
            {
                verificador = -1;
                throw ex;
            }
            catch (System.Exception ex)
            {
                verificador = -1;
                throw ex;
            }
        }

        public static DataSet DAL_CategList(bool keydown, int tipo, int? codigo, string desc, int IdNumTipoEtiqProm, int IdNumVigProm, int IdNumGpoCateg, int? IndDEPorImp, ref int verificador)
        {
            DataSet ds = new DataSet();
            try
            {
                System.Collections.Hashtable parametros = new System.Collections.Hashtable();
                parametros.Add("@keydown ", keydown);
                parametros.Add("@tipo ", tipo);
                parametros.Add("@codigo ", codigo);
                parametros.Add("@desc ", desc);
                parametros.Add("@IdNumTipoEtiqProm", IdNumTipoEtiqProm);
                parametros.Add("@IdNumVigProm", IdNumVigProm);
                parametros.Add("@IdNumGpoCateg", IdNumGpoCateg);
                parametros.Add("@IndDEPorImp", IndDEPorImp);

                ds = sqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "upTdaAe_dd_Categ", false, parametros);
                verificador = 1;
                return ds;
            }
            catch (SqlException ex)
            {
                verificador = -1;
                throw ex;
            }
            catch (System.Exception ex)
            {
                verificador = -1;
                throw ex;
            }
        }

        public static DataSet DAL_LineaList(bool keydown, int tipo, int? codigo, string desc, int IdNumTipoEtiqProm, int IdNumVigProm, int IdNumCateg, int? IndDEPorImp, ref int verificador)
        {
            DataSet ds = new DataSet();
            try
            {
                System.Collections.Hashtable parametros = new System.Collections.Hashtable();
                parametros.Add("@keydown ", keydown);
                parametros.Add("@tipo ", tipo);
                parametros.Add("@codigo ", codigo);
                parametros.Add("@desc ", desc);
                parametros.Add("@IdNumTipoEtiqProm", IdNumTipoEtiqProm);
                parametros.Add("@IdNumVigProm", IdNumVigProm);
                parametros.Add("@IdNumCateg", IdNumCateg);
                parametros.Add("@IndDEPorImp", IndDEPorImp);

                ds = sqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "upTdaAe_dd_Linea", false, parametros);
                verificador = 1;
                return ds;
            }
            catch (SqlException ex)
            {
                verificador = -1;
                throw ex;
            }
            catch (System.Exception ex)
            {
                verificador = -1;
                throw ex;
            }
        }


    }
}

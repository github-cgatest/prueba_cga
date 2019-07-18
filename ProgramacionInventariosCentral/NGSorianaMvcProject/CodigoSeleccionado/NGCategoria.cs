using DTSenalizacionSoriana.CodigoSeleccionado;
using EDSenalizacionSoriana.CodigoSeleccionado;
using System.Collections.Generic;
using System.Data;
using Soriana.FWK.Common;
using System.Linq;
using System.Data.SqlClient;
using System;

namespace NGSenalizacionSoriana.CodigoSeleccionado
{
    public static class NGCategoria
    {
        public static List<EDCategoria> NG_GpoCategList(bool keydown, int tipo, int? codigo, string desc, int IdNumTipoEtiqProm, int IdNumVigProm, int? IndDEPorImp, DateTime? FechaProc, int? IdNumDiv, ref int verificador)
        {
            DataSet dtCategoria = DALCategoria.DAL_GpoCategList(keydown, tipo, codigo, desc, IdNumTipoEtiqProm, IdNumVigProm, IndDEPorImp, FechaProc, IdNumDiv, ref verificador);

            try
            { 
                if (dtCategoria.Tables.Count > 0 )
                {
                    //agregar para columnas sin nombre en SPs
                    //foreach (DataTable table in Seguridad.Tables)
                    //{
                    //    table.Columns[0].ColumnName = "Nombre de la Columna";
                    //}
                    return dtCategoria.Tables[0].ToList<EDCategoria>();
                }
                else
                {
                    return null;
                }
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

        public static List<EDCategoria> NG_CategList(bool keydown, int tipo, int? codigo, string desc, int IdNumTipoEtiqProm, int IdNumVigProm, int IdNumGpoCateg, int? IndDEPorImp, ref int verificador)
        {
            DataSet dtCategoria = DALCategoria.DAL_CategList(keydown, tipo, codigo, desc, IdNumTipoEtiqProm, IdNumVigProm, IdNumGpoCateg, IndDEPorImp, ref verificador);

            try
            {
                if (dtCategoria.Tables.Count > 0)
                {
                    //agregar para columnas sin nombre en SPs
                    //foreach (DataTable table in Seguridad.Tables)
                    //{
                    //    table.Columns[0].ColumnName = "Nombre de la Columna";
                    //}
                    return dtCategoria.Tables[0].ToList<EDCategoria>();
                }
                else
                {
                    return null;
                }
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

        public static List<EDCategoria> NG_LineaList(bool keydown, int tipo, int? codigo, string desc, int IdNumTipoEtiqProm, int IdNumVigProm, int IdNumCateg, int? IndDEPorImp, ref int verificador)
        {
            DataSet dtlinea = DALCategoria.DAL_LineaList(keydown, tipo, codigo, desc, IdNumTipoEtiqProm, IdNumVigProm, IdNumCateg, IndDEPorImp, ref verificador);

            try
            {
                if (dtlinea.Tables.Count > 0)
                {
                    //agregar para columnas sin nombre en SPs
                    //foreach (DataTable table in Seguridad.Tables)
                    //{
                    //    table.Columns[0].ColumnName = "Nombre de la Columna";
                    //}
                    return dtlinea.Tables[0].ToList<EDCategoria>();
                }
                else
                {
                    return null;
                }
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

using DTSorianaMvcProject.ProgramacionInvCentral;
using EDSorianaMvcProject.ProgramacionInvCentral;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGSorianaMvcProject.ProgramacionInvCentral
{
    public class NGProgramacionInvCentral
    {
    }

    public class NGRProgramaInventario
    {
        DTRProgramaInventario _Inventario = new DTRProgramaInventario();

        public List<EDRConsultaListGenerica> ListTiendas()
        {
            List<EDRConsultaListGenerica> tiendas = new List<EDRConsultaListGenerica>();
            EDRConsultaListGenerica item = new EDRConsultaListGenerica();
            DataTable dt = new DataTable();
            try
            {
                dt = _Inventario.ObtenerTiendas();
                if (dt.Rows.Count > 0)
                {
                    item.Descripcion = "Seleccione";
                    item.Id = "-1";
                    tiendas.Add(item);
                    foreach (DataRow dr in dt.Rows)
                    {
                        item = new EDRConsultaListGenerica();

                        item.Descripcion = dr["descripcion"].ToString();
                        item.Id = dr["id_Num_un"].ToString();

                        tiendas.Add(item);
                    }

                }
                else
                {
                    item.Descripcion = "-- Sin Datos --";
                    item.Id = "0";
                    tiendas.Add(item);
                }
            }
            catch (System.Exception ex)
            {
                if (tiendas.Count <= 0)
                {
                    item.Descripcion = "-- Sin Datos --";
                    item.Id = "0";
                    tiendas.Add(item);
                }
                throw ex;
            }
            return tiendas;
        }

        public List<EDRConsultaListGenerica> ListTipoInventario()
        {
            return LlenarLista(_Inventario.ObtenerTipoInventario());
        }

        public int TraerConsecutivo()
        {
            return _Inventario.ObtenerConsecutivo();
        }

        private List<EDRConsultaListGenerica> LlenarLista(DataTable dt)
        {
            List<EDRConsultaListGenerica> lista = new List<EDRConsultaListGenerica>();
            EDRConsultaListGenerica item = new EDRConsultaListGenerica();
            try
            {
                //dt = _Inventario.ObtenerTiendas();
                if (dt.Rows.Count > 0)
                {
                    item.Descripcion = "Seleccione";
                    item.Id = "-1";
                    lista.Add(item);
                    foreach (DataRow dr in dt.Rows)
                    {
                        item = new EDRConsultaListGenerica();

                        item.Descripcion = dr[1].ToString();
                        item.Id = dr[0].ToString();

                        lista.Add(item);
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                lista.Clear();
                item.Descripcion = "-- Sin Datos --";
                item.Id = "0";
                lista.Add(item);
                throw ex;
            }

            return lista;
        }

        public bool Guardar(EDRInventario _Inv)
        {
            return _Inventario.GuardarInv(_Inv);
        }

        public DataSet GuardarInventarioGeneral(DataTable listInv)
        {
            return _Inventario.GuardarInventarioGeneral(listInv);
        }

        //GuardarInventarioRotativo
        public DataSet GuardaInventarioRotativo(DataTable listInv)
        {
            return _Inventario.GuardaInventarioRotativo(listInv);
        }

        public List<EDRInventario> TraerInvProgramados()
        {
            List<EDRInventario> lista = new List<EDRInventario>();
            EDRInventario item = new EDRInventario();
            try
            {
                DataTable dt = _Inventario.ConsultaInvProgramados();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        item = new EDRInventario()
                        {
                            Ids_Num_Un = Convert.ToInt32(dr["Ids_Num_UN"]),
                            Desc_UN = dr["Desc_UN"].ToString(),
                            Desc_Tipo_Alc = dr["Stipo_Alcance"].ToString(),
                            Fecx_IniInv = Convert.ToDateTime(dr["Fecx_IniInv"]).ToString("dd MMM yyyy").ToUpper(),
                            Fecx_FinInv = Convert.ToDateTime(dr["Fecx_FinInv"]).ToString("dd MMM yyyy").ToUpper(),
                            Id_Alcance = Convert.ToInt32(dr["Alcance"])
                        };
                        lista.Add(item);
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                lista.Clear();
                //item.Descripcion = "-- Sin Datos --";
                //item.Id = "0";
                //lista.Add(item);
                throw ex;
            }

            return lista;
        }

        public string ValidarTienda(string[] tiendas)
        {
            foreach (var item in tiendas)
            {
                DataTable dt = _Inventario.validarTiendas(item);
                if (dt.Rows.Count == 0)
                {
                    return item;
                }
            }
            return "";
        }

        public string ValidarTipoInv(string[] tipoInv)
        {
            foreach (var item in tipoInv)
            {
                DataTable dt = _Inventario.ValidarTipoInv(item.ToUpper());
                if (dt.Rows.Count == 0)
                {
                    return item;
                }
            }
            return "";
        }

        public string ValidarAlcance(string[] tipoAlc)
        {
            foreach (var item in tipoAlc)
            {
                DataTable dt = _Inventario.ValidarAlcance(item.ToUpper());
                if (dt.Rows.Count == 0)
                {
                    return item;
                }
            }
            return "";
        }
    }
}

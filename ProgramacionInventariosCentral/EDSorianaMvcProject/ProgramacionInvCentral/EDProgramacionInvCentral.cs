using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSorianaMvcProject.ProgramacionInvCentral
{
    public class EDProgramacionInvCentral
    {
    }

    public class EDRConsultaListGenerica
    {
        public string Descripcion { get; set; }
        public string Id { get; set; }
    }

    public class EDRTipoInventario
    {
        public string Id_TipoInv { get; set; }
        public string Desc_TipoInv { get; set; }

    }

    public class EDRInventario
    {
        public int Ids_Cnsc_Inv { get; set; }
        public int Ids_Num_Un { get; set; }
        public string Desc_UN { get; set; }
        public int Id_Tipo_Inv { get; set; }
        public string Desc_Tipo_Inv { get; set; }
        public int Id_Alcance { get; set; }
        public string Desc_Tipo_Alc { get; set; }
        public string Fecx_IniInv { get; set; }
        public string Fecx_FinInv { get; set; }
        public int Id_Num_MtvoCancel { get; set; }
        public DateTime Fec_Movto { get; } = DateTime.Now;
        public int Bit_AplicaInv { get; set; }
        public int Id_Num_AnioInv { get; set; }
        public int Id_Cnsc_Inv { get; set; }
        public int Id_Num_InvStat { get; set; }
        public int Id_Num_InvEvto { get; set; }
        public int Id_Form { get; set; }
        public string Desc_Form { get; set; }

        public int Id_Prov { get; set; }
        public string Desc_Prov { get; set; }
        public int Id_Categ { get; set; }
        public string Desc_Categ { get; set; }
        public int Id_Lin { get; set; }
        public string Desc_Linea { get; set; }
        public int Id_Cod { get; set; }
        public string Desc_Codigo { get; set; }
        public int Id_Frec { get; set; }
        public string Desc_Frec { get; set; }

        public string Fecx_IniInv2 { get; set; }
        public string Fecx_FinInv2 { get; set; }

    }

    public class EDRArchivo
    {
        public string Ruta { get; set; }
        public string Nombre { get; set; }
        public string Extension { get; set; }
    }
}

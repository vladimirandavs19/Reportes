using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoConsa.Reportes.Entidades
{
    public class REPORTE
    {

        public string NombreReporte { get; set; }

        public string Codigo { get; set; }

        public string Servidor { get; set; }

        public string BaseDatos { get; set; }

        public List<string> parametros { get; set; }

        public bool VisorCRV { get; set; }
    }
}

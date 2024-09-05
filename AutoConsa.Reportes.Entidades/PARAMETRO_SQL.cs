using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoConsa.Reportes.Entidades
{
    public class PARAMETRO_SQL
    {
        public string nombre { get; set; }
        public SqlDbType tipo { get; set; }
        public int tamaño { get; set; }
        public ParameterDirection direccion { get; set; }
        public Object valor { get; set; }
    }
}

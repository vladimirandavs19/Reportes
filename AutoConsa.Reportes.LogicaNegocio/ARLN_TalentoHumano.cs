using AD = AutoConsa.Reportes.AccesoDatos;
using DTO = AutoConsa.Reportes.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AutoConsa.Reportes.LogicaNegocio
{
    public class ARLN_TalentoHumano
    {
        private readonly DTO.REPORTE _reporte;
        public ARLN_TalentoHumano(DTO.REPORTE reporte)
        {
            // TODO: Complete member initialization
            _reporte = reporte;
        }

        public System.Data.DataSet ConsultarConoceColaborador(decimal codigo, List<string> parametros)
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_R_CONOCECOLABORADOR {0}, {1}", codigo, parametros[0]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "Empleado" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
    }
}

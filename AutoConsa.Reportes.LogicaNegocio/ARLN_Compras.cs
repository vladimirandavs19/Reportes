using AutoConsa.Reportes.Entidades;
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
    public class ARLN_Compras
    {
        private REPORTE _reporte;

        public ARLN_Compras(REPORTE _reporte)
        {
            // TODO: Complete member initialization
            this._reporte = _reporte;
        }

        public DataSet ConsultarDocumento(decimal codigoPrincipal, List<string> parametros)
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet estadoCuenta = new DataSet();
            query = String.Format("exec sp_RC_RIDE {0}, {1}", codigoPrincipal, parametros[0]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = parametros[1] });
            query = String.Format("exec sp_RC_RIDE {0}, 6", codigoPrincipal);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "dsDetalleAdicional" });
            query = String.Format("exec sp_RC_RIDE {0}, 7", codigoPrincipal);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "dsFormaPago" });
            query = String.Format("exec sp_RC_RIDE {0}, 8", codigoPrincipal);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "dsDetalleIVA" });
            estadoCuenta = consulta.Consulta(listaConsulta, ref respuesta);
            return estadoCuenta;
        }
    }
}

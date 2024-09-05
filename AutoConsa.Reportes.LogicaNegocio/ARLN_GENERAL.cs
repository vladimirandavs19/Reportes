using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO = AutoConsa.Reportes.Entidades;
using AD = AutoConsa.Reportes.AccesoDatos;
using System.Data;

namespace AutoConsa.Reportes.LogicaNegocio
{
    public class ARLN_GENERAL
    {
        private DTO.REPORTE _reporte;
        public ARLN_GENERAL(DTO.REPORTE _reporte)
        {
            this._reporte = _reporte;
        }

        public DataSet ConsultarEmpresa()
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = "SELECT * FROM SI_EMPRESA";
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SI_EMPRESA" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarReporteCorreo(List<string> parametros)
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RG_VERIFALIA '{0}' , '{1}'", parametros[0], parametros[1]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RG_VERIFALIA" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarReporteEstadoCuenta(decimal codigoCliente, List<string> parametros)
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            //Cabecera del Estado de Cuenta
            query = String.Format("exec sp_RG_EstadoCuenta {0}, {1}, '{2}'", codigoCliente, parametros[0], parametros[1]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RG_EstadoCuenta" });
            //Totales del Estado de Cuenta
            query = String.Format("exec sp_RG_TotalEstadoCuenta '{0}'", codigoCliente);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RG_TotalEstadoCuenta" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarReporteResumenPresupuesto(decimal numero, List<string> parametros)
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            //Cabecera del Estado de Cuenta
            query = String.Format("exec SP_RC_PRESUPUESTO {0}, 1, {1}, {2},{3},{4},{5}", numero, parametros[1], parametros[2] == "" ? "0" : parametros[2], parametros[0], parametros[3] == "" ? "0" : parametros[3], parametros[4] == "" ? "0" : parametros[4], parametros[5] == "" ? "0" : parametros[5]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "CABECERA_PRESUPUESTO" });
            //Totales del Estado de Cuenta
            query = String.Format("exec SP_RC_PRESUPUESTO {0}, 2, {1}, {2},{3},{4},{5}", numero, parametros[1], parametros[2] == "" ? "0" : parametros[2], parametros[0], parametros[3] == "" ? "0" : parametros[3], parametros[4] == "" ? "0" : parametros[4], parametros[5] == "" ? "0" : parametros[5]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "DETALLE_PRESUPUESTO" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
    }
}

using AutoConsa.Reportes.AccesoDatos;
using AutoConsa.Reportes.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO = AutoConsa.Reportes.Entidades;

namespace AutoConsa.Reportes.LogicaNegocio
{
    public class ARLN_Repuesto
    {
        private REPORTE _reporte;

        public ARLN_Repuesto(REPORTE _reporte)
        {
            // TODO: Complete member initialization
            this._reporte = _reporte;
        }
        public System.Data.DataSet ConsultarProformaRepuesto(decimal codigoProforma, string nombreUsuario, int tipo)
        {
            /*
            RESPUESTA objetoRespuesta = new RESPUESTA();
            DataSet retorno = new DataSet();
            ARAD_Conexion consulta = new ARAD_Conexion();
            List<SqlParameter> listaParametros = new List<SqlParameter>();
            SqlParameter objetoParametro = new SqlParameter();
            objetoParametro = consulta.param("@PR_CODIGO",SqlDbType.Decimal,50,ParameterDirection.Input,codigoProforma);
            listaParametros.Add(objetoParametro);
            DataTable tabla = new DataTable();
            tabla = consulta.EjecutarProcedimientoAlmacenadoDataTable("sp_R_ProformaRepuesto", listaParametros, ref objetoRespuesta);
            retorno.Tables.Add(tabla.Copy());
            return retorno;
            */
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec sp_RR_ProformaRepuesto {0}, '{1}', {2}", codigoProforma, nombreUsuario, tipo);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RR_ProformaRepuesto" });

            query = String.Format("exec sp_RG_Agencia");
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RG_Agencia" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);

            return retorno;
        }
        public System.Data.DataSet ConsultarVentasPerdidas(decimal codigo, DateTime fechainicio, DateTime fechafin, String agencias )
        {
           ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec sp_RR_vent_perd_most '{0}', '{1}', '{2}'", fechainicio.ToString("yyyy-MM-dd"), fechafin.ToString("yyyy-MM-dd"), agencias);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RR_vent_perd_most" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);

            return retorno;
        }
    }
}

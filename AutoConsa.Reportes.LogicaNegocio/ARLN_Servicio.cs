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
    public class ARLN_Servicio
    {
        private REPORTE _reporte;

        public ARLN_Servicio(REPORTE _reporte)
        {
            // TODO: Complete member initialization
            this._reporte = _reporte;
        }
        public System.Data.DataSet ConsultarPrefacturaServicio(decimal codigoHoja,string tipoItem,string tipoTitulo)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec sp_RS_Prefactura {0}, '{1}', '{2}'", codigoHoja,tipoItem,tipoTitulo);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RS_Prefactura" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        public System.Data.DataSet ConsultarProformaServicio(decimal codigoProforma, string opcion)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec sp_RS_Proforma {0}, '{1}'", codigoProforma, opcion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RS_PROFORMA" });
            query = String.Format("exec sp_RG_Agencia");
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RG_Agencia" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        public System.Data.DataSet ConsultarHojaTrabajoServicio(decimal codigoHoja, Int16 opcion, Int16 codigoAgencia, Int16 codigoTitulo1, string codigoTitulo2, Int16 codigoTaxi)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RS_HOJA_TRABAJO {0}, {1}, {2}, {3}, '{4}', {5}", codigoHoja, opcion, codigoAgencia, codigoTitulo1, codigoTitulo2,codigoTaxi);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RS_HOJA_TRABAJO" });
            query = String.Format("exec SP_RS_DETALLE_HOJA_TRABAJO {0}", codigoHoja);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RS_DETALLE_HOJA_TRABAJO" });
            query = String.Format("exec SP_RS_OBSE_HOJA_TRABAJO {0}", codigoHoja);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RS_OBSE_HOJA_TRABAJO" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarInformeTecnico(decimal codigoHoja, string opcion)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_REPO_INFO_TECN {0},'{1}'", codigoHoja,opcion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_REPO_INFO_TECN" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarHistorialHoja(decimal codigoSticker)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_SERV_HIST_VEHI {0}", codigoSticker);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_SERV_HIST_VEHI" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        public DataSet ConsultarEgresoBodega(decimal codigo, Int16 opcion)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RS_SALI_BODE {0},{1}",codigo,opcion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RS_SALI_BODE" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        public DataSet ConsultarCheckListColi(decimal codigoHoja)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RS_CHECK_LIST_COLI {0}", codigoHoja);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RS_CHECK_LIST_COLI" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarResumenTorreColisiones(decimal codigoAgencia)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RS_RESU_TORRE_COLI {0}", codigoAgencia);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RS_RESU_TORRE_COLI" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        public System.Data.DataSet ConsultartorreServiciosScreen(decimal codigoAgencia, string cadenaSql)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RS_TORRE_CONT_SERV_SCREEN {0}, '{1}'", codigoAgencia, cadenaSql);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RS_TORRE_CONT_SERV_SCREEN" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        public System.Data.DataSet ConsultarDatosFlota(decimal flt_codigo)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_CONS_FLOT_SERV 104, {0}", flt_codigo);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_CONS_FLOT_SERV" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO = AutoConsa.Reportes.Entidades;
using AD = AutoConsa.Reportes.AccesoDatos;

namespace AutoConsa.Reportes.LogicaNegocio
{
    public class ARLNF_Contratos
    {
        DTO.REPORTE _reporte;
        public ARLNF_Contratos(DTO.REPORTE _reporte)
        {
            this._reporte = _reporte;
        }


        public DataSet ConsultarContratoNormal(decimal codigoContrato, decimal opcion, decimal tipoImpresion)
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            //Cabecera
            query = String.Format("exec sp_RC_ContratoNormal {0}, 1, {2} ", codigoContrato, opcion, tipoImpresion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RC_ContratoNormal" });
            //Rubros de los contratos
            query = String.Format("exec sp_RC_ContratoNormal  {0},2, {1}", codigoContrato, tipoImpresion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RC_SContratoNormal" });
            //Conductores Adicionales
            query = String.Format("exec sp_RC_ContratoNormal {0}, 3, {1}", codigoContrato, tipoImpresion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RC_SContratoNormalCA" });
            //Anexo
            query = String.Format("exec sp_RC_ContratoNormal {0}, 4, {1}", codigoContrato, tipoImpresion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RC_SContratoNormalAx" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarContratoLiquidacion(decimal codigoContrato, decimal opcion, decimal tipoImpresion)
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            //Cabecera
            query = String.Format("exec sp_RC_ContratoNormal {0}, 1, {2} ", codigoContrato, opcion, tipoImpresion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RC_ContratoNormal" });
            //Rubros de los contratos
            query = String.Format("exec sp_RC_ContratoNormal  {0},2, {1}", codigoContrato, tipoImpresion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RC_SContratoNormal" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarCambioAuto(decimal codigoContrato, decimal opcion, decimal tipoImpresion)
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            //Cabecera Cambio Auto
            query = String.Format("exec sp_RC_ContratoNormal {0}, 5, 3 ", codigoContrato, opcion, tipoImpresion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RC_ContratoCambioAuto" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarProformaRenta(decimal codigoProforma, decimal opcion)
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            //Cabecera Cambio Auto
            query = String.Format("exec sp_RV_ProformaRenta {0}, {1} ", codigoProforma, opcion);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RV_ProformaRenta" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarOrdenTrabajo(decimal codigoOrdenTrabajo)
        {
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            //Cabecera Cambio Auto
            query = String.Format("exec sp_RC_ORDENTRABAJO {0}, {1} ", 1, codigoOrdenTrabajo);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RC_OrdenTrabajo" });

            query = String.Format("exec sp_RC_ORDENTRABAJO {0}, {1} ", 2, codigoOrdenTrabajo);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RC_DetaOrdenTrabajo" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
    }
}

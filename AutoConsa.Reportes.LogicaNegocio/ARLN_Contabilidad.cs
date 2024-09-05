using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO = AutoConsa.Reportes.Entidades;
using AD = AutoConsa.Reportes.AccesoDatos;
using AutoConsa.Reportes.AccesoDatos;
using System.Data;
using System.Data.SqlClient;
using iTextSharp.text.pdf;
using System.IO;

namespace AutoConsa.Reportes.LogicaNegocio
{
    public class ARLN_Contabilidad
    {
        private DTO.REPORTE _reporte;

        public ARLN_Contabilidad(DTO.REPORTE _reporte)
        {
            // TODO: Complete member initialization
            this._reporte = _reporte;
        }

        public DataSet ConsultarReciboCajaPorCodigoDiario(decimal codigoDiario, string nombreAdicional, string loginUsuario, ref DTO.RESPUESTA respuesta, string tipo = "0")
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            String query = String.Empty;
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            List<DTO.PARAMETRO_SQL> listaParametros = new List<DTO.PARAMETRO_SQL>();
            DTO.PARAMETRO_SQL parametro = new DTO.PARAMETRO_SQL();
            parametro.direccion = ParameterDirection.InputOutput;
            parametro.nombre = "@codigo";
            parametro.tamaño = 4;
            parametro.tipo = SqlDbType.Int;
            parametro.valor = 123;
            listaParametros.Add(parametro);

            var numero = consulta.EjecutarProcedimientoAlmacenadoConRetorno("sp_pedir_numero", listaParametros, new List<string>() { "@codigo" }, ref respuesta);
            listaParametros = new List<DTO.PARAMETRO_SQL>();
            parametro = new DTO.PARAMETRO_SQL();
            parametro.direccion = ParameterDirection.Input;
            parametro.nombre = "@numero";
            parametro.tamaño = 6;
            parametro.tipo = SqlDbType.Int;
            parametro.valor = numero.First().VALOR;
            listaParametros.Add(parametro);

            parametro = new DTO.PARAMETRO_SQL();
            parametro.direccion = ParameterDirection.Input;
            parametro.nombre = "@cd_codigo";
            parametro.tamaño = 8;
            parametro.tipo = SqlDbType.Decimal;
            parametro.valor = codigoDiario;
            listaParametros.Add(parametro);

            parametro = new DTO.PARAMETRO_SQL();
            parametro.direccion = ParameterDirection.Input;
            parametro.nombre = "@tipo";
            parametro.tamaño = 2;
            parametro.tipo = SqlDbType.Int;
            parametro.valor = 1;
            listaParametros.Add(parametro);
            if (tipo != "6")
                consulta.EjecutarProcedimientoAlmacenado("sp_impr_reci_caja", listaParametros, ref respuesta);
            else
                consulta.EjecutarProcedimientoAlmacenado("sp_impr_devolucion", listaParametros, ref respuesta);
            //Cabecera 
            query = String.Format("SELECT TOP 1 {0}, (SELECT TOP 1 EM_NOMBRE FROM SI_EMPRESA) EM_NOMBRE, (SELECT TOP 1 EM_RUC FROM SI_EMPRESA) EM_RUC, AG_NOMBRE, dbo.fn_Devolver_Secuencia(RECA_ID) RECA_ID, RC.CD_CODIGO, '' TIPO_TRAN, RCI.CD_FECHA CD_FECHA, RTRIM(LTRIM(CL.CL_NOMBRE + ' ' + CL.CL_APELLIDO)) CL_NOMBRE, UPPER(ISNULL(CD_DESCRIPCION,RC.RECA_OBSERVACION)) CD_DESCRIPCION, RECA_VALOR CANTIDAD, '{2}' AS US_NOMBRE_SECUNDARIO, (select us_nombre from seg_usuarios where us_codigo = rc.us_codigo) US_NOMBRE, (SELECT TOP 1 EM_IMAGEN FROM SI_EMPRESA) EM_IMAGEN, CL.CL_ID, CL.CL_DIRE_FACT_AUTO CL_DIRECCION, CL.CL_TELEFONO1+ CASE WHEN ISNULL(CL.CL_TELEFONO2,'') = '' THEN ISNULL(CL.CL_TELEFONO2,'') ELSE '/' +CL.CL_TELEFONO2 END CL_TELEFONO, (SELECT TOP 1 EM_RESOLUCION FROM SI_EMPRESA) EM_RESOLUCION FROM SI_REPO_COMP_INGR RCI LEFT JOIN SI_RECI_CAJA RC ON RC.CD_CODIGO = RCI.CD_CODIGO LEFT JOIN SI_CLIENTE CL ON RC.CL_CODIGO = CL.CL_CODIGO WHERE NUMERO = {1}", new object[] { numero.First().VALOR, numero.First().VALOR, nombreAdicional, loginUsuario });
            //query = String.Format("SELECT {0}, (SELECT TOP 1 EM_NOMBRE FROM SI_EMPRESA) EM_NOMBRE, (SELECT TOP 1 EM_RUC FROM SI_EMPRESA) EM_RUC, AG_NOMBRE, dbo.fn_Devolver_Secuencia(RECA_ID) RECA_ID, RC.CD_CODIGO, '' TIPO_TRAN, ISNULL(CD.CD_FECHA,RC.RECA_FECHA) CD_FECHA, RTRIM(LTRIM(CL.CL_NOMBRE + ' ' + CL.CL_APELLIDO)) CL_NOMBRE, UPPER(ISNULL(CD.CD_DESCRIPCION,RC.RECA_OBSERVACION)) CD_DESCRIPCION, RECA_VALOR CANTIDAD, '{2}' AS US_NOMBRE_SECUNDARIO, (select us_nombre from seg_usuarios where us_codigo = rc.us_codigo) US_NOMBRE, (SELECT TOP 1 EM_IMAGEN FROM SI_EMPRESA) EM_IMAGEN, CL.CL_ID, CL.CL_DIRE_FACT_AUTO CL_DIRECCION, CL.CL_TELEFONO1+ CASE WHEN ISNULL(CL.CL_TELEFONO2,'') = '' THEN ISNULL(CL.CL_TELEFONO2,'') ELSE '/' +CL.CL_TELEFONO2 END CL_TELEFONO, (SELECT TOP 1 EM_RESOLUCION FROM SI_EMPRESA) EM_RESOLUCION FROM SI_RECI_CAJA RC LEFT JOIN SI_CABE_DIAR CD ON RC.CD_CODIGO = CD.CD_CODIGO INNER JOIN SI_CLIENTE CL ON RC.CL_CODIGO = CL.CL_CODIGO INNER JOIN SI_AGENCIA AG ON RC.AG_CODIGO = AG.AG_CODIGO WHERE RC.CD_CODIGO = {1}", new object[] { numero.First().VALOR, codigoDiario, nombreAdicional, loginUsuario });
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "dsRepoCompIngr" });
            //Detalle
            query = String.Empty;
            query = String.Format("SELECT BANCO PD_FORM_PAGO, CHEQUE PD_NUME_CHEQ, CUENTA PD_NUME_CUEN, VALOR FROM si_repo_comp_ingr_deta WHERE NUMERO = {0} order by PD_NUME_CHEQ DESC", numero.First().VALOR);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "dsDetaCompIngr" });
            //Agencias
            query = String.Empty;
            query = "exec SP_RG_AGENCIA";
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RG_AGENCIA" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);

            listaParametros = new List<DTO.PARAMETRO_SQL>();
            parametro = new DTO.PARAMETRO_SQL();
            parametro.direccion = ParameterDirection.Input;
            parametro.nombre = "@NUMERO";
            parametro.tamaño = 7;
            parametro.tipo = SqlDbType.Int;
            parametro.valor = numero.First().VALOR;
            listaParametros.Add(parametro);

            consulta.EjecutarProcedimientoAlmacenado("sp_Delete_RepoReciCaja", listaParametros, ref respuesta);

            return retorno;
        }

        public DataSet ConsultarLiquidacionCompra(decimal codigoPrincipal)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec sp_RC_LiquidacionXml {0}, {1}", codigoPrincipal, 1);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "infoTributaria" });

            query = String.Format("exec sp_RC_LiquidacionXml {0}, {1}", codigoPrincipal, 2);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "infoLiquidacionCompra" });

            query = String.Format("exec sp_RC_LiquidacionXml {0}, {1}", codigoPrincipal, 3);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "detalles" });

            query = String.Format("exec sp_RC_LiquidacionXml {0}, {1}", codigoPrincipal, 4);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "infoAdicional" });

            query = String.Format("exec sp_RC_LiquidacionXml {0}, {1}", codigoPrincipal, 6);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "pagos" });

            query = String.Format("exec sp_RC_LiquidacionXml {0}, {1}", codigoPrincipal, 7);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "totalImpuesto" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);

            DataTable general = new DataTable("general");
            DataColumn columnaGeneral = new DataColumn("codigoPrincipalDocumento", System.Type.GetType("System.Decimal"));
            general.Columns.Add(columnaGeneral);
            columnaGeneral = new DataColumn("imagenclaveAcceso", System.Type.GetType("System.Byte[]"));
            general.Columns.Add(columnaGeneral);
            columnaGeneral = new DataColumn("correos", System.Type.GetType("System.String"));
            general.Columns.Add(columnaGeneral);
            columnaGeneral = new DataColumn("primerpie", System.Type.GetType("System.String"));
            general.Columns.Add(columnaGeneral);
            columnaGeneral = new DataColumn("segundopie", System.Type.GetType("System.String"));
            general.Columns.Add(columnaGeneral);

            DataSet tablaGeneral = new DataSet();
            listaConsulta = new List<DTO.CONSULTA_BD>();
            query = String.Format("SELECT * FROM SI_CAMP_REPO_REPU WHERE RR_TIPO = 194 ORDER BY RR_NOMB_REAL_S2");
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "generico" });
            tablaGeneral = consulta.Consulta(listaConsulta, ref respuesta);

            DataRow fila = general.NewRow();
            string numeroAutorizacion = (string)retorno.Tables["infoTributaria"].Rows[0]["numeroAutorizacion"];
            fila["codigoPrincipalDocumento"] = codigoPrincipal;
            fila["imagenclaveAcceso"] = GenerarCodigoBarras((string)retorno.Tables["infoTributaria"].Rows[0]["claveAcceso"]);
            if (numeroAutorizacion != String.Empty)
                fila["primerpie"] = (string)tablaGeneral.Tables["generico"].Rows[0][3];
            else
                fila["primerpie"] = ((string)tablaGeneral.Tables["generico"].Rows[1][3] + (string)tablaGeneral.Tables["generico"].Rows[1][4]).Trim();
            fila["segundopie"] = (string)tablaGeneral.Tables["generico"].Rows[2][3];

            var correos = (from infoAdicional in retorno.Tables["infoAdicional"].AsEnumerable() where infoAdicional.Field<string>("nombre") == "emailCliente" select infoAdicional).FirstOrDefault();
            fila["correos"] = correos["campoAdicional"];

            general.Rows.Add(fila);
            retorno.Tables.Add(general);

            return retorno;
        }

        internal byte[] GenerarCodigoBarras(string claveAcceso)
        {
            //Genero la imagen del documento
            Barcode128 codigobarra = new Barcode128();
            codigobarra.CodeType = iTextSharp.text.pdf.Barcode.CODE128;
            codigobarra.StartStopText = true;
            codigobarra.GenerateChecksum = true;
            codigobarra.ChecksumText = true;
            codigobarra.BarHeight = 50;
            codigobarra.Code = claveAcceso;
            System.Drawing.Image img = codigobarra.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imagen = ms.ToArray();
            return imagen;
        }

        public DataSet ConsultaCartaCobroxDia(decimal numero, decimal dias)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            String query = String.Empty;
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            DataSet retorno = new DataSet();
            query = String.Format("exec sp_R_CartaSeguimientoCobro {0},{1}", numero, dias);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_R_CartaSeguimientoCobro" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarCierreCaja(List<string> parametros)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            String query = String.Empty;
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            DataSet retorno = new DataSet();
            query = String.Format("exec sp_R_CierreCaja '{0}',{1},{2}", parametros[0], parametros[2], parametros[1]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_R_CierreCaja" });

            query = String.Format("exec sp_R_ResumenNotaCredito '{0}', {1}, {2}", parametros[0], parametros[2], parametros[1]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_R_ResumenNotaCredito" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta, 180);
            return retorno;
        }
        public DataSet ConsultarResumenTarjeta(String fecha, Int16 caja, Int16 agencia)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec sp_g_GenerarResumenTarjetas {0},{1},{2}", fecha, caja, agencia);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_g_GenerarResumenTarjetas" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarResumenDeposito(String fecha, Int16 caja, Int16 agencia)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec sp_g_GenerarResumenDeposito {0},{1},{2}", fecha, caja, agencia);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_g_GenerarResumenDeposito" });
            query = String.Format("exec sp_g_GenerarResumenDeposito1 {0},{1},{2}", fecha, caja, agencia);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_g_GenerarResumenDeposito1" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarResumenServipagos(String fecha, Int16 caja, Int16 agencia)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RC_GENE_RESU_TRAN_SERVI_PAGOS {0},{1},{2}", fecha, caja, agencia);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RC_GENE_RESU_TRAN_SERVI_PAGOS" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarResumenTransaccionesElectronicas(String fecha, Int16 caja, Int16 agencia)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_G_GenerarResumenTranElectronicas {0},{1},{2}", fecha, caja, agencia);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_G_GenerarResumenTranElectronicas" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarConciliacion(decimal codigoPrincipal, List<string> parametros, int tipo)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            switch (tipo)
            {
                case 1:
                    //Mayor
                    //Conciliacion
                    query = String.Format("exec SP_RC_MOVIMIENTOCONTABLE {0},'{1}'", codigoPrincipal.ToString(), parametros[0]);
                    listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RC_MOVIMIENTOCONTABLE" });
                    break;
                case 2:
                    //Conciliacion
                    query = String.Format("exec SP_RC_CONCILIACIONDIARIO {0},{1},{2},'{3}'", new object[] { codigoPrincipal.ToString(), parametros[0], parametros[1], parametros[2] });
                    listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RC_CONCILIACIONDIARIO" });
                    break;
            }


            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarPlanAcumulativo(Int16 opcion, String fecha, Int16 agencia)
        {
            ARAD_Conexion consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec P_CONS_PLAN_ACUMULATIVO {0},{1},{2}", opcion, fecha, agencia);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "P_CONS_PLAN_ACUMULATIVO" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

    }
}

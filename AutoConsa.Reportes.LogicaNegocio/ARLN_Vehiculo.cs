using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoConsa.Reportes.AccesoDatos;
using DTO = AutoConsa.Reportes.Entidades;
using System.Data;
using System.Xml;
using System.IO;

namespace AutoConsa.Reportes.LogicaNegocio
{
    public class ARLN_Vehiculo
    {
        private DTO.REPORTE _reporte;
        ARAD_Conexion consulta;
        public ARLN_Vehiculo(DTO.REPORTE _reporte)
        {
            // TODO: Complete member initialization
            this._reporte = _reporte;
        }

        public DataSet ConsultarMantenimientoPrepagado(decimal codigoProforma, List<string> parametros, string ruta)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            //Detalle Contratos Intermediacion
            query = String.Format("exec SP_RV_MANTENIMIENTOPREPAGO {0}", codigoProforma);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_MANTENIMIENTOPREPAGO" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            string filename = "";
            foreach (DataRow fila in retorno.Tables["SP_RV_MANTENIMIENTOPREPAGO"].Rows)
            {
                decimal ta_codigo = (decimal)fila["TA_CODIGO"];
                if (ta_codigo == 1)
                {
                    filename = String.Format(@"{0}livianos.jpg", ruta);
                }
                else
                {
                    filename = String.Format(@"{0}pesados.jpg", ruta);
                }
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    // Create a byte array of file stream length
                    byte[] bytes = System.IO.File.ReadAllBytes(filename);
                    //Read block of bytes from stream into the byte array
                    fs.Read(bytes, 0, System.Convert.ToInt32(fs.Length));
                    //Close the File Stream
                    fs.Close();

                    fila["IMAGEN"] = bytes;
                }
            }

            return retorno;

        }

        public DataSet ConsultarHojaTrabajoPorCodigo(decimal codigoHojaControl, string[] parametros)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            //Cabecera HC
            decimal valoresAdicionales = 0;
            query = String.Format("exec sp_R_HojaControl {0}, 2, {1}", codigoHojaControl, 6);
            foreach (string item in parametros)
            {
                if (Decimal.TryParse(item, out valoresAdicionales))
                {
                    query += String.Format(",{0}", valoresAdicionales);
                }
                else
                {
                    query += String.Format(",'{0}'", item);
                }
            }
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "HojaControl" });
            //Detalle Cliente HC
            query = String.Format("exec sp_R_HojaControl {0}, 2, {1}", codigoHojaControl, 1);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "DetalleClienteHojaControl" });
            //Detalle Origen Ingresos
            query = String.Format("exec sp_R_HojaControl {0}, 2, {1}", codigoHojaControl, 7);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "OrigenIngresosHojaControl" });
            //Detalle Hoja Control
            query = String.Format("exec sp_R_HojaControl {0}, 2, {1}", codigoHojaControl, 2);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "DetalleHojaControl" });
            //Detalle Financiera
            query = String.Format("exec sp_R_HojaControl {0}, 2, {1}", codigoHojaControl, 3);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "DetalleFinancieraHojaControl" });

            //Detalle Nota Credito
            query = String.Format("exec sp_R_HojaControl {0}, 2, {1}", codigoHojaControl, 4);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "DetalleDescuentoHojaControl" });

            //Detalle Rebates
            query = String.Format("exec sp_R_HojaControl {0}, 2, {1}", codigoHojaControl, 5);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "DetalleRebatesHojaControl" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);

            return retorno;
        }

        #region Contratos
        /// <summary>
        /// Consultar información para el reporte de dominio de juricido nombre del reporte siac --> Cont_Domi_Juridico.rpt
        /// </summary>
        /// <param name="codigoFactura">Código principal de la Factura</param>
        /// <returns>DataSet del sp-> SP_RV_CONT_DOMI_JURIDICO </returns>
        public System.Data.DataSet ConsultarDominioJuridico(decimal codigoFactura)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            query = String.Format("exec SP_RV_CONT_DOMI_JURIDICO {0}", codigoFactura);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_CONT_DOMI_JURIDICO" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        /// <summary>
        /// Consultar información para el reporte Pagare nombre del reporte siac --> PAGARE1 - copia (2).rpt
        /// </summary>
        /// <param name="codigoFactura">Código principal de la Factura</param>
        /// <returns>DataSet del sp </returns>
        public System.Data.DataSet ConsultarPagare(decimal codigoFactura)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            //Obtiene la información de la cabecera
            query = String.Format("exec SP_RV_PAGARE {0}, {1}", codigoFactura, 1);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_PAGARE_CABECERA" });


            //Detalle de la tabla de Amortización
            query = String.Format("exec SP_RV_PAGARE {0}, {1}", codigoFactura, 2);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_PAGARE_AMORTIZACION" });


            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoFactura"></param>
        /// <returns></returns>
        public System.Data.DataSet ConsultarPagareDos(decimal codigoFactura)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            //Obtiene la información de la cabecera
            query = String.Format("exec SP_RV_PAGARE_DOS {0}, {1}, {2}", codigoFactura, 1, 0);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_PAGARE_DOS_CABE" });



            ////Detalle de la tabla de Amortización
            query = String.Format("exec SP_RV_PAGARE_DOS {0}, {1}, {2}", codigoFactura, 2, 2);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_PAGARE_DOS_DETA" });


            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoFactura"></param>
        /// <param name="tipo">1=> Tabla de amortizacion 2=> total deudado</param>
        /// <returns></returns>
        public System.Data.DataSet ConsultarPagareCartaVenta(decimal codigoFactura, decimal tipo)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            //Obtiene la información de la cabecera
            query = String.Format("exec SP_RV_PAGARE_DOS {0}, {1}, {2}", codigoFactura, 1, 0);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_PAGARE_DOS_CABE" });


            ////Detalle de la tabla de Amortización
            query = String.Format("exec SP_RV_PAGARE_DOS {0}, {1}, {2}", codigoFactura, 2, tipo);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_PAGARE_DOS_DETA" });


            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }



        public System.Data.DataSet ConsultarPeps(decimal codigoHojaControl)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            query = String.Format("exec SP_RV_PEPS {0}", codigoHojaControl);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_PEPS" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        /// <summary>
        /// Consultar información para el reporte carta de cesión 
        /// </summary>
        /// <param name="codigoFactura">Código principal de la Factura</param>
        /// <returns>DataSet del sp</returns>
        public System.Data.DataSet ConsultarCartaCesion(decimal codigoFactura)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            query = String.Format("exec SP_RV_CART_CESI {0}", codigoFactura);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_CART_CESI" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        /// <summary>
        /// Consultar información para el reporte de carta de Venta siac --> Cart_vent.rpt, siac web->rptVCartaVenta.rpt
        /// </summary>
        /// <param name="codigoFactura">Código principal de la Factura</param>
        /// <returns>DataSet del sp</returns>
        public System.Data.DataSet ConsultarCartaVenta(decimal codigoFactura)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            query = String.Format("exec SP_RV_CART_VENT {0}", codigoFactura);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_CART_VENT" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        /// <summary>
        /// Consultar información para el reporte de Declaración juramentada de origen de recursos de fondos
        /// siac --> Origen_recursos.rpt, siac web->rptVOrigenRecursos.rpt
        /// </summary>
        /// <param name="codigoFactura">Código principal de la Factura</param>
        /// <returns>DataSet del sp</returns>
        public System.Data.DataSet ConsultarOrigenRecurso(decimal codigoFactura)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            query = String.Format("exec SP_RV_ORIG_RECU {0}", codigoFactura);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_ORIG_RECU" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        /// <summary>
        /// Consultar información para el reporte carta Fidelizacion
        /// </summary>
        /// <param name="codigoFactura"></param>
        /// <returns></returns>
        public System.Data.DataSet ConsultarCartaFidelizacion(decimal codigoFactura)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            //Obtiene la información de la cabecera
            query = String.Format("exec SP_RV_CART_FIDE {0}, {1}", codigoFactura, 1);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_CART_FIDE_CABECERA" });

            //Detalle de la tabla colaborador
            query = String.Format("exec SP_RV_CART_FIDE {0}, {1}", codigoFactura, 2);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_CART_FIDE_COLABORADOR" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        /// <summary>
        /// Consultar información para el reporte de Reconocimiento de firmas siac --> reco_firmas.rpt
        /// </summary>
        /// <param name="codigoFactura">Código principal de la Factura</param>
        /// <returns>DataSet del sp</returns>
        public System.Data.DataSet ConsultarReconocimientoFirma(decimal codigoFactura)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            query = String.Format("exec SP_RV_RECO_FIRM {0}", codigoFactura);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_RECO_FIRM" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        /// <summary>
        /// Consultar información para el reporte de Reconocimiento de firmas siac --> reco_firmas.rpt
        /// </summary>
        /// <param name="codigoFactura">Código principal de la Factura</param>
        /// <returns>DataSet del sp</returns>
        public System.Data.DataSet ConsultarCartaNoContratacionSeguro(decimal codigoFactura)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            query = String.Format("exec SP_RV_NO_SEGURO {0}", codigoFactura);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_NO_SEGURO" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        /// <summary>
        /// Consultar información para el reporte de solicitud de debito automatico siac --> r_soli_debi_auto.rpt -->r_soli_debi_auto_juridico.rpt
        /// Siac web ->rptVSoliDebitoAutomatico.rpt 
        /// </summary>
        /// <param name="codigoFactura">Código principal de la Factura</param>
        /// <param name="codigoCliente">Código principal del cliente</param>
        /// <returns>DataSet del sp</returns>
        public System.Data.DataSet ConsultarDebitoAutomatico(decimal codigoFactura, decimal codigoCliente)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            query = String.Format("exec SP_RV_DEBI_AUTO {0},{1}", codigoFactura, codigoCliente);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_DEBI_AUTO" });

            query = String.Format("exec sp_RG_Agencia");
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_RG_Agencia" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }


        #endregion

        public DataSet ConsultarHojaVinculacion(decimal codigoPrincipal, List<string> parametros)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("SP_RV_VINCULACION {0}, {1}, 1", codigoPrincipal, parametros[4]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "dsHojaVinculacion" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            string info = String.Format("{0}?COD={1}", retorno.Tables["dsHojaVinculacion"].Rows[0]["webReporte"], parametros[5]);
            QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData qrCodeData = qrGenerator.CreateQrCode(info, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode qrCode = new QRCoder.QRCode(qrCodeData);
            System.Drawing.Bitmap qrCodeImage = qrCode.GetGraphic(20);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            byte[] imagen = ms.ToArray();

            foreach (DataRow filaVinculacion in retorno.Tables[0].Rows)
            {
                filaVinculacion["codigoBarras"] = imagen;
            }
            return retorno;
        }

        public DataSet ConsultaContratoIntermediacionUsados(decimal CodigoAutoUsado)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            //Detalle Contratos Intermediacion
            query = String.Format("exec sp_R_ContratoIntermediacionUsados {0}", CodigoAutoUsado);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "sp_R_ContratoIntermediacionUsado" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);

            return retorno;
        }

        public DataSet ConsultarProformaVehiculo(decimal codigo, List<string> listaParametros)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();

            //Cabecera proforma
            if (listaParametros.Count > 0)
                query = String.Format("exec SP_RV_PROFORMAVEHICULO {0}, {1}", codigo, listaParametros[0]);
            else
                query = String.Format("exec SP_RV_PROFORMAVEHICULO {0}", codigo);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_PROFORMAVEHICULO" });

            //Detalle Proforma
            query = String.Format("exec SP_RV_PROFORMAVEHICULOOTROITEM {0}", codigo);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_PROFORMAVEHICULOOTROITEM" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);

            return retorno;
        }

        public DataSet ConsultaCartaConsentimiento(decimal CodigoFactura)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RV_CART_CONS_EXPR {0}", CodigoFactura);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_CART_CONS_EXPR" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarTestDriveVehiculo(decimal codigoProforma, List<string> list)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RV_TESTDRIVE {0}", codigoProforma);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_TESTDRIVE" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarAcuerdoNegociacion(decimal codigoHojaControl, List<string> parametros)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RV_ACUERDONEGOCIACION {0}, 1, {1}, {2}", parametros[0], codigoHojaControl, parametros[1]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "AcuerdoNegociacion" });

            query = String.Format("exec SP_RV_ACUERDONEGOCIACION {0}, 2, {1}, {2}", parametros[0], codigoHojaControl, parametros[1]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "RubrosAcuerdoNegociacion" });

            query = String.Format("exec SP_RV_ACUERDONEGOCIACION {0}, 3, {1}, {2}", parametros[0], codigoHojaControl, parametros[1]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "AccesorioAcuerdoNegociacion" });

            query = String.Format("exec SP_RV_ACUERDONEGOCIACION {0}, 4, {1}, {2}", parametros[0], codigoHojaControl, parametros[1]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "PagosAcuerdoNegociacion" });

            query = String.Format("exec SP_RV_ACUERDONEGOCIACION {0}, 6, {1}, {2}", parametros[0], codigoHojaControl, parametros[1]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "ObsequiosAcuerdoNegociacion" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
        public DataSet ConsultaCertificadoGarantia(decimal CodigoHojaControl)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RV_CERT_GARA_CHEV {0}", CodigoHojaControl);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_CERT_GARA_CHEV" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarEncuestaTestDriveVehiculo(decimal codigoAgenda, List<string> parametros)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RV_ENCUESTATESTDRIVE {0}", codigoAgenda);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "SP_RV_ENCUESTATESTDRIVE" });
            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }

        public DataSet ConsultarPreAcuerdoNegociacion(decimal codigoAcuerdoNegociacion, List<string> parametros)
        {
            consulta = new ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            DataSet retorno = new DataSet();
            query = String.Format("exec SP_RV_ACUERDO {0}, {1}, 1", codigoAcuerdoNegociacion, parametros[0]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "dsAcuerdoNegociacion" });

            query = String.Format("exec SP_RV_ACUERDO {0}, {1}, 2", codigoAcuerdoNegociacion, parametros[0]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "dsANDatosAccesorio" });

            query = String.Format("exec SP_RV_ACUERDO {0}, {1}, 3", codigoAcuerdoNegociacion, parametros[0]);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "dsANDescuentoObsequio" });

            retorno = consulta.Consulta(listaConsulta, ref respuesta);
            return retorno;
        }
    }

}

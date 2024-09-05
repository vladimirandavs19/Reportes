using DTO = AutoConsa.Reportes.Entidades;
using AutoConsa.Reportes.LogicaNegocio;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web.HtmlReportRender;
using EncriptarDatos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.Shared;

namespace AutoConsa.Reportes.Presentacion
{
    public partial class rptReportesSIAC : System.Web.UI.Page
    {
        ARLN_Vehiculo vehiculo;
        private readonly string rutaReportes = ConfigurationManager.AppSettings["RepositorioGenerico"];
        private readonly string rutaFinanzas = "Finanzas/";
        private readonly string rutaContabilidad = "Contabilidad/";
        private readonly string rutaVehiculos = "Vehiculo/";
        private readonly string rutaRepuestos = "Repuestos/";
        private readonly string rutaServicios = "Servicios/";
        private readonly string rutaCompras = "Compras/";
        private readonly string rutaContratos = "Finamerica/Contratos/";
        private readonly string rutaGeneral = "General/";
        private readonly string rutaVentas = "Ventas/";
        private readonly string rutaSITH = "TTHH/";
        ReportDocument documento;
        ReportDocument subinforme;
        ReportDocument documentoIndividual;
        String captionDocumento;
        DTO.RESPUESTA respuesta;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.mensajedealerta.Visible = false;
            this.mensajedeerror.Visible = false;
            DTO.REPORTE _reporte = new DTO.REPORTE();
            try
            {
                //Verifico el primer parametro
                string[] parametros;
                int contador = 0;
                string temp = String.Empty;

                CodificarUrl procesar = new CodificarUrl();
                string informacionAdicional = procesar.Desencriptar(Request.QueryString["COD"]);

                temp = informacionAdicional;
                while (temp.IndexOf("|") >= 0)
                {
                    contador++;
                    temp = temp.Substring(temp.IndexOf("|") + 1).Trim();
                }
                parametros = new string[contador];
                temp = informacionAdicional;
                contador = 0;
                while (temp.IndexOf("|") >= 0)
                {
                    parametros[contador] = temp.Substring(0, temp.IndexOf("|")).Trim();
                    temp = temp.Substring(temp.IndexOf("|") + 1).Trim();
                    contador++;
                }
                _reporte.NombreReporte = parametros[0];
                _reporte.Codigo = parametros[1];
                _reporte.Servidor = parametros[2];
                _reporte.BaseDatos = parametros[3];
                _reporte.VisorCRV = Convert.ToBoolean(parametros[4]);
                _reporte.parametros = new List<string>();
                if (parametros.Count() > 5)
                {
                    for (int i = 5; i < parametros.Count(); i++)
                    {
                        _reporte.parametros.Add(parametros[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblMensajeError.Text = String.Format("Existe un error al momento de generar el documento. Estado: {0}", ex.Message);
                this.mensajedeerror.Visible = true;
            }
            finally
            {
                if (!mensajedeerror.Visible && !mensajedealerta.Visible)
                    GenerarReporte(_reporte);
            }
        }

        protected void Page_UnLoad(object sender, EventArgs e)
        {
            try
            {
                if (documento != null)
                {
                    if (documento.IsLoaded)
                    {
                        documento.Close();
                        documento.Dispose();
                    }

                    crvGeneral.Dispose();
                }
            }
            catch (Exception)
            {

            }

        }

        private void GenerarReporte(DTO.REPORTE _reporte)
        {
            try
            {
                if (_reporte.NombreReporte == "")
                {
                    throw new Exception("No se encontro el nombre del reporte para continuar con el proceso");
                }
                ARLN_GENERAL general = new ARLN_GENERAL(_reporte);
                string fileName = String.Empty;
                documento = new ReportDocument();
                subinforme = new ReportDocument();
                captionDocumento = String.Empty;
                respuesta = new DTO.RESPUESTA();
                System.Data.DataSet dsGenerico = new System.Data.DataSet();
                switch (_reporte.NombreReporte)
                {
                    #region Contabilidad
                    case "rptCReciboCaja.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_Contabilidad contabilidad = new ARLN_Contabilidad(_reporte);
                        string tipoImpresionRecibo = (_reporte.parametros.Count > 4 && _reporte.parametros.Count > 8) ? _reporte.parametros[8] : "0";
                        dsGenerico = contabilidad.ConsultarReciboCajaPorCodigoDiario(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros[3], _reporte.parametros[3], ref respuesta, tipoImpresionRecibo);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.SetParameterValue("tipoTransaccion", _reporte.parametros[2]);
                        documento.SetParameterValue("cantidadEnLetras", NumLetra.Convertir(dsGenerico.Tables["dsRepoCompIngr"].Rows[0]["CANTIDAD"].ToString(), true));
                        documento.SetParameterValue("TEXTO_1", _reporte.parametros[1]);
                        documento.SetParameterValue("TEXTO_2", _reporte.parametros[0]);
                        captionDocumento = String.Format("recibo_caja_{0}", dsGenerico.Tables["dsRepoCompIngr"].Rows[0]["RECA_ID"].ToString());
                        break;

                    case "rptCCartaCobro.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_Contabilidad cartaCobro = new ARLN_Contabilidad(_reporte);
                        dsGenerico = cartaCobro.ConsultaCartaCobroxDia(Convert.ToDecimal(_reporte.Codigo), Convert.ToDecimal(_reporte.parametros[0]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Impresion de Cartas";
                        break;

                    case "rptCConciliacion.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_Contabilidad conciliacion = new ARLN_Contabilidad(_reporte);
                        dsGenerico = conciliacion.ConsultarConciliacion(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros, 2);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = _reporte.Codigo;
                        break;

                    case "rptCMovimientoContable.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_Contabilidad movimientoContable = new ARLN_Contabilidad(_reporte);
                        dsGenerico = movimientoContable.ConsultarConciliacion(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros, 1);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = _reporte.Codigo;
                        break;

                    case "rptCCierreCaja.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_Contabilidad cierre = new ARLN_Contabilidad(_reporte);
                        dsGenerico = cierre.ConsultarCierreCaja(_reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = _reporte.Codigo;
                        break;

                    case "rptCResumenDepositos.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_Contabilidad resumendepositos = new ARLN_Contabilidad(_reporte);
                        dsGenerico = resumendepositos.ConsultarResumenDeposito(Convert.ToString(_reporte.parametros[0]), Convert.ToInt16(_reporte.parametros[1]), Convert.ToInt16(_reporte.parametros[2]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Impresion Resumen Depositos";
                        break;

                    case "rptCResumenTarjetas.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_Contabilidad resumentarjetas = new ARLN_Contabilidad(_reporte);
                        dsGenerico = resumentarjetas.ConsultarResumenTarjeta(Convert.ToString(_reporte.parametros[0]), Convert.ToInt16(_reporte.parametros[1]), Convert.ToInt16(_reporte.parametros[2]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Impresion Resumen Tarjetas";
                        break;

                    case "rptCResumenServipagos.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_Contabilidad resumenservipagos = new ARLN_Contabilidad(_reporte);
                        dsGenerico = resumenservipagos.ConsultarResumenServipagos(Convert.ToString(_reporte.parametros[0]), Convert.ToInt16(_reporte.parametros[1]), Convert.ToInt16(_reporte.parametros[2]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Impresion Resumen Servipagos";
                        break;


                    case "rptCResumenTransaccionesElectronicas.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_Contabilidad resumenTransaccionesElectronicas = new ARLN_Contabilidad(_reporte);
                        dsGenerico = resumenTransaccionesElectronicas.ConsultarResumenTransaccionesElectronicas(Convert.ToString(_reporte.parametros[0]), Convert.ToInt16(_reporte.parametros[1]), Convert.ToInt16(_reporte.parametros[2]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Impresion Resumen Transacciones Electrónicas";
                        break;

                    case "rptRLiquidacion.rpt":
                        ARLN_Contabilidad liquidacion = new ARLN_Contabilidad(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        dsGenerico = liquidacion.ConsultarLiquidacionCompra(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        break;

                    case "rptCPlanAcumulativoContable.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_Contabilidad planacumulativo = new ARLN_Contabilidad(_reporte);
                        dsGenerico = planacumulativo.ConsultarPlanAcumulativo(Convert.ToInt16(_reporte.parametros[0]), Convert.ToString(_reporte.parametros[1]), Convert.ToInt16(_reporte.parametros[2]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Reporte Contable Plan Acumulativo";
                        break;
                    #endregion Contabilidad

                    #region Repuestos
                    case "rptRProforma.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaRepuestos, _reporte.NombreReporte);
                        ARLN_Repuesto repuesto = new ARLN_Repuesto(_reporte);
                        dsGenerico = repuesto.ConsultarProformaRepuesto(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros[0], Convert.ToInt16(_reporte.parametros[1]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "proforma_repuesto";
                        break;
                    case "rptRVentPerdMost.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaRepuestos, _reporte.NombreReporte);
                        ARLN_Repuesto VentasPerdidas = new ARLN_Repuesto(_reporte);
                        dsGenerico = VentasPerdidas.ConsultarVentasPerdidas(Convert.ToDecimal(_reporte.Codigo), Convert.ToDateTime(_reporte.parametros[0]), Convert.ToDateTime(_reporte.parametros[1]), Convert.ToString(_reporte.parametros[2]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Ventas Perdidas Mostrador";
                        break;
                    #endregion Repuestos

                    #region Vehiculos
                    case "rptPVMantenimientoPrepago.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        vehiculo = new ARLN_Vehiculo(_reporte);
                        dsGenerico = vehiculo.ConsultarMantenimientoPrepagado(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros, String.Format("{0}{1}", rutaReportes, rutaVehiculos));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = String.Format("Mantenimiento_Prepagado_PR{0}", _reporte.Codigo);
                        break;
                    case "rptVAcuerdoNegociacion.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        vehiculo = new ARLN_Vehiculo(_reporte);
                        dsGenerico = vehiculo.ConsultarAcuerdoNegociacion(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = String.Format("Acuerdo_Negociacion_HC{0}", _reporte.Codigo);
                        break;
                    case "rptVPreAcuerdoNegociacion.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        vehiculo = new ARLN_Vehiculo(_reporte);
                        dsGenerico = vehiculo.ConsultarPreAcuerdoNegociacion(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = String.Format("PreAcuerdoNegociacion_{0}", _reporte.Codigo);
                        break;
                    case "rptVHojaControl.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        List<string> parametrosAdicionales = new List<string>();
                        for (int i = 0; i < _reporte.parametros.Count(); i++)
                        {
                            if (i > 2)
                            {
                                parametrosAdicionales.Add(_reporte.parametros[i]);
                            }
                        }
                        vehiculo = new ARLN_Vehiculo(_reporte);
                        dsGenerico = vehiculo.ConsultarHojaTrabajoPorCodigo(Convert.ToDecimal(_reporte.Codigo), parametrosAdicionales.ToArray());
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        captionDocumento = _reporte.parametros[1];
                        break;

                    case "rptVTestDrive.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        vehiculo = new ARLN_Vehiculo(_reporte);
                        dsGenerico = vehiculo.ConsultarTestDriveVehiculo(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = String.Format("Proforma_Vehiculo_{0}", _reporte.Codigo);
                        break;
                    case "rptVEncuestaTestDrive.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        vehiculo = new ARLN_Vehiculo(_reporte);
                        dsGenerico = vehiculo.ConsultarEncuestaTestDriveVehiculo(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = String.Format("Encuesta_TestDrive_{0}", _reporte.Codigo);
                        break;
                    case "rptVProforma.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        vehiculo = new ARLN_Vehiculo(_reporte);
                        dsGenerico = vehiculo.ConsultarProformaVehiculo(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = String.Format("Proforma_Vehiculo_{0}", _reporte.Codigo);
                        break;
                    case "rptVCartaConsentimientoExpreso.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        vehiculo = new ARLN_Vehiculo(_reporte);
                        dsGenerico = vehiculo.ConsultaCartaConsentimiento(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Impresion de Carta Consentimiento Expreso";
                        break;
                    case "rptPCHojaVinculacionCliente.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        vehiculo = new ARLN_Vehiculo(_reporte);
                        _reporte.parametros.Add(System.Net.WebUtility.UrlEncode(Request.QueryString["COD"]));
                        dsGenerico = vehiculo.ConsultarHojaVinculacion(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = _reporte.Codigo;
                        break;
                    case "rptVCertificadoGarantiaChevrolet.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        vehiculo = new ARLN_Vehiculo(_reporte);
                        dsGenerico = vehiculo.ConsultaCertificadoGarantia(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Impresión de Certificado Garantía CHEVROLET";
                        break;
                    case "rptVTestDrive":

                        break;

                    #endregion Vehiculos

                    #region Vehiculos usados
                    case "rptVUContratoIntermediacion.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo vehiusado = new ARLN_Vehiculo(_reporte);
                        dsGenerico = vehiusado.ConsultaContratoIntermediacionUsados(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Impresion de Contratos Auto Usado";
                        break;

                    #endregion Vehiculos usados

                    #region Servicios

                    case "rptSPrefactura.rpt":
                        ARLN_Servicio prefactura = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = prefactura.ConsultarPrefacturaServicio(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros[0], _reporte.parametros[1]);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Prefactura de Servicios";
                        break;
                    //case "rptCFlota.rpt":
                    //    fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContratos, _reporte.NombreReporte);
                    //    ARLNF_Contratos contrato = new ARLNF_Contratos(_reporte);
                    //    dsGenerico = contrato.ConsultarContratoFlota(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros[0]);
                    //    break;
                    case "rptSProforma.rpt":
                        ARLN_Servicio proforma = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = proforma.ConsultarProformaServicio(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros[0]);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        documento.SetParameterValue("letras", NumLetra.Convertir(_reporte.parametros[1], true));
                        documento.SetParameterValue("ciudad", _reporte.parametros[2]);
                        documento.SetParameterValue("mes", _reporte.parametros[3]);
                        documento.SetParameterValue("usuario", _reporte.parametros[4]);
                        documento.SetParameterValue("programa", _reporte.parametros[5]);
                        captionDocumento = "Proforma de Servicios";
                        break;

                    case "rptSHojaTrabajo.rpt":
                        ARLN_Servicio hojaTrabajo = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = hojaTrabajo.ConsultarHojaTrabajoServicio(Convert.ToDecimal(_reporte.Codigo), Convert.ToInt16(_reporte.parametros[0]), Convert.ToInt16(_reporte.parametros[1]), Convert.ToInt16(_reporte.parametros[2]), _reporte.parametros[3], Convert.ToInt16(_reporte.parametros[4]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        //documento.SetParameterValue("letras", NumLetra.Convertir(_reporte.parametros[1], true));
                        //documento.SetParameterValue("ciudad", _reporte.parametros[2]);
                        //documento.SetParameterValue("mes", _reporte.parametros[3]);
                        //documento.SetParameterValue("usuario", _reporte.parametros[4]);
                        //documento.SetParameterValue("programa", _reporte.parametros[5]);
                        String numeroHoja = String.Empty;
                        try
                        {

                            var tabla = dsGenerico.Tables["SP_RS_HOJA_TRABAJO"];
                            numeroHoja = tabla.Rows[0]["HT_HOJA_NUME"].ToString();

                        }
                        catch (Exception) { }

                        captionDocumento = "Hoja de Trabajo Servicios " + numeroHoja;
                        break;

                    case "rptSActaEntregaRecepcion.rpt":
                        ARLN_Servicio acta = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = acta.ConsultarPrefacturaServicio(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros[0], _reporte.parametros[1]);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        if (_reporte.parametros[1] == "AR")
                        {
                            captionDocumento = "Acta Entrega Recepción Repuestos";
                        }
                        if (_reporte.parametros[1] == "AS")
                        {
                            captionDocumento = "Acta Entrega Recepción Servicios";
                        }
                        if (_reporte.parametros[1] == "GA")
                        {
                            captionDocumento = "Garantía Técnica";
                        }

                        break;
                    case "rptSInformeTecnico.rpt":
                        ARLN_Servicio informeTecnico = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = informeTecnico.ConsultarInformeTecnico(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros[0]);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Informe Técnico";
                        break;
                    case "rptSHistorialHoja.rpt":
                        ARLN_Servicio historial = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = historial.ConsultarHistorialHoja(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Historial Hoja Trabajo";
                        break;
                    case "rptSRegistroSalidaBodega.rpt":
                        ARLN_Servicio egreso = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = egreso.ConsultarEgresoBodega(Convert.ToDecimal(_reporte.Codigo), Convert.ToInt16(_reporte.parametros[0]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Egreso de Bodega";
                        break;
                    case "rptRegistroSalidaGarantia.rpt":
                        ARLN_Servicio garantia = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = garantia.ConsultarEgresoBodega(Convert.ToDecimal(_reporte.Codigo), Convert.ToInt16(_reporte.parametros[0]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Etiqueta Identeficación GM";
                        break;
                    case "rptSCheckListColi.rpt":
                        ARLN_Servicio checklistcoli = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = checklistcoli.ConsultarCheckListColi(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Check List de Entrega";
                        break;
                    case "rptSResumenTorreColisiones.rpt":
                        ARLN_Servicio resumenTorreColisiones = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = resumenTorreColisiones.ConsultarResumenTorreColisiones(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Resumen Torre Control Colisiones";
                        break;
                    case "rptSTorreControlScreen.rpt":
                        ARLN_Servicio torreServiciosScreen = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = torreServiciosScreen.ConsultartorreServiciosScreen(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros[0]);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Detalle Torre de Control Servicios";
                        break;
                    case "rptPConvenioFlotaServ.rpt":
                        ARLN_Servicio flotasConvenio = new ARLN_Servicio(_reporte);
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaServicios, _reporte.NombreReporte);
                        dsGenerico = flotasConvenio.ConsultarDatosFlota(Convert.ToDecimal(_reporte.Codigo));
                        DataTable tablaFlot = dsGenerico.Tables[0];
                        tablaFlot.Rows[0]["FLT_CRIT_COM1"] = NumLetra.Convertir(tablaFlot.Rows[0]["FLT_MONTO"].ToString(), true);
                        tablaFlot.Rows[0]["FLT_CHEV_STAR_FLOT"] = String.Format("{0:dd}", tablaFlot.Rows[0]["FLT_DESDE_SERV"]) + " de " + String.Format("{0:MMMM}", tablaFlot.Rows[0]["FLT_DESDE_SERV"]) + " de " + String.Format("{0:yyyy}", tablaFlot.Rows[0]["FLT_DESDE_SERV"]);

                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "ConvenioFlota" + _reporte.Codigo;
                        break;
                    #endregion // fin servicios

                    #region Contratos

                    case "rptVContDomiJuridico.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventContratoVenta = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventContratoVenta.ConsultarDominioJuridico(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();

                        //documento.SetParameterValue("ciudad", _reporte.parametros[0]);
                        //documento.SetParameterValue("dias", _reporte.parametros[1]);
                        documento.SetParameterValue("num_uno", _reporte.parametros[2]);
                        documento.SetParameterValue("num_dos", _reporte.parametros[3]);
                        //documento.SetParameterValue("operacion", _reporte.parametros[4]);

                        captionDocumento = "Dominio Juridico";
                        break;
                    case "rptVContDomiJuridicoSinLey.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventContratoVentaSinLey = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventContratoVentaSinLey.ConsultarDominioJuridico(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();

                        //documento.SetParameterValue("ciudad", _reporte.parametros[0]);
                        //documento.SetParameterValue("dias", _reporte.parametros[1]);
                        documento.SetParameterValue("num_uno", _reporte.parametros[2]);
                        documento.SetParameterValue("num_dos", _reporte.parametros[3]);
                        //documento.SetParameterValue("operacion", _reporte.parametros[4]);

                        captionDocumento = "Dominio Juridico sin ley";
                        break;
                    case "rptVPagare.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventPagare = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventPagare.ConsultarPagare(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();

                        documento.SetParameterValue("total_credi", _reporte.parametros[0]);

                        captionDocumento = "Reporte Pagare";
                        break;
                    case "rptVCartaCesion.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventCartaCesion = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventCartaCesion.ConsultarCartaCesion(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();

                        //documento.SetParameterValue("total_credi", _reporte.parametros[0]);

                        captionDocumento = "Reporte Carta Cesión";
                        break;
                    case "rptVCartaVenta.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventCartaVenta = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventCartaVenta.ConsultarCartaVenta(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();

                        documento.SetParameterValue("serie", _reporte.parametros[0]);
                        documento.SetParameterValue("dominio", _reporte.parametros[1]);
                        documento.SetParameterValue("operacion", _reporte.parametros[2]);

                        captionDocumento = "Reporte Carta Venta";
                        break;
                    case "rptVOrigenRecursos.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventOrigenRecurso = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventOrigenRecurso.ConsultarOrigenRecurso(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Reporte Carta Fidelización";
                        break;
                    case "rptVCartaFidelizacion.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventCartaFidelizacion = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventCartaFidelizacion.ConsultarCartaFidelizacion(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Reporte Origen Recursos";
                        break;
                    case "rptVReconocimientoFirma.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventReconoFirma = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventReconoFirma.ConsultarReconocimientoFirma(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Reporte Reconocimiento Firma";
                        break;

                    case "rptVCartaNoSeguro.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventCartaNoSeguro = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventCartaNoSeguro.ConsultarCartaNoContratacionSeguro(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Reporte No Contratacion de seguro";
                        break;
                    case "rptVPagareDos.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventPagareDos = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventPagareDos.ConsultarPagareDos(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();

                        documento.SetParameterValue("encabezado", _reporte.parametros[0]);
                        documento.SetParameterValue("monto", _reporte.parametros[1]);
                        documento.SetParameterValue("tasan", _reporte.parametros[2]);
                        documento.SetParameterValue("tasane", _reporte.parametros[3]);

                        documento.SetParameterValue("LUGAR", _reporte.parametros[4]);
                        documento.SetParameterValue("NOMB_CONY", _reporte.parametros[5]);
                        documento.SetParameterValue("CED_CONY", _reporte.parametros[6]);
                        documento.SetParameterValue("DIR_CONY", _reporte.parametros[7]);
                        documento.SetParameterValue("TEL_CONY", _reporte.parametros[8]);
                        documento.SetParameterValue("operacion", _reporte.parametros[9]);
                        documento.SetParameterValue("TEMP", _reporte.parametros[10]);

                        captionDocumento = "Reporte Pagare Dos";
                        break;
                    case "rptVSoliDebitoAutomatico.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventDebitoAutomatico = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventDebitoAutomatico.ConsultarDebitoAutomatico(Convert.ToDecimal(_reporte.Codigo), Convert.ToDecimal(_reporte.parametros[1]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();

                        documento.SetParameterValue("ciudad", _reporte.parametros[0].ToString());

                        captionDocumento = "Reporte Debito Automatico";
                        break;

                    case "rptRVPeps.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventDPeps = new ARLN_Vehiculo(_reporte);

                        dsGenerico = ventDPeps.ConsultarPeps(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();

                        documento.SetParameterValue("Ciudad", _reporte.parametros[0].ToString());
                        documento.SetParameterValue("Operacion", _reporte.parametros[1].ToString());

                        captionDocumento = "Reporte PEPS" + _reporte.Codigo.ToString();
                        break;
                    case "rptVPagareVentaCartera.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVehiculos, _reporte.NombreReporte);
                        ARLN_Vehiculo ventPagareVentaCartera = new ARLN_Vehiculo(_reporte);

                        //Identificador impresion tabla de amortizacion=> 1 o total deudado=> 2
                        decimal tipo = Convert.ToDecimal(_reporte.parametros[19]);

                        dsGenerico = ventPagareVentaCartera.ConsultarPagareCartaVenta(Convert.ToDecimal(_reporte.Codigo), tipo);


                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();

                        documento.SetParameterValue("encabezado", _reporte.parametros[0]);
                        documento.SetParameterValue("monto", _reporte.parametros[1]);
                        documento.SetParameterValue("tasan", _reporte.parametros[2]);
                        documento.SetParameterValue("tasane", _reporte.parametros[3]);

                        documento.SetParameterValue("LUGAR", _reporte.parametros[4]);
                        documento.SetParameterValue("NOMB_CONY", _reporte.parametros[5]);
                        documento.SetParameterValue("CED_CONY", _reporte.parametros[6]);
                        documento.SetParameterValue("DIR_CONY", _reporte.parametros[7]);
                        documento.SetParameterValue("TEL_CONY", _reporte.parametros[8]);
                        documento.SetParameterValue("operacion", _reporte.parametros[9]);
                        documento.SetParameterValue("TEMP", _reporte.parametros[10]);

                        documento.SetParameterValue("nombre", _reporte.parametros[11]);
                        documento.SetParameterValue("telefono", _reporte.parametros[12]);
                        documento.SetParameterValue("direccion", _reporte.parametros[13]);
                        documento.SetParameterValue("id", _reporte.parametros[14]);
                        documento.SetParameterValue("nombre2", _reporte.parametros[15]);
                        documento.SetParameterValue("direccion2", _reporte.parametros[16]);
                        documento.SetParameterValue("telefono2", _reporte.parametros[17]);
                        documento.SetParameterValue("id2", _reporte.parametros[18]);

                        captionDocumento = "Reporte Pagare Venta Cartera";
                        break;
                    #endregion // fin contratos

                    #region Finamerica.Contratos
                    case "rptCNormal.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContratos, _reporte.NombreReporte);
                        ARLNF_Contratos contrato = new ARLNF_Contratos(_reporte);
                        dsGenerico = contrato.ConsultarContratoNormal(Convert.ToDecimal(_reporte.Codigo), Convert.ToDecimal(_reporte.parametros[0]), Convert.ToDecimal(_reporte.parametros[1]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Contratos Normales";
                        break;
                    case "rptCLiquidacion.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContratos, _reporte.NombreReporte);
                        ARLNF_Contratos liquidacionC = new ARLNF_Contratos(_reporte);
                        dsGenerico = liquidacionC.ConsultarContratoLiquidacion(Convert.ToDecimal(_reporte.Codigo), Convert.ToDecimal(_reporte.parametros[0]), Convert.ToDecimal(_reporte.parametros[1]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Liquidación de Contratos";
                        break;
                    case "rptCCambioAuto.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContratos, _reporte.NombreReporte);
                        ARLNF_Contratos cambioAuto = new ARLNF_Contratos(_reporte);
                        dsGenerico = cambioAuto.ConsultarCambioAuto(Convert.ToDecimal(_reporte.Codigo), Convert.ToDecimal(_reporte.parametros[0]), Convert.ToDecimal(_reporte.parametros[1]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Cambio de Auto";
                        break;
                    case "rptCProforma.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContratos, _reporte.NombreReporte);
                        ARLNF_Contratos ProformaRenta = new ARLNF_Contratos(_reporte);
                        dsGenerico = ProformaRenta.ConsultarProformaRenta(Convert.ToDecimal(_reporte.Codigo), Convert.ToDecimal(_reporte.parametros[0]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Impresion de Proformas";
                        break;
                    case "rptCOrdenTrabajo.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContratos, _reporte.NombreReporte);
                        ARLNF_Contratos ordenTrabajo = new ARLNF_Contratos(_reporte);
                        dsGenerico = ordenTrabajo.ConsultarOrdenTrabajo(Convert.ToDecimal(_reporte.Codigo));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Impresion de Proformas";
                        break;
                    case "rptCNormal_blanco.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContratos, _reporte.NombreReporte);
                        ARLNF_Contratos contratoB = new ARLNF_Contratos(_reporte);
                        dsGenerico = contratoB.ConsultarContratoNormal(Convert.ToDecimal(_reporte.Codigo), Convert.ToDecimal(_reporte.parametros[0]), Convert.ToDecimal(_reporte.parametros[1]));
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = "Contratos Normales";
                        break;
                    #endregion // fin Finamerica.Contratos

                    #region Compras
                    case "rptCFactura.rpt":
                    case "rptcRetencion.rpt":
                    case "rptCNotaCredito.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaCompras, _reporte.NombreReporte);
                        ARLN_Compras compras = new ARLN_Compras(_reporte);
                        dsGenerico = compras.ConsultarDocumento(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = _reporte.Codigo;
                        break;
                    #endregion Compras

                    #region Ventas
                    case "rptVRetencion.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaVentas, _reporte.NombreReporte);
                        ARLN_Ventas ventaRetencion = new ARLN_Ventas(_reporte);
                        dsGenerico = ventaRetencion.ConsultarDocumento(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = _reporte.Codigo;
                        break;
                    #endregion Ventas

                    #region General
                    case "rptGVerifalia.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaGeneral, _reporte.NombreReporte);
                        ARLN_GENERAL verifalia = new ARLN_GENERAL(_reporte);
                        dsGenerico = verifalia.ConsultarReporteCorreo(_reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = String.Format("{0}_{1}", System.IO.Path.GetFileNameWithoutExtension(_reporte.NombreReporte), DateTime.Now.ToString("yyyyMMdd")); //_reporte.Codigo;
                        break;
                    case "rptCEstadoCuenta.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaGeneral, _reporte.NombreReporte);
                        ARLN_GENERAL estadoCuenta = new ARLN_GENERAL(_reporte);
                        dsGenerico = estadoCuenta.ConsultarReporteEstadoCuenta(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = String.Format("{0}_{1}", System.IO.Path.GetFileNameWithoutExtension(_reporte.NombreReporte), DateTime.Now.ToString("yyyyMMdd")); //_reporte.Codigo;
                        break;
                    #endregion General

                    #region Presupuesto
                    case "rptPResumenPresupuesto.rpt":
                        fileName = String.Format("{0}{1}{2}", rutaReportes, rutaContabilidad, _reporte.NombreReporte);
                        ARLN_GENERAL resumenPresupuesto = new ARLN_GENERAL(_reporte);
                        dsGenerico = resumenPresupuesto.ConsultarReporteResumenPresupuesto(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = String.Format("{0}_{1}", System.IO.Path.GetFileNameWithoutExtension(_reporte.NombreReporte), DateTime.Now.ToString("yyyyMMdd")); //_reporte.Codigo;
                        break;
                    #endregion Presupuesto

                    #region SITH
                    case "rptConoceColaborador.rpt":
                        fileName = string.Format("{0}{1}{2}", rutaReportes, rutaSITH, _reporte.NombreReporte);
                        ARLN_TalentoHumano conoceColaborador = new ARLN_TalentoHumano(_reporte);
                        dsGenerico = conoceColaborador.ConsultarConoceColaborador(Convert.ToDecimal(_reporte.Codigo), _reporte.parametros);
                        documento.FileName = fileName;
                        documento.SetDataSource(dsGenerico);
                        documento.Refresh();
                        captionDocumento = string.Format("ConoceColaborador_{0}", _reporte.Codigo);
                        break;
                    #endregion SITH
                }

                var informacionEmpresa = general.ConsultarEmpresa();
                if (informacionEmpresa.Tables.Count > 0)
                    this.Title = String.Format("Reportes {0} | {1}", general.ConsultarEmpresa().Tables[0].Rows[0][1].ToString(), captionDocumento);
                else
                    this.Title = String.Format("Reportes {0}", captionDocumento);

                if (_reporte.VisorCRV)
                {
                    this.crvGeneral.ID = captionDocumento;
                    this.crvGeneral.BestFitPage = true;
                    this.crvGeneral.DocumentView = CrystalDecisions.Shared.DocumentViewType.PrintLayout;
                    this.crvGeneral.HasSearchButton = false;
                    this.crvGeneral.HasToggleGroupTreeButton = false;
                    this.crvGeneral.HasCrystalLogo = false;
                    this.crvGeneral.HasDrilldownTabs = false;
                    this.crvGeneral.HasDrillUpButton = false;
                    this.crvGeneral.HasToggleParameterPanelButton = false;
                    this.crvGeneral.EnableDatabaseLogonPrompt = false;
                    this.crvGeneral.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                    this.crvGeneral.ReportSource = documento;
                    this.crvGeneral.DataBind();

                    //documento.Dispose();
                    //documento.Close();
                }
                else
                {
                    this.crvGeneral.ReportSource = documento;
                    this.crvGeneral.DataBind();
                    //Direct to Stream
                    System.IO.Stream s = documento.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    //Convert Stream to Byte
                    byte[] b = new byte[s.Length];
                    int numBytesToRead = (int)s.Length;
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    s.CopyTo(stream);

                    /*
                    var EA = documento.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    System.IO.MemoryStream stream = (System.IO.MemoryStream)documento.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                     * Para pasar el archivo de crystal a html
                    var ExportFormat = ExportFormatType.HTML40;
                    string filename = String.Format(@"C:\XML_AUTOCONSA\{0}.htm", captionDocumento);
                    documento.ExportToDisk(ExportFormat, filename);
                    */
                    Response.Clear();
                    Response.AppendHeader("Content-Disposition", "filename=" + captionDocumento + ".pdf");
                    Response.ContentType = "application/pdf";
                    Response.Buffer = true;
                    stream.WriteTo(Response.OutputStream);
                    crvGeneral.Dispose();
                    documento.Dispose();
                    documento.Close();
                    stream.Dispose();
                    stream.Close();
                    Response.Cache.SetNoStore();
                    Response.OutputStream.Dispose();

                }
            }
            catch (Exception err)
            {
                this.lblMensajeError.Text = String.Format("Existe un error al momento de generar el documento. Estado: {0}", err.Message);
                this.mensajedeerror.Visible = true;
            }
        }
    }
}
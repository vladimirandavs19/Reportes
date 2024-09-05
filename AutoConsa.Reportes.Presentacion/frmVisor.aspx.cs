using DTO = AutoConsa.Reportes.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EncriptarDatos;
using AutoConsa.Reportes.Proxy;
using AutoConsa.Reportes.Proxy.CatalogoSiacWeb;
using System.IO;

namespace AutoConsa.Reportes.Presentacion
{
    public partial class frmVisor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.mensajedealerta.Visible = false;
                this.mensajedeerror.Visible = false;
                DTO.REPORTE _reporte = new DTO.REPORTE();
                try
                {

                    CodificarUrl procesar = new CodificarUrl();

                    //Verifico el primer parametro
                    string[] parametros;
                    int contador = 0;
                    string temp = String.Empty;

                    string informacionAdicional = String.Empty;
                    try
                    {
                        informacionAdicional = procesar.Desencriptar(Request.QueryString["COD"]);
                    }
                    catch
                    {
                        string COD = System.Net.WebUtility.UrlEncode(Request.QueryString["COD"]);
                        informacionAdicional = procesar.Desencriptar(COD);
                    }
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
                if (!mensajedeerror.Visible && !mensajedealerta.Visible)
                    GenerarReporte(_reporte);
            }
        }

        private void GenerarReporte(DTO.REPORTE _reporte)
        {
            bool error;
            string mensaje;
            ARP_Catalogo catalogo = new ARP_Catalogo();

            List<Documento> documento = catalogo.ConsultarDocumentoSiac(_reporte.Servidor, _reporte.parametros[0], Convert.ToDecimal(_reporte.Codigo), _reporte.NombreReporte, out error, out mensaje);

            if (documento.Count > 0)
            {
                MemoryStream stream = new MemoryStream();
                stream.Write(documento.First().archivo, 0, documento.First().archivo.Length);

                Response.Clear();
                Response.AppendHeader("Content-Disposition", "filename=" + _reporte.Codigo + ".pdf");
                Response.ContentType = "application/pdf";
                Response.Buffer = true;
                stream.WriteTo(Response.OutputStream);
                stream.Dispose();
                stream.Close();
                Response.Cache.SetNoStore();
                Response.OutputStream.Dispose();
            }
            else
            {
                this.mensajedealerta.Visible = true;
                this.lblMensajeAlerta.Text = "No se encontro el documento al que hace referencia. Puede que no exista o se haya elminado.";
            }
        }
    }
}
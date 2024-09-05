using AD = AutoConsa.Reportes.AccesoDatos;
using DTO = AutoConsa.Reportes.Entidades;
using AutoConsa.Reportes.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using System.IO;

namespace AutoConsa.Reportes.LogicaNegocio
{
    public class ARLN_Ventas
    {
        private REPORTE _reporte;

        public ARLN_Ventas(REPORTE _reporte)
        {
            // TODO: Complete member initialization
            this._reporte = _reporte;
        }

        public DataSet ConsultarDocumento(decimal codigoPrincipal, List<string> parametros)
        {
            DataSet ds = new DataSet();
            AD.ARAD_Conexion consulta = new AD.ARAD_Conexion(_reporte.Servidor, _reporte.BaseDatos);
            string query = string.Empty;
            DTO.RESPUESTA respuesta = new DTO.RESPUESTA();
            List<DTO.CONSULTA_BD> listaConsulta = new List<DTO.CONSULTA_BD>();
            query = String.Format("exec SP_RV_RIDE '{0}',1, {1}", parametros[0], codigoPrincipal);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = parametros[1] });
            query = String.Format("exec SP_RV_RIDE '{0}',2, {1}", parametros[0], codigoPrincipal);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = parametros[2] });
            if (parametros[0] != "RE")
            {
                query = String.Format("exec SP_RV_RIDE '{0}',3, {1}", parametros[0], codigoPrincipal);
                listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = parametros[2] });
            }
            query = String.Format("exec SP_RV_RIDE '{0}',4, {1}", parametros[0], codigoPrincipal);
            listaConsulta.Add(new DTO.CONSULTA_BD() { CONSULTA = query, TABLA = "infoAdicional" });
            ds = consulta.Consulta(listaConsulta, ref respuesta);

            DataColumn columnaCodigoBarras = new DataColumn("imagenClaveAcceso");
            columnaCodigoBarras.DataType = System.Type.GetType("System.Byte[]");
            ds.Tables[parametros[1]].Columns.Add(columnaCodigoBarras);
            foreach (DataRow fila in ds.Tables[parametros[1]].Rows)
            {
                fila["imagenClaveAcceso"] = GenerarCodigoBarras(fila["claveAcceso"].ToString());
            }
            return ds;
        }

        internal static byte[] GenerarCodigoBarras(string claveAcceso)
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
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imagen = ms.ToArray();
            return imagen;
        }
    }
}
using CSW = AutoConsa.Reportes.Proxy.CatalogoSiacWeb;
using CSWEA = AutoConsa.Reportes.Proxy.CatalogoSiacWebEA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace AutoConsa.Reportes.Proxy
{
    public class ARP_Catalogo
    {
        public List<CSW.Documento> ConsultarDocumentoSiac(string servidor, string nombreDocumento, decimal codigoTabla, string nombreTabla, out bool error, out string mensaje)
        {

            CSW.CatalogoClient catalogo = new CSW.CatalogoClient();
            CSWEA.CatalogoClient catalogoEA = new CSWEA.CatalogoClient();

            List<CSW.Documento> listaDocumento = new List<CSW.Documento>();
            List<CSWEA.Documento> listaDocumentoEA = new List<CSWEA.Documento>();
            error = false;
            mensaje = string.Empty;
            try
            {
                if (servidor.ToUpper() != "SERVIDOR")
                {
                    var configuration = new MapperConfiguration(cgf =>
                        {
                            cgf.CreateMap<CSWEA.Documento, CSW.Documento>();
                        });
                    var mapper = configuration.CreateMapper();
                    listaDocumentoEA = catalogoEA.ConsultarDocumentoSiac(nombreDocumento, codigoTabla, nombreTabla);
                    listaDocumento = mapper.Map<List<CSW.Documento>>(listaDocumentoEA);
                }
                else
                {
                    listaDocumento = catalogo.ConsultarDocumentoSiac(nombreDocumento, codigoTabla, nombreTabla); 
                }
            }
            catch (Exception ex)
            {
                error = true;
                mensaje = ex.Message;
            }
            finally
            {
                catalogo.Close();
                catalogoEA.Close();
            }

            return listaDocumento;
        }
    }
}

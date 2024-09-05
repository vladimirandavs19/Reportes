using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO = AutoConsa.Reportes.Entidades;

namespace AutoConsa.Reportes.AccesoDatos
{
    internal enum Transaccion : int { Insertar = 1, Actualizar, Eliminar, Otro }
    internal enum MensajeSistema { error, success, warning, info }
    internal class FuncionesAdicionales
    {
        //Constructor
        public FuncionesAdicionales()
        {

        }

        #region Respuestas Genéricas
        /// <summary>
        /// Devuelve un objeto Entidad (DTO) con la respuesta de la transaccion realizada
        /// </summary>
        /// <param name="transaccion">enumerado tipo Transaccion</param>
        /// <param name="mensajeAdicional">mensaje Adicional (opcional)</param>
        /// <returns>objeto RESPUESTA (DTO)</returns>
        internal static DTO.RESPUESTA MensajeObjetoExito(Transaccion transaccion, String mensajeAdicional = "")
        {
            StringBuilder retorno = new StringBuilder();
            string titulo = String.Empty;

            switch (transaccion)
            {
                case Transaccion.Insertar:
                    retorno.Append("Se ha grabado con éxito");
                    titulo = "Registro Grabado";
                    break;
                case Transaccion.Actualizar:
                    retorno.Append("Se ha actualizado la información con éxito");
                    titulo = "Registro Actualizado";
                    break;
                case Transaccion.Eliminar:
                    retorno.Append("Se ha actualizado la información con éxito");
                    titulo = "Registro Eliminado";
                    break;
                case Transaccion.Otro:
                    retorno.Append("Proceso Realizado Exitosamente");
                    titulo = "S.I.A.C.";
                    break;
            }

            if (mensajeAdicional != String.Empty)
            {
                retorno.AppendLine(mensajeAdicional);
            }
            //Formateo la cadena 
            String mensajeFinal = retorno.ToString();
            //Elimino comilla simple
            mensajeFinal = mensajeFinal.Replace("'", "\"");
            //Elimino saltos de linea
            mensajeFinal = mensajeFinal.Replace(Environment.NewLine, @"\r\n");
            //Mensaje no debe tener mas de 200 caracteres
            mensajeFinal = (mensajeFinal.Length > 200) ? mensajeFinal.Substring(0, 200) : mensajeFinal;
            //Recorto
            mensajeFinal = mensajeFinal.Trim();
            //Devuelvo el objeto formateado
            return new DTO.RESPUESTA { mensaje = mensajeFinal, tipoMensaje = "success", tituloPantalla = titulo, error = false };
        }

        /// <summary>
        /// Devuelve un objeto Entidad (DTO) con error de acuerdo a la transaccion realizada
        /// </summary>
        /// <param name="transaccion">Enumerado del tipo Transacccion</param>
        /// <param name="detalle">Detalle del error generado</param>
        /// <returns>objeto RESPUESTA (DTO)</returns>
        internal static DTO.RESPUESTA MensajeObjetoError(Transaccion transaccion, string detalle, MensajeSistema tipoMensaje = MensajeSistema.error)
        {
            StringBuilder retorno = new StringBuilder();
            string titulo = String.Empty;
            retorno.Append("No se ha podido realizar la accion especificada.");
            retorno.AppendLine(detalle);
            switch (transaccion)
            {
                case Transaccion.Insertar:
                    titulo = "Registro NO Grabado";
                    break;
                case Transaccion.Actualizar:
                    titulo = "Registro NO Actualizado";
                    break;
                case Transaccion.Eliminar:
                    titulo = "Registro NO Eliminado";
                    break;
                case Transaccion.Otro:
                    titulo = "S.I.A.C.";
                    break;
            }
            //Formateo la cadena 
            String mensajeFinal = retorno.ToString();
            //Elimino comilla simple
            mensajeFinal = mensajeFinal.Replace("'", "\"");
            //Elimino saltos de linea
            mensajeFinal = mensajeFinal.Replace(Environment.NewLine, @"\r\n");
            //Mensaje no debe tener mas de 200 caracteres
            mensajeFinal = (mensajeFinal.Length > 200) ? mensajeFinal.Substring(0, 200) : mensajeFinal;
            //Recorto
            mensajeFinal = mensajeFinal.Trim();
            //Devuelvo el objeto formateado
            return new DTO.RESPUESTA { mensaje = mensajeFinal, tipoMensaje = tipoMensaje.ToString(), tituloPantalla = titulo, error = true };
        }
        #endregion Respuestas Genéricas
    }
}

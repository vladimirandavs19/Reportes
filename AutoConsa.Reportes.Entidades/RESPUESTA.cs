using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoConsa.Reportes.Entidades
{
    public class RESPUESTA
    {
        /// <summary>
        /// Titulo del mensaje que se va a mostrar en el alert 
        /// </summary>
        public string tituloPantalla { get; set; }

        /// <summary>
        /// Mensaje que se muestra en el alert
        /// </summary>
        public string mensaje { get; set; }

        /// <summary>
        /// Tipo de mensaje mostrado puede ser "warning", "error", "success", "info" 
        /// </summary>
        public string tipoMensaje { get; set; } //cambiarle al enumerado

        /// <summary>
        /// Identifica si lo que se devuelve es un error o no
        /// </summary>
        public bool error { get; set; }

        /// <summary>
        /// Codigo adicional para cualquier situacion
        /// </summary>
        public object codigo { get; set; }
    }
}

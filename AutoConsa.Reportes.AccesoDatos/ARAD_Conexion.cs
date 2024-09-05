using EncriptarDatos;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DTO = AutoConsa.Reportes.Entidades;

namespace AutoConsa.Reportes.AccesoDatos
{
    public class ARAD_Conexion
    {
        private string cadenaConexion;
        SqlConnection sqlConnection;
        SqlCommand sqlCommand;
        DataSet dataSet;
        SqlDataAdapter sqlDataAdapter;

        /// <summary>
        /// Metodo Constructor para la clase
        /// </summary>
        public ARAD_Conexion()
        {
            CodificarUrl codificarUrl = new CodificarUrl();
            cadenaConexion = codificarUrl.Desencriptar(ConfigurationManager.ConnectionStrings["siacEntities"].ToString());
        }

        /// <summary>
        /// Constructor que toma el nombre del servidor y el nombre de la base
        /// </summary>
        /// <param name="nombreServidor"></param>
        /// <param name="nombreBase"></param>
        public ARAD_Conexion(string nombreServidor, string nombreBase)
        {
            cadenaConexion = String.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID=us_rep;Password=HB3eC:_82", nombreServidor, nombreBase);
        }
        /// <summary>
        /// Devuelve un numero unico para la transaccion que se desea realizar
        /// </summary>
        /// <returns>numero para la transaccion (Decimal)</returns>
        public Decimal SolicitarNumero(ref DTO.RESPUESTA objetoRespuesta)
        {
            return EjecutarProcedimientoAlmacenado("sp_pedir_numero", ref objetoRespuesta);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve un decimal con el codigo de la transaccion
        /// </summary>
        /// <param name="nombreProcedimientoAlmacenado">nombre del procedimiento almacenado</param>
        /// <returns>codigo de la transaccion</returns>
        public Decimal EjecutarProcedimientoAlmacenado(String nombreProcedimientoAlmacenado, ref DTO.RESPUESTA objetoRespuesta)
        {
            Decimal codigo = 0;
            try
            {
                sqlConnection = new SqlConnection(cadenaConexion);
                sqlConnection.Open();
                sqlCommand = new SqlCommand(nombreProcedimientoAlmacenado, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@codigo", SqlDbType.Decimal).Direction = ParameterDirection.Output;
                sqlCommand.CommandTimeout = 180;
                sqlCommand.ExecuteNonQuery();
                codigo = Convert.ToDecimal(sqlCommand.Parameters["@codigo"].Value);
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoExito(Transaccion.Otro);
                return codigo;
            }
            catch (Exception ex)
            {
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoError(Transaccion.Otro, ex.Message);
                return codigo;
            }
            finally
            {
                sqlCommand.Parameters.Clear();
                sqlConnection.Close();
            }

        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve un decimal con el codigo de la transaccion
        /// </summary>
        /// <param name="sp">nombre del procedimiento almacenado</param>
        /// <param name="numero">codigo del numero entregado por el sistema para la transaccion</param>
        /// <returns>codigo de la transaccion</returns>
        public Decimal EjecutarProcedimientoAlmacenado(String nombreProcedimientoAlmacenado, Decimal numero, ref DTO.RESPUESTA objetoRespuesta)
        {
            try
            {
                sqlConnection = new SqlConnection(cadenaConexion);
                sqlConnection.Open();
                sqlCommand = new SqlCommand(nombreProcedimientoAlmacenado, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@numero", SqlDbType.Decimal);
                sqlCommand.Parameters[0].Value = numero;
                sqlCommand.Parameters.Add("@codigo", SqlDbType.Decimal).Direction = ParameterDirection.Output;
                sqlCommand.CommandTimeout = 180;
                sqlCommand.ExecuteNonQuery();
                Decimal codigo = Convert.ToDecimal(sqlCommand.Parameters["@codigo"].Value);
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoExito(Transaccion.Otro);
                return codigo;
            }
            catch (Exception ex)
            {
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoError(Transaccion.Otro, ex.Message);
                return 0;
            }
            finally
            {
                sqlCommand.Parameters.Clear();
                sqlConnection.Close();
            }

        }


        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve un decimal con el codigo de la transaccion
        /// </summary>
        /// <param name="sp">nombre del procedimiento almacenado</param>
        /// <param name="numero">codigo del numero entregado por el sistema para la transaccion</param>
        /// <returns>codigo de la transaccion</returns>
        public void EjecutarProcedimientoAlmacenado(String nombreProcedimientoAlmacenado, List<SqlParameter> parametros, ref DTO.RESPUESTA objetoRespuesta)
        {
            try
            {
                sqlConnection = new SqlConnection(cadenaConexion);
                sqlConnection.Open();
                sqlCommand = new SqlCommand(nombreProcedimientoAlmacenado, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter item in parametros)
                {
                    sqlCommand.Parameters.Add(item);
                }

                sqlCommand.ExecuteNonQuery();

                objetoRespuesta = FuncionesAdicionales.MensajeObjetoExito(Transaccion.Otro);

            }
            catch (Exception ex)
            {
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoError(Transaccion.Otro, ex.Message);
            }
            finally
            {
                sqlCommand.Parameters.Clear();
                sqlConnection.Close();
            }

        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve un DataSet
        /// </summary>
        /// <param name="nombreProcedimientoAlmacenado">nombre del procedimiento almacenado</param>
        /// <param name="parametros">lista SqlParameter con los parametros que necesita el sp</param>
        /// <returns>DataSet con la tabla consultada</returns>
        public DataSet EjecutarProcedimientoAlmacenado(String nombreProcedimientoAlmacenado, List<DTO.PARAMETRO_SQL> listaParametro, ref DTO.RESPUESTA objetoRespuesta, string nombreTabla = "tabla")
        {
            sqlConnection = new SqlConnection(cadenaConexion);
            sqlCommand = new SqlCommand(nombreProcedimientoAlmacenado, sqlConnection);
            dataSet = new DataSet();

            try
            {
                sqlConnection.Open();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                List<SqlParameter> parametros = new List<SqlParameter>();

                //Normalizo el objeto para hacerlo parametro
                foreach (DTO.PARAMETRO_SQL objetoParametro in listaParametro)
                {
                    parametros.Add(param(objetoParametro.nombre, objetoParametro.tipo, objetoParametro.tamaño, objetoParametro.direccion, objetoParametro.valor));
                }

                foreach (var parametro in parametros)
                {
                    sqlCommand.Parameters.Add(parametro);
                }

                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataSet);
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoExito(Transaccion.Otro);
                return dataSet;
            }
            catch (Exception ex)
            {
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoError(Transaccion.Otro, ex.Message);
                return dataSet;
            }
            finally
            {
                sqlConnection.Close();
                sqlCommand.Parameters.Clear();
            }
        }

        /// <summary>
        /// Ejecutar un procedimiento almacenado que devuelve un DataTable
        /// </summary>
        /// <param name="nombreProcedimientoAlmacenado"></param>
        /// <param name="parametros"></param>
        /// <param name="objetoRespuesta"></param>
        /// <param name="nombreTabla"></param>
        /// <returns></returns>
        public DataTable EjecutarProcedimientoAlmacenadoDataTable(string nombreProcedimientoAlmacenado, List<SqlParameter> parametros, ref DTO.RESPUESTA objetoRespuesta, string nombreTabla = "tabla")
        {

            //Variables de coneccion y ejecución
            sqlConnection = new SqlConnection(cadenaConexion);
            SqlDataAdapter da = new SqlDataAdapter(nombreProcedimientoAlmacenado, sqlConnection);
            try
            {
                //Abro la coneccion
                sqlConnection.Open();
                sqlCommand = new SqlCommand(nombreProcedimientoAlmacenado, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandTimeout = 420;

                foreach (var parametro in parametros)
                {
                    sqlCommand.Parameters.Add(parametro);
                }

                sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, nombreTabla);
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoExito(Transaccion.Otro);
                return dataSet.Tables[nombreTabla];
            }
            catch (Exception ex)
            {
                //Hubo un error devuelvo nulo y guardo la informacion en el ref
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoError(Transaccion.Otro, ex.Message);
                return null;
            }
            finally
            {
                //Limpio los parametros enviados
                sqlCommand.Parameters.Clear();
                //Aunque exista error en el proceso, limpio los parametros y cierro la conexion
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado y devuelve la cantidad de parametros necesaria
        /// </summary>
        /// <param name="nombreProcedimientoAlmacenado">nombre del procedimiento almacenado</param>
        /// <param name="listaParametro">lista de parametros enviada desde el cliente</param>
        /// <param name="listaRetorno">nombre de las variables de retorno</param>
        /// <param name="objetoRespuesta">respuesta de error o correcto desde el sp</param>
        /// <returns>"Entidad" (Clase) de tipo PARAMETRO_RESPUESTA</returns>
        public List<DTO.PARAMETRO_RESPUESTA> EjecutarProcedimientoAlmacenadoConRetorno(string nombreProcedimientoAlmacenado, List<DTO.PARAMETRO_SQL> listaParametro, List<string> listaRetorno, ref DTO.RESPUESTA objetoRespuesta)
        {
            Transaccion tran = Transaccion.Otro;

            //Variables de coneccion y ejecución
            sqlConnection = new SqlConnection(cadenaConexion);
            sqlCommand = new SqlCommand(nombreProcedimientoAlmacenado, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            //Variables de instancia
            List<DTO.PARAMETRO_RESPUESTA> retorno = new List<DTO.PARAMETRO_RESPUESTA>();
            List<SqlParameter> parametros = new List<SqlParameter>();

            //Normalizo el objeto para hacerlo parametro
            foreach (DTO.PARAMETRO_SQL objetoParametro in listaParametro)
            {
                parametros.Add(param(objetoParametro.nombre, objetoParametro.tipo, objetoParametro.tamaño, objetoParametro.direccion, objetoParametro.valor));
            }

            //Añado los parametros al comando SQL
            foreach (SqlParameter para in parametros)
            {
                sqlCommand.Parameters.Add(para);
            }

            try
            {
                //Abro la coneccion
                sqlConnection.Open();

                //Ejecuto el comando
                sqlCommand.ExecuteNonQuery();

                //Almaceno en las variable de instancia los retornos
                foreach (string parametroRetorno in listaRetorno)
                {
                    retorno.Add(new DTO.PARAMETRO_RESPUESTA { NOMBRE = parametroRetorno, VALOR = sqlCommand.Parameters[parametroRetorno].Value.ToString() });
                }

                //Como no existe error la respuesta es false
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoExito(tran);
                return retorno;
            }
            catch (Exception err)
            {
                //Hubo un error devuelvo nulo y guardo la informacion en el ref
                objetoRespuesta = FuncionesAdicionales.MensajeObjetoError(tran, err.Message);
                return null;
            }
            finally
            {
                //Aunque exista error en el proceso, limpio los parametros y cierro la conexion
                sqlCommand.Parameters.Clear();
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Devuelve un DataSet de aacuerdo a las consultas que se envien en el objeto DTO.CONSULTA_BD
        /// </summary>
        /// <param name="listaConsulta">lista que contiene el objeto DTO.CONSULTA_BD </param>
        /// <param name="objetoRespuesta">objeto con la respuesta de la transaccion</param>
        /// <param name="CommandTimeOut">tiempo de ejecucion de la consulta (predeterminado 30) https://docs.microsoft.com/es-es/aspnet/web-forms/overview/data-access/advanced-data-access-scenarios/configuring-the-data-access-layer-s-connection-and-command-level-settings-cs</param>
        /// <returns>DataSet con las DataTables de acuerdo a lo enviado en DTO.CONSULTA_BD</returns>
        public DataSet Consulta(List<DTO.CONSULTA_BD> listaConsulta, ref DTO.RESPUESTA objetoRespuesta, int CommandTimeOut = 30)
        {
            DataSet dsRetorno = new DataSet();

            foreach (DTO.CONSULTA_BD consulta in listaConsulta)
            {
                try
                {
                    //Variables de coneccion y ejecución
                    sqlConnection = new SqlConnection(cadenaConexion);
                    sqlCommand = new SqlCommand(consulta.CONSULTA, sqlConnection);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandTimeout = CommandTimeOut;
                    sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlConnection.Open();
                    sqlDataAdapter.Fill(dsRetorno, consulta.TABLA);
                    objetoRespuesta = FuncionesAdicionales.MensajeObjetoExito(Transaccion.Otro);
                }
                catch (Exception ex)
                {
                    objetoRespuesta = FuncionesAdicionales.MensajeObjetoError(Transaccion.Otro, ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return dsRetorno;
        }

        public DataSet ConsultaGenerica()
        {
            return new DataSet();
        }


        /// <summary>
        /// Metodo que devuelve un objeto SqlParameter 
        /// </summary>
        /// <param name="Nombre">nombre del parametro (debe preceder la @)</param>
        /// <param name="tipo">objeto SqlDbType para saber el tipo de parametro</param>
        /// <param name="size">Tamaño de la variable</param>
        /// <param name="direccion">objeto ParameterDirection que indica la dirección del parametro</param>
        /// <param name="valor">valor que tiene el parametro</param>
        /// <returns></returns>
        public SqlParameter param(string Nombre, SqlDbType tipo, int size, ParameterDirection direccion, Object valor)
        {
            SqlParameter SQLParametro = new SqlParameter(Nombre, tipo, size);
            SQLParametro.Direction = direccion;
            SQLParametro.Value = valor;

            return SQLParametro;
        }
    }
}

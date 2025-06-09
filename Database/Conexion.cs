using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Videoclub
{
    public class Conexion
    {
        public MySqlConnection ConexionBaseDeDatos()
        {            
            string strDBServidor = ConfigurationManager.AppSettings.Get("dbServer");
            string strDBUsuario = ConfigurationManager.AppSettings.Get("dbUser");
            string strDBPassword = ConfigurationManager.AppSettings.Get("dbPassword");
            string strBDNombre = ConfigurationManager.AppSettings.Get("dbName");
            string strConexion = "server=" + strDBServidor + ";user=" + strDBUsuario + ";pwd=" + strDBPassword + ";database=" + strBDNombre;
            return new MySqlConnection(strConexion);
        }

    }
}
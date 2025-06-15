using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Videoclub
{
    class BaseDeDatos
    {
        Conexion objConexion = new Conexion();

        public DataTable LeerTablaClientes()
        {
            MySqlConnection connection = null;
            DataTable dtClientes = null;

            try
            {
                dtClientes = new DataTable();
                connection = objConexion.ConexionBaseDeDatos();

                connection.Open();                

                string sql = "SELECT * FROM clientes";
                MySqlCommand cmd = new MySqlCommand(sql, connection);                    

                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dtClientes);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {                
                if (connection != null) connection.Close();
            }

            return dtClientes;
        }

        public DataTable LeerTablaEmpleados()
        {
            MySqlConnection connection = null;
            DataTable dtEmpleados = null;

            try
            {
                dtEmpleados = new DataTable();
                connection = objConexion.ConexionBaseDeDatos();
                connection.Open();

                string sql = "SELECT * FROM empleados";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dtEmpleados);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (connection != null) connection.Close();
            }

            return dtEmpleados;
        }

        public DataTable LeerTablaEmpleados(string usuario, string password)
        {
            MySqlConnection connection = null;
            DataTable dtEmpleados = null;

            try
            {
                dtEmpleados = new DataTable();
                connection = objConexion.ConexionBaseDeDatos();
                connection.Open();

                string sql = "SELECT * FROM empleados WHERE Usuario = '" + usuario + "' AND Password = '" + password + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dtEmpleados);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (connection != null) connection.Close();
            }

            return dtEmpleados;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//INCLUIR LIBRERIAS DE SQL
using System.Data.SqlClient;
using System.Data;
using Usuario.Models;
using System.Data.Common;
using System.Windows;

namespace Usuario.Services
{
    public class DatoUsuario
    {
        //atributos 

        //metodos
        //constructor vacio 
        public DatoUsuario()
        {

        }

        //METODO PARA CARGAR DATOS AL DATAGRID
        public static List<UsuariosModel> MuestraUsuario()
        {
            List<UsuariosModel> lstUsuarios = new List<UsuariosModel>();

            try
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.conexionDB))
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "SPMOSTRARUSUARIOS";
                        using (DbDataReader dr = command.ExecuteReader())
                        {
                            //recorrer el data reader
                            while (dr.Read())
                            {
                                UsuariosModel user = new UsuariosModel();
                                user.UsuarioId = int.Parse(dr["USUARIOID"].ToString());
                                user.NombreCompleto = dr["NOMBRECOMPLETO"].ToString();
                                user.Correo = dr["CORREO"].ToString();
                                user.Clave = dr["CLAVE"].ToString();

                                //agregar a la lista inicial
                                lstUsuarios.Add(user);
                            }
                        }
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al intentar mostrar los registros: "+ex.Message,"Validacion", 
                    MessageBoxButton.OK,MessageBoxImage.Error);
            }

            return lstUsuarios;
        }//fin de metodo mostrar

        //METODO PARA INSERTAR USUARIOS
        public static int AltaUsuarios(UsuariosModel usuario)
        {
            int res = 0;

            try
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.conexionDB))
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "SPINSERTARUSUARIO";
                        command.Parameters.AddWithValue("@NOMBRE",usuario.NombreCompleto);
                        command.Parameters.AddWithValue("@CORREO", usuario.Correo);
                        command.Parameters.AddWithValue("@CLAVE", usuario.Clave);

                        //ejecutar la consulta
                        command.ExecuteNonQuery();
                        res = 1;
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al intentar insertar registro: " + ex.Message, "Validacion",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return res;
        }//fin de metodo insertar

        //METODO PARA EDITAR USUARIOS
        public static int ModificarUsuario(UsuariosModel usuario,int id)
        {
            int res = 0;

            try
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.conexionDB))
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "SPACTUALIZARUSUARIO";
                        command.Parameters.AddWithValue("@USUARIOID", id);
                        command.Parameters.AddWithValue("@NOMBRE", usuario.NombreCompleto);
                        command.Parameters.AddWithValue("@CORREO", usuario.Correo);
                        command.Parameters.AddWithValue("@CLAVE", usuario.Clave);

                        //ejecutar la consulta
                        command.ExecuteNonQuery();
                        res = id;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al intentar editar el registro: " + ex.Message, "Validacion",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return res;
        }//fin de metodo editar

        //METODO PARA ELIMINAR USUARIOS
        public static int EliminarUsuario(UsuariosModel usuario)
        {
            int res = 0;

            try
            {
                using (var conn = new SqlConnection(Properties.Settings.Default.conexionDB))
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "SPELIMINARUSUARIO";
                        command.Parameters.AddWithValue("@USUARIOID", usuario.UsuarioId);

                        //ejecutar la consulta
                        command.ExecuteNonQuery();
                        res = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al intentar editar el registro: " + ex.Message, "Validacion",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return res;
        }//fin de metodo editar
    }
}

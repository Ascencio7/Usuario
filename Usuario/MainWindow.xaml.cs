using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Usuario
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region DECLARACION DE VARIABLES LOCALES
        //conexion a la base de datos
        SqlConnection conDB = new SqlConnection(Properties.Settings.Default.conexionDB);

        //variable para consultas SQL
        string consultaSQL = null;
        #endregion

        #region METODOS PERSONALIZADOS
        void EncontrarUsuario()
        {
            int resultado = 0;

            //aperturar la BD
            if (conDB.State == ConnectionState.Closed)
            {
                conDB.Open();

                //Consulta para buscar el usuario
                consultaSQL = null;
                consultaSQL = "SELECT dbo.FNENCONTRARUSUARIO(@User,@Password)";

                SqlCommand sqlCmd = new SqlCommand(consultaSQL, conDB);
                sqlCmd.CommandType = CommandType.Text;
                //enviar valores por parametro
                sqlCmd.Parameters.AddWithValue("@User", txtCorreo.Text.Trim());
                sqlCmd.Parameters.AddWithValue("@Password", txtPassword.Password.Trim());

                //Ejecutar consulta SQL
                resultado = Convert.ToInt32(sqlCmd.ExecuteScalar());

                //Evaluar el resultado
                if (resultado == 1)
                {
                    //Instanciar al formulario de usuarios
                    frmUsuarios frmUsuarios = new frmUsuarios();
                    frmUsuarios.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Usuario no encontrado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //cerrar la base de datos o la conexion
                conDB.Close();
            }
        }
        #endregion

        #region EVENTOS DEL FORMULARIO
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Llamada al metodo para validar el ususario
            EncontrarUsuario();
        }
        #endregion
    }
}

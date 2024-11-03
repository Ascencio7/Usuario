using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using Usuario.Models;
using Usuario.Services;

using Usuario.Reports;

//Incluir librerias SQL
using System.Data;

namespace Usuario
{
    /// <summary>
    /// Lógica de interacción para frmUsuarios.xaml
    /// </summary>
    public partial class frmUsuarios : Window
    {
        public frmUsuarios()
        {
            InitializeComponent();
            //Mostrar usuarios
            MostrarUsuarios();
        }

        #region DECLARACION DE VARIABLES LOCALES
        //conexion a la base de datos
        SqlConnection conDB = new SqlConnection(Properties.Settings.Default.conexionDB);

        //variable para consultas SQL
        string consultaSQL = null;

        //Variables de estado
        bool Agregando = false, Editando = false;

        //variable para almacenar el id del usuario
        int usuarioid = 0;//almacenar el ID del usuario actual

        #endregion

        #region METODOS PERSONALIZADOS
        void MostrarUsuarios()
        {
            //assiganr la informacion a mi DataGrid mediante el metodo creado en la clase DatoUsuario
            dgUsuarios.DataContext = DatoUsuario.MuestraUsuario();

            /*
            //Consulta SQL de registros
            consultaSQL = null;
            consultaSQL = "SELECT USUARIOID, NOMBRECOMPLETO, CORREO FROM USUARIOS";

            //cREANDO ELEMENTO SQLDATAADAPTER
            SqlDataAdapter da = new SqlDataAdapter(consultaSQL, conDB);
            //Crear DataTable
            DataTable dt = new DataTable();
            //Llenar el DataTable
            da.Fill(dt);
            //Proceder con el llenado del DataGrid
            dgUsuarios.ItemsSource = dt.DefaultView;
            //cerrar la conexion
            conDB.Close();
            */
        }

        //Metodo para habilitar y deshabilitar objetos del formulario
        void HabilitarObjetos(bool accion)
        {
            if (accion == true)
            {
                //habilitar los objetos
                txtNombreCompleto.IsEnabled = true;
                txtCorreo.IsEnabled = true;
                txtClave.IsEnabled = true;
                txtConfirmacion.IsEnabled = true;
            }
            else
            {
                //deshabilitar los objetos
                txtNombreCompleto.IsEnabled = false;
                txtCorreo.IsEnabled = false;
                txtClave.IsEnabled = false;
                txtConfirmacion.IsEnabled = false;
            }
        }

        //Metodo de limpieza de objetos
        void LimpiarObjetos()
        {
            txtNombreCompleto.Clear();
            txtCorreo.Clear();
            txtClave.Clear();
            txtConfirmacion.Clear();
        }

        //Metodo para controlar la ToolBar
        void controlToolBar()
        {
            //Si el DataGrid no tiene registros
            if (dgUsuarios.Items.Count == 0)
            {
                btnNuevo.Visibility = Visibility.Visible;
                btnGuardar.Visibility = Visibility.Collapsed;
                btnEditar.Visibility = Visibility.Collapsed;
                btnCancelar.Visibility = Visibility.Collapsed;
                btnEliminar.Visibility = Visibility.Collapsed;
            }
            else
            {
                //Si el DataGrid tiene por lo menos un registro
                btnNuevo.Visibility = Visibility.Visible;
                btnGuardar.Visibility = Visibility.Collapsed;
                btnEditar.Visibility = Visibility.Visible;
                btnCancelar.Visibility = Visibility.Collapsed;
                btnEliminar.Visibility = Visibility.Visible;
            }

            if (Agregando || Editando)
            {
                //Si estoy AGREGANDO o EDITANDO un registro
                btnGuardar.Visibility = Visibility.Visible;
                btnCancelar.Visibility = Visibility.Visible;
                btnNuevo.Visibility = Visibility.Collapsed;
                btnEditar.Visibility = Visibility.Collapsed;
                btnEliminar.Visibility = Visibility.Collapsed;
            }

        }

        //Metodo para validar el formulario
        bool ValidarFormulario()
        {
            bool estado = true;
            string mensaje = null;

            //txtNombreCompleto
            if (string.IsNullOrEmpty(txtNombreCompleto.Text))
            {
                estado = false;
                mensaje += "Nombre de usuario\n";
            }

            //txtCorreo
            if (string.IsNullOrEmpty(txtCorreo.Text))
            {
                estado = false;
                mensaje += "Correo\n";
            }

            //txtClave
            if (string.IsNullOrEmpty(txtClave.Password))
            {
                estado = false;
                mensaje += "Clave\n";
            }

            //txtConfirmacion
            if (string.IsNullOrEmpty(txtConfirmacion.Password))
            {
                estado = false;
                mensaje += "Confirmacion\n";
            }

            //validar contraseñas
            if (estado)//si todas las cajas de texto estan llenas
            {
                if (txtClave.Password != txtConfirmacion.Password)
                {
                    estado = false;
                    mensaje += "Las contraseñas no son iguales\n";
                }
            }

            if (!estado)
            {
                MessageBox.Show("Debe completar o cumplir estos campos:\n" + mensaje,
                                "Validación de Formulario",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            return estado;
        }
        #endregion

        #region METODOS DEL FORMULARIO
        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            //Habilitar objetos
            HabilitarObjetos(true);

            //Limpiar objetos
            LimpiarObjetos();

            //Extablecer estado de AGREGANDO
            Agregando = true;
            Editando = false;

            //Configurar toolbar
            controlToolBar();

            //Enviar el enfoque al txt de Nombre
            txtNombreCompleto.Focus();
        }

        //Evento para cancelar del formulario
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            //pedir confirmacion
            if (MessageBox.Show("Desea cancelar la operación?",
                "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                //Limpiar los objetos
                LimpiarObjetos();

                //Bloquear objetos
                HabilitarObjetos(false);

                //Establecer valor de editando o agregando en false
                Agregando = false;
                Editando = false;

                //Configurar el ToolBar
                controlToolBar();
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {

            //Variable para mensajes de estado
            string mensaje;

            if (ValidarFormulario())
            {
                //Recuperar valores del formulario
                UsuariosModel usuario = new UsuariosModel();
                usuario.NombreCompleto = txtNombreCompleto.Text;
                usuario.Correo = txtCorreo.Text;
                usuario.Clave = txtClave.Password;

                //Evaluar si se esta agregando
                if (Agregando)
                {
                    //Llamar al metodo de insercion
                    usuarioid = DatoUsuario.AltaUsuarios(usuario);
                    mensaje = "Registro almacenado correctamente";
                }
                else
                {
                    //Lamar al metodo para editar
                    usuarioid = DatoUsuario.ModificarUsuario(usuario, usuarioid);
                    mensaje = "Registro modificado exitosamente";
                }


                //Evaluar si se ingreso el registro
                if (usuarioid > 0)
                {
                    //mostrar mensaje de confirmacion
                    MessageBox.Show(mensaje,
                        "Validación de formulario",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    //limpiamos cajas de texto
                    LimpiarObjetos();

                    //recargar el datagrid
                    MostrarUsuarios();

                    //reiniciar las variables de estado
                    Agregando = false;
                    Editando = false;

                    //bloquear objetos del formulario
                    HabilitarObjetos(false);

                    //manejar los botones
                    controlToolBar();


                }

                //actualizar mi datagrid
                MostrarUsuarios();

                //variables de estado 
                Agregando = false;
                Editando = false;

                //habilitamos los iconos o botones correspondientes
                controlToolBar();
                //desabilitamos las cajas de texto
                HabilitarObjetos(false);
            }

        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {

        }

        //para poder llenar los controles a partir de la seleccion del DG
        private void dgUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UsuariosModel user = (UsuariosModel)dgUsuarios.SelectedItem;
            if (user == null)
            {
                return;
            }

            //llenar las cajas de texto con lo que traemos del DG
            usuarioid = user.UsuarioId;
            txtNombreCompleto.Text = user.NombreCompleto;
            txtCorreo.Text = user.Correo;
            txtClave.Password = user.Clave;
            txtConfirmacion.Password = user.Clave;
        }

        private void btnEditar_Click_1(object sender, RoutedEventArgs e)
        {
            //Habilitar objetos
            HabilitarObjetos(true);

            //Extablecer estado de EDITANDO
            Agregando = false;
            Editando = true;

            //Configurar toolbar
            controlToolBar();

            //Enviar el enfoque al txt de Nombre
            txtNombreCompleto.Focus();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            //validar si tenemos registros para eliminar
            if (dgUsuarios.Items.Count > 0)
            {
                if (MessageBox.Show("Desea eliminar el registro #" + usuarioid + " ?",
                "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    UsuariosModel usuario = new UsuariosModel();
                    usuario.UsuarioId = usuarioid;

                    //proceder con la eliminacion
                    if (DatoUsuario.EliminarUsuario(usuario) > 0)
                    {
                        MessageBox.Show("Registro eliminado correctamente", "Validacion",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        //Limpiar el formulario
                        LimpiarObjetos();

                        //Recargar el dataGrid
                        MostrarUsuarios();

                        //Reiniciar variables de estado
                        Agregando = false;
                        Editando = false;

                        //habilitar botones
                        controlToolBar();
                    }
                }
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            //Instanciar el reporte de Crystal Reports
            rptUsuarios rpt = new rptUsuarios();
            //Instanciar el formulario visor del reporte
            vsReporte visor = new vsReporte();

            //Confirgurar la carga del reporte
            rpt.Load(@"rptUsuarios.rpt");

            //Establecer el parametro al reporte
            rpt.SetParameterValue("pmUsuarioId", usuarioid);

            //Asignar el origen del reporte al visor
            visor.crvReporte.ViewerCore.ReportSource = rpt;

            //mostrar el visor o la ventana con el reporte
            visor.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Deshabilitar objetos
            HabilitarObjetos(false);
            //Habilitar botones
            controlToolBar();
        }
        #endregion
    }
}

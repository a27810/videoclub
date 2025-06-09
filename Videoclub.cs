using System;
using Terminal.Gui;
using MySql.Data.MySqlClient;
using System.Data;


namespace Videoclub
{    
    public class Videoclub : Window
    {       

        static void Main(string[] args)
        {
            Application.Init();
            VistaPrincipal();
            Application.Run();
            Application.Shutdown();
        }

        static void VistaPrincipal()
        {           

            var top = new Toplevel()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            // Barra de menu con 3 opciones: peliculas, genero y usuarios.
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("Peliculas", new MenuItem [] {
                    new MenuItem ("Listado de peliculas...", "", () => {
                        
                    }),
                    new MenuItem ("Nueva pelicula...", "", () => {
                        
                    })
                }),
                new MenuBarItem ("Genero", new MenuItem [] {
                    new MenuItem ("Listado de generos...", "", () => {
                        
                    }),
                    new MenuItem ("Nuevo genero...", "", () => {
                        
                    })
                }),
                new MenuBarItem ("Usuarios", new MenuItem [] {
                    new MenuItem ("Listado de usuarios...", "", () => {
                        VentanaLogin();
                    }),
                    new MenuItem ("Nuevo usuario...", "", () => {
                        
                    }),
                    new MenuItem ("Editar usuario...", "", () => {
                       
                    }),
                    new MenuItem ("Desactivar usuario...", "", () => {
                        
                    })
                }),
                new MenuBarItem ("Salir", "", () => ConfirmarCerrarConsola())
            });

            // Ventana principal con el nombre del videoclub
            var win = new Window("Videoclub Torrejon de la Calzada")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };

            // Añade la barra de menu y la ventana principal a la vista superior.
            Application.Top.Add(win, menu);
        }

        static void CargarClientes()
        {
            BaseDeDatos objBaseDeDatos = new BaseDeDatos();
            DataTable dtClientes = objBaseDeDatos.LeerTablaClientes();            

            TableView tableView = new TableView()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };

            tableView.Table = dtClientes;
            //Add(tableView);
            Application.Top.Add(tableView);
        }

        static void ConfirmarCerrarConsola()
        {
            var mensaje = MessageBox.Query("Cerrar consola", "¿Seguro que quieres salir?", "Sí", "No");
            if (mensaje == 0) Application.RequestStop();
        }

        static void VentanaLogin()
        {   
            var etiquetaUsuario = new Label("Usuario: ")
            {
                X = 1,
                Y = 1,
                Width = 20,
                Height = 1
            };
            
            var campoUsuario = new TextField()
            {
                X = 20,
                Y = 1,
                Width = Dim.Fill(),
                Height = 1
            };

            var etiquetaPassword = new Label("Password: ")
            {
                X = 1,
                Y = 2,
                Width = 20,
                Height = 1
            };

            var campoPassword = new TextField()
            {
                X = 20,
                Y = 2,
                Width = Dim.Fill(),
                Height = 1
            };

            bool boolHacerLogin = false;
            var botonLogin = new Button("Acceder");
            botonLogin.Clicked += () => Application.RequestStop(); boolHacerLogin = true;

            var botonCancelar = new Button("Cancelar");
            botonCancelar.Clicked += () => Application.RequestStop(); boolHacerLogin = false;

            var dialog = new Dialog("Login", 60, 18, botonLogin, botonCancelar);

            dialog.Add(etiquetaUsuario, campoUsuario, etiquetaPassword, campoPassword);
            Application.Run(dialog);            

            if (boolHacerLogin) HacerLogin((string)campoUsuario.Text, (string)campoPassword.Text);
        }

        static void HacerLogin(string usuario, string password)
        {
            BaseDeDatos objBaseDeDatos = new BaseDeDatos();
            DataTable dtEmpleado = objBaseDeDatos.LeerTablaEmpleados(usuario, password);

            if (dtEmpleado.Rows.Count == 0)
            {
                var etiquetaError = new Label("Usuario incorrecto")
                {
                    X = Pos.Center(),
                    Y = 1,
                    Width = Dim.Fill(),
                    Height = 1
                };

                var botonOK = new Button("OK");
                botonOK.Clicked += () => Application.RequestStop();                
                var dialogError = new Dialog("Error de usuario", 60, 7, botonOK);
                dialogError.Add(etiquetaError);
                Application.Run(dialogError);
            }
            else
            {
                CargarClientes();
            }
                
        }

    }


}
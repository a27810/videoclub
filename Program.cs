using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using Videoclub.Models;

namespace Videoclub
{
    class Program
    {
        static List<Pelicula> peliculas;
        static int siguienteIdPelicula;
        static List<Cliente> clientes;
        static int siguienteIdCliente;
        static List<Alquiler> alquileres;
        static Cliente currentUser;

        static void Main()
        {
            Application.Init();
            var top = Application.Top;

            CargarDatosIniciales();
            EjecutarLogin(top);
            MostrarMenuPrincipal(top);
            Application.Run();
            Application.Shutdown();
        }

        static void CargarDatosIniciales()
        {
            peliculas = new List<Pelicula> {
                new Pelicula { Id = 1, Titulo="El Padrino", Genero="Crimen", AnioEstreno=1972, DuracionMinutos=175, Disponible=true },
                new Pelicula { Id = 2, Titulo="Casablanca", Genero="Romance", AnioEstreno=1942, DuracionMinutos=102, Disponible=false }
            };
            siguienteIdPelicula = peliculas.Max(p => p.Id) + 1;

            clientes = new List<Cliente> {
                new Cliente { Id=1, Nombre="Admin", Email="admin@admin.com", Password="admin", Role="admin", FechaAlta=DateTime.Today, EsPremium=false },
                new Cliente { Id=2, Nombre="Juan Pérez", Email="juan@ej.com", Password="juan123", Role="user", FechaAlta=DateTime.Today.AddMonths(-3), EsPremium=false },
                new Cliente { Id=3, Nombre="Ana García", Email="ana@ej.com", Password="ana123", Role="user", FechaAlta=DateTime.Today.AddYears(-1), EsPremium=true  }
            };
            siguienteIdCliente = clientes.Max(c => c.Id) + 1;

            alquileres = new List<Alquiler>();
        }

        static void EjecutarLogin(Toplevel top)
        {
            var dlg = new Dialog("🔑 Iniciar sesión", 50, 14) { Modal = true };
            var lblEmail    = new Label(2, 2, "Email:");
            var txtEmail    = new TextField(10, 2, 30, "");
            var lblPass     = new Label(2, 4, "Contraseña:");
            var txtPass     = new TextField(12, 4, 28, "") { Secret = true };
            var btnEntrar   = new Button("Entrar")    { X =  2, Y = 10 };
            var btnRegistro = new Button("Registro")  { X = 12, Y = 10 };
            var btnSalir    = new Button("Salir App") { X = 26, Y = 10 };

            btnEntrar.Clicked += () => {
                var user = clientes.FirstOrDefault(c =>
                    c.Email.Equals(txtEmail.Text.ToString().Trim(), StringComparison.OrdinalIgnoreCase)
                    && c.Password == txtPass.Text.ToString());
                if (user == null) {
                    MessageBox.ErrorQuery("Error", "Email o contraseña incorrectos.", "OK");
                    return;
                }
                currentUser = user;
                Application.RequestStop();
            };

            btnRegistro.Clicked += () => {
                top.Remove(dlg);
                MostrarRegistro(top);
            };

            btnSalir.Clicked += () => Environment.Exit(0);

            dlg.Add(lblEmail, txtEmail, lblPass, txtPass, btnEntrar, btnRegistro, btnSalir);
            top.Add(dlg);
            Application.Run(dlg);
        }

        static void MostrarRegistro(Toplevel top)
        {
            var dlg = new Dialog("🆕 Nuevo Usuario", 50, 14) { Modal = true };
            var lblNombre = new Label(2, 2, "Nombre:");
            var txtNombre = new TextField(10, 2, 30, "");
            var lblEmail  = new Label(2, 4, "Email:");
            var txtEmail  = new TextField(10, 4, 30, "");
            var lblPass   = new Label(2, 6, "Contraseña:");
            var txtPass   = new TextField(12, 6, 28, "") { Secret = true };
            var btnGuardar= new Button("Guardar")  { X =  4, Y = 10 };
            var btnCancel = new Button("Cancelar") { X = 20, Y = 10 };

            btnGuardar.Clicked += () => {
                var nombre = txtNombre.Text.ToString().Trim();
                var email  = txtEmail.Text.ToString().Trim();
                var pass   = txtPass.Text.ToString();
                if (string.IsNullOrEmpty(nombre)|| string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass)) {
                    MessageBox.ErrorQuery("Error", "Rellena todos los campos.", "OK");
                    return;
                }
                if (clientes.Any(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase))) {
                    MessageBox.ErrorQuery("Error", "Email ya existe.", "OK");
                    return;
                }
                var nuevo = new Cliente {
                    Id = siguienteIdCliente++,
                    Nombre = nombre,
                    Email = email,
                    Password = pass,
                    Role = "user",
                    FechaAlta = DateTime.Today,
                    EsPremium = false
                };
                clientes.Add(nuevo);
                MessageBox.Query("Listo", "Usuario creado.", "OK");
                top.Remove(dlg);
                EjecutarLogin(top);
            };

            btnCancel.Clicked += () => {
                top.Remove(dlg);
                EjecutarLogin(top);
            };

            dlg.Add(lblNombre, txtNombre, lblEmail, txtEmail, lblPass, txtPass, btnGuardar, btnCancel);
            top.Add(dlg);
            Application.Run(dlg);
        }

        static void MostrarMenuPrincipal(Toplevel top)
        {
            var menuItems = new List<MenuBarItem>();

            var pelItems = new List<MenuItem> {
                new MenuItem("Listado", "Ver todas las películas", ListadoPeliculas),
                new MenuItem("Buscar",  "Buscar por título/género", BuscarPeliculas)
            };
            if (currentUser.Role == "admin") {
                pelItems.Add(null);
                pelItems.Add(new MenuItem("Nuevo",   "Añadir película",     NuevaPelicula));
                pelItems.Add(new MenuItem("Editar",  "Modificar película",  EditarPelicula));
                pelItems.Add(new MenuItem("Eliminar","Borrar película",     BorrarPelicula));
            }
            menuItems.Add(new MenuBarItem("Películas", pelItems.ToArray()));

            if (currentUser.Role == "admin") {
                var usrItems = new List<MenuItem> {
                    new MenuItem("Listado",  "Ver todos los usuarios", ListadoUsuarios),
                    new MenuItem("Eliminar", "Eliminar usuario",       BorrarUsuario)
                };
                menuItems.Add(new MenuBarItem("Usuarios", usrItems.ToArray()));
            }

            var alqItems = new List<MenuItem>();
            if (currentUser.Role == "admin") {
                alqItems.Add(new MenuItem("Nuevo",    "Registrar alquiler",    NuevoAlquiler));
                alqItems.Add(new MenuItem("Devolver", "Registrar devolución",  DevolverAlquiler));
            } else {
                alqItems.Add(new MenuItem("Mis Alq.", "Ver mis alquileres",    ListadoAlquileresUsuario));
            }
            menuItems.Add(new MenuBarItem("Alquiler", alqItems.ToArray()));

            var perfilItems = new List<MenuItem> {
                new MenuItem("Datos",  "Ver/Editar mi perfil", MostrarPerfil),
                new MenuItem("Logout", "Cerrar sesión", () => {
                    Application.RequestStop();
                    Application.Top.RemoveAll();
                    EjecutarLogin(Application.Top);
                    MostrarMenuPrincipal(Application.Top);
                    Application.Run();
                })
            };
            menuItems.Add(new MenuBarItem("Mi perfil", perfilItems.ToArray()));

            menuItems.Add(new MenuBarItem("Salir", new[] {
                new MenuItem("Salir App", "Terminar la aplicación", () => Environment.Exit(0))
            }));

            var menu = new MenuBar(menuItems.ToArray());
            top.Add(menu);
            top.Add(new Window("🎬 Videoclub") {
                X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill()
            });
        }

        // ——— Stubs a implementar ———
        static void ListadoPeliculas()    { /* … */ }
        static void BuscarPeliculas()     { /* … */ }
        static void NuevaPelicula()       { /* … */ }
        static void EditarPelicula()      { /* … */ }
        static void BorrarPelicula()      { /* … */ }

        static void MostrarPerfil()            { /* … */ }
        static void ListadoAlquileresUsuario() { /* … */ }
        static void NuevoAlquiler()            { /* … */ }
        static void DevolverAlquiler()         { /* … */ }

        static void ListadoUsuarios() { /* … */ }
        static void BorrarUsuario()   { /* … */ }
    }
}

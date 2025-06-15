using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
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
        static int siguienteIdAlquiler;
        static Cliente currentUser;

        static readonly string DataDir = "data";
        static readonly string PeliculasPath = Path.Combine(DataDir, "peliculas.json");
        static readonly string ClientesPath  = Path.Combine(DataDir, "clientes.json");
        static readonly string AlquileresPath = Path.Combine(DataDir, "alquileres.json");

        static void Main()
        {
            Application.Init();
            var top = Application.Top;

            CargarDatosIniciales();
            MostrarMenuPublico(top); // Muestra el menú público primero

            Application.Run();
            Application.Shutdown();
        }

        static void MostrarMenuPublico(Toplevel top)
        {
            var dlg = new Dialog("🎬 Videoclub - Zona Pública", 60, 16);

            var btnListar = new Button("Listar Películas") { X = 2, Y = 2 };
            var btnBuscar = new Button("Buscar Películas") { X = 20, Y = 2 };
            var btnLogin  = new Button("Iniciar Sesión")   { X = 2, Y = 6 };
            var btnRegistro = new Button("Registrarse")    { X = 20, Y = 6 };
            var btnSalir  = new Button("Salir")            { X = 2, Y = 10 };

            btnListar.Clicked += () => { ListadoPeliculas(); };
            btnBuscar.Clicked += () => { BuscarPeliculas(); };

            btnLogin.Clicked += () =>
            {
                Application.RequestStop();
                top.Remove(dlg);
                EjecutarLogin(top);
                MostrarMenuPrincipal(top);
                return; // IMPORTANTE: salimos del método tras login
            };

            btnRegistro.Clicked += () =>
            {
                Application.RequestStop();
                top.Remove(dlg);
                MostrarRegistro(top);
                return; // IMPORTANTE: salimos del método tras registro
            };

            btnSalir.Clicked += () => Environment.Exit(0);

            dlg.Add(btnListar, btnBuscar, btnLogin, btnRegistro, btnSalir);
            top.Add(dlg);
            Application.Run(dlg);

            top.Remove(dlg); // Limpia el diálogo tras cada vuelta
        }


        static void CargarDatosIniciales()
        {
            if (!Directory.Exists(DataDir))
                Directory.CreateDirectory(DataDir);

            if (File.Exists(PeliculasPath))
                peliculas = JsonSerializer.Deserialize<List<Pelicula>>(File.ReadAllText(PeliculasPath));
            else
                peliculas = new List<Pelicula> {
                    new Pelicula { Id = 1, Titulo="El Padrino", Genero="Crimen", AnioEstreno=1972, DuracionMinutos=175, Disponible=true },
                    new Pelicula { Id = 2, Titulo="Casablanca", Genero="Romance", AnioEstreno=1942, DuracionMinutos=102, Disponible=false }
                };
            siguienteIdPelicula = peliculas.Any() ? peliculas.Max(p => p.Id) + 1 : 1;

            if (File.Exists(ClientesPath))
                clientes = JsonSerializer.Deserialize<List<Cliente>>(File.ReadAllText(ClientesPath));
            else
                clientes = new List<Cliente> {
                    new Cliente { Id=1, Nombre="Admin", Email="admin@admin.com", Password="admin", Role="admin", FechaAlta=DateTime.Today, EsPremium=false },
                    new Cliente { Id=2, Nombre="Juan Pérez", Email="juan@ej.com", Password="juan123", Role="user", FechaAlta=DateTime.Today.AddMonths(-3), EsPremium=false },
                    new Cliente { Id=3, Nombre="Ana García", Email="ana@ej.com", Password="ana123", Role="user", FechaAlta=DateTime.Today.AddYears(-1), EsPremium=true  }
                };
            siguienteIdCliente = clientes.Any() ? clientes.Max(c => c.Id) + 1 : 1;

            if (File.Exists(AlquileresPath))
                alquileres = JsonSerializer.Deserialize<List<Alquiler>>(File.ReadAllText(AlquileresPath));
            else
                alquileres = new List<Alquiler>();
            siguienteIdAlquiler = alquileres.Any() ? alquileres.Max(a => a.Id) + 1 : 1;
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
                top.Remove(dlg);
            };

            btnRegistro.Clicked += () => {
                Application.RequestStop();
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
                GuardarClientes();
                MessageBox.Query("Listo", "Usuario creado.", "OK");
                Application.RequestStop();
                top.Remove(dlg);
                EjecutarLogin(top);
            };

            btnCancel.Clicked += () => {
                Application.RequestStop();
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

            // — Películas —
            var pelItems = new List<MenuItem> {
                new MenuItem("Listado", "Ver todas las películas", ListadoPeliculas),
                new MenuItem("Buscar",   "Buscar por título/género", BuscarPeliculas)
            };
            if (currentUser.Role == "admin") {
                pelItems.Add(null);
                pelItems.Add(new MenuItem("Nuevo",   "Añadir película",     NuevaPelicula));
                pelItems.Add(new MenuItem("Editar",  "Modificar película",   EditarPelicula));
                pelItems.Add(new MenuItem("Eliminar","Borrar película",      BorrarPelicula));
            }
            menuItems.Add(new MenuBarItem("Películas", pelItems.ToArray()));

            // — Usuarios (solo admin) —
            if (currentUser.Role == "admin") {
                var usrItems = new List<MenuItem> {
                    new MenuItem("Listado",  "Ver todos los usuarios",    ListadoUsuarios),
                    new MenuItem("Editar",   "Editar usuario",            EditarUsuario),
                    new MenuItem("Eliminar","Eliminar usuario",           BorrarUsuario)
                };
                menuItems.Add(new MenuBarItem("Usuarios", usrItems.ToArray()));
            }

            // — Alquileres —
            var alqItems = new List<MenuItem>();
            if (currentUser.Role == "admin") {
                alqItems.Add(new MenuItem("Nuevo",     "Registrar alquiler",    NuevoAlquiler));
                alqItems.Add(new MenuItem("Devolver",  "Registrar devolución",   DevolverAlquiler));
            } else {
                alqItems.Add(new MenuItem("Mis Alq.",  "Ver mis alquileres",     ListadoAlquileresUsuario));
            }
            menuItems.Add(new MenuBarItem("Alquiler", alqItems.ToArray()));

            // — Mi perfil —
            var perfilItems = new List<MenuItem> {
                new MenuItem("Datos",   "Ver/Editar mi perfil", MostrarPerfil),
                new MenuItem("Logout", "Cerrar sesión", () => {
                    Application.RequestStop(); // Cierra el menú principal actual
                    currentUser = null;        // Elimina el usuario actual

                    Application.MainLoop.Invoke(() =>
                    {
                        Application.Shutdown();
                        Application.Init();
                        MostrarMenuPublico(Application.Top);
                        Application.Run();
                    });
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

        // --- CRUD PELÍCULAS ---
        static void ListadoPeliculas()
        {
            var dlg = new Dialog("🎬 Todas las Películas", 60, 20);

            var lista = peliculas
                .Select(p => $"{p.Titulo} ({p.Genero}, {p.AnioEstreno}) - {(p.Disponible ? "Disponible" : "Alquilada")}")
                .ToList();

            var listView = new ListView(lista)
            {
                X = 2, Y = 2, Width = 55, Height = 12
            };

            var btnCerrar = new Button("Cerrar")
            {
                X = 20, Y = 16
            };
            btnCerrar.Clicked += () => Application.RequestStop();

            dlg.Add(listView, btnCerrar);
            Application.Run(dlg);
        }

        static void BuscarPeliculas()
        {
            var dlg = new Dialog("🔍 Buscar Película", 60, 20);

            var lblBuscar = new Label(2, 2, "Buscar por título o género:");
            var txtBuscar = new TextField("")
            {
                X = 2, Y = 3, Width = 50
            };

            var btnBuscar = new Button("Buscar")
            {
                X = 2, Y = 5
            };

            var listaResultados = new ListView()
            {
                X = 2, Y = 7, Width = 55, Height = 8
            };

            btnBuscar.Clicked += () =>
            {
                string termino = txtBuscar.Text.ToString().Trim().ToLower();
                var resultados = peliculas
                    .Where(p => p.Titulo.ToLower().Contains(termino) || p.Genero.ToLower().Contains(termino))
                    .Select(p => $"{p.Titulo} ({p.Genero}, {p.AnioEstreno}) - {(p.Disponible ? "Disponible" : "Alquilada")}")
                    .ToList();

                listaResultados.SetSource(resultados);

                if (!resultados.Any())
                {
                    MessageBox.Query("Sin resultados", "No se encontraron coincidencias.", "OK");
                }
            };

            var btnCerrar = new Button("Cerrar")
            {
                X = 20, Y = 16
            };
            btnCerrar.Clicked += () => Application.RequestStop();

            dlg.Add(lblBuscar, txtBuscar, btnBuscar, listaResultados, btnCerrar);
            Application.Run(dlg);
        }

        static void NuevaPelicula()
        {
            var dlg = new Dialog("🎬 Añadir Película", 50, 16);

            var lblTitulo = new Label(2, 2, "Título:");
            var txtTitulo = new TextField(10, 2, 35, "");
            var lblGenero = new Label(2, 4, "Género:");
            var txtGenero = new TextField(10, 4, 35, "");
            var lblAnio = new Label(2, 6, "Año:");
            var txtAnio = new TextField(10, 6, 8, "");
            var lblDuracion = new Label(20, 6, "Duración:");
            var txtDuracion = new TextField(30, 6, 8, "");
            var chkDisponible = new CheckBox(10, 8, "Disponible", true);

            var btnGuardar = new Button("Guardar") { X = 4, Y = 12 };
            var btnCancelar = new Button("Cancelar") { X = 20, Y = 12 };

            btnGuardar.Clicked += () =>
            {
                if (string.IsNullOrWhiteSpace(txtTitulo.Text.ToString()) ||
                    string.IsNullOrWhiteSpace(txtGenero.Text.ToString()) ||
                    string.IsNullOrWhiteSpace(txtAnio.Text.ToString()) ||
                    string.IsNullOrWhiteSpace(txtDuracion.Text.ToString()))
                {
                    MessageBox.ErrorQuery("Error", "Todos los campos son obligatorios.", "OK");
                    return;
                }
                if (!int.TryParse(txtAnio.Text.ToString(), out int anio) ||
                    !int.TryParse(txtDuracion.Text.ToString(), out int duracion))
                {
                    MessageBox.ErrorQuery("Error", "Año y duración deben ser números.", "OK");
                    return;
                }
                var nuevaPelicula = new Pelicula
                {
                    Id = siguienteIdPelicula++,
                    Titulo = txtTitulo.Text.ToString(),
                    Genero = txtGenero.Text.ToString(),
                    AnioEstreno = anio,
                    DuracionMinutos = duracion,
                    Disponible = chkDisponible.Checked
                };
                peliculas.Add(nuevaPelicula);
                GuardarPeliculas();
                MessageBox.Query("Ok", "Película añadida.", "OK");
                Application.RequestStop();
            };

            btnCancelar.Clicked += () => Application.RequestStop();

            dlg.Add(lblTitulo, txtTitulo, lblGenero, txtGenero, lblAnio, txtAnio, lblDuracion, txtDuracion, chkDisponible, btnGuardar, btnCancelar);
            Application.Run(dlg);
        }

        static void EditarPelicula()
        {
            if (!peliculas.Any())
            {
                MessageBox.Query("Info", "No hay películas para editar.", "OK");
                return;
            }
            var dlg = new Dialog("Editar Película", 60, 20);

            var lista = new ListView(peliculas.Select(p => $"{p.Titulo} ({p.AnioEstreno})").ToList())
            {
                X = 2, Y = 2, Width = 50, Height = 8
            };
            dlg.Add(lista);

            var lblInfo = new Label(2, 11, "Selecciona una película y pulsa Editar.");
            var btnEditar = new Button("Editar") { X = 2, Y = 14 };
            var btnCerrar = new Button("Cerrar") { X = 16, Y = 14 };

            btnEditar.Clicked += () =>
            {
                int idx = lista.SelectedItem;
                if (idx < 0) return;
                var peli = peliculas[idx];
                var editDlg = new Dialog("Modificar Película", 50, 16);

                var txtTitulo = new TextField(peli.Titulo) { X = 10, Y = 2, Width = 35 };
                var txtGenero = new TextField(peli.Genero) { X = 10, Y = 4, Width = 35 };
                var txtAnio = new TextField(peli.AnioEstreno.ToString()) { X = 10, Y = 6, Width = 8 };
                var txtDuracion = new TextField(peli.DuracionMinutos.ToString()) { X = 30, Y = 6, Width = 8 };
                var chkDisponible = new CheckBox(10, 8, "Disponible", peli.Disponible);

                var btnGuardar = new Button("Guardar") { X = 4, Y = 12 };
                var btnCancel = new Button("Cancelar") { X = 20, Y = 12 };

                btnGuardar.Clicked += () =>
                {
                    if (string.IsNullOrWhiteSpace(txtTitulo.Text.ToString()) ||
                        string.IsNullOrWhiteSpace(txtGenero.Text.ToString()) ||
                        string.IsNullOrWhiteSpace(txtAnio.Text.ToString()) ||
                        string.IsNullOrWhiteSpace(txtDuracion.Text.ToString()))
                    {
                        MessageBox.ErrorQuery("Error", "Todos los campos son obligatorios.", "OK");
                        return;
                    }
                    if (!int.TryParse(txtAnio.Text.ToString(), out int anio) ||
                        !int.TryParse(txtDuracion.Text.ToString(), out int duracion))
                    {
                        MessageBox.ErrorQuery("Error", "Año y duración deben ser números.", "OK");
                        return;
                    }
                    peli.Titulo = txtTitulo.Text.ToString();
                    peli.Genero = txtGenero.Text.ToString();
                    peli.AnioEstreno = anio;
                    peli.DuracionMinutos = duracion;
                    peli.Disponible = chkDisponible.Checked;

                    GuardarPeliculas();
                    MessageBox.Query("Ok", "Película editada.", "OK");
                    Application.RequestStop();
                };

                btnCancel.Clicked += () => Application.RequestStop();
                editDlg.Add(
                    new Label(2, 2, "Título:"), txtTitulo,
                    new Label(2, 4, "Género:"), txtGenero,
                    new Label(2, 6, "Año:"), txtAnio,
                    new Label(20, 6, "Duración:"), txtDuracion,
                    chkDisponible, btnGuardar, btnCancel
                );
                Application.Run(editDlg);
            };
            btnCerrar.Clicked += () => Application.RequestStop();

            dlg.Add(lblInfo, btnEditar, btnCerrar);
            Application.Run(dlg);
        }

        static void BorrarPelicula()
        {
            if (!peliculas.Any())
            {
                MessageBox.Query("Info", "No hay películas para borrar.", "OK");
                return;
            }
            var dlg = new Dialog("Eliminar Película", 60, 16);
            var lista = new ListView(peliculas.Select(p => $"{p.Titulo} ({p.AnioEstreno})").ToList())
            {
                X = 2, Y = 2, Width = 50, Height = 8
            };

            var btnBorrar = new Button("Borrar") { X = 2, Y = 12 };
            var btnCerrar = new Button("Cerrar") { X = 16, Y = 12 };

            btnBorrar.Clicked += () =>
            {
                int idx = lista.SelectedItem;
                if (idx < 0) return;
                var peli = peliculas[idx];

                bool alquilada = alquileres.Any(a => a.PeliculaId == peli.Id && !a.Devuelto);
                if (alquilada)
                {
                    MessageBox.ErrorQuery("Error", "No se puede borrar: la película está alquilada.", "OK");
                    return;
                }

                if (MessageBox.Query("Confirmar", $"¿Borrar '{peli.Titulo}'?", "Sí", "No") == 0)
                {
                    peliculas.RemoveAt(idx);
                    GuardarPeliculas();
                    MessageBox.Query("Ok", "Película eliminada.", "OK");
                    Application.RequestStop();
                }
            };
            btnCerrar.Clicked += () => Application.RequestStop();

            dlg.Add(lista, btnBorrar, btnCerrar);
            Application.Run(dlg);
        }

        // --- CRUD ALQUILERES ---
        static void NuevoAlquiler()
        {
            var dlg = new Dialog("Nuevo Alquiler", 60, 18);

            var disponibles = peliculas.Where(p => p.Disponible).ToList();
            if (!disponibles.Any())
            {
                MessageBox.Query("Info", "No hay películas disponibles.", "OK");
                return;
            }
            var lista = new ListView(disponibles.Select(p => $"{p.Titulo} ({p.AnioEstreno})").ToList())
            {
                X = 2, Y = 2, Width = 52, Height = 7
            };

            var lblCliente = new Label(2, 10, "Cliente:");
            var clientesUser = clientes.Where(c => c.Role == "user").ToList();
            var comboClientes = new ComboBox()
            {
                X = 10, Y = 10, Width = 32
            };
            comboClientes.SetSource(clientesUser.Select(c => c.Nombre + " - " + c.Email).ToList());

            var btnAlquilar = new Button("Alquilar") { X = 4, Y = 14 };
            var btnCancelar = new Button("Cancelar") { X = 20, Y = 14 };

            btnAlquilar.Clicked += () =>
            {
                int idxPeli = lista.SelectedItem;
                int idxCli = comboClientes.SelectedItem;
                if (idxPeli < 0 || idxCli < 0) return;

                var peli = disponibles[idxPeli];
                var cliente = clientesUser[idxCli];

                var alq = new Alquiler
                {
                    Id = siguienteIdAlquiler++,
                    PeliculaId = peli.Id,
                    ClienteId = cliente.Id,
                    FechaAlquiler = DateTime.Now,
                    Devuelto = false
                };
                alquileres.Add(alq);
                peli.Disponible = false;
                GuardarAlquileres();
                GuardarPeliculas();
                MessageBox.Query("Ok", "Alquiler registrado.", "OK");
                Application.RequestStop();
            };

            btnCancelar.Clicked += () => Application.RequestStop();

            dlg.Add(lista, lblCliente, comboClientes, btnAlquilar, btnCancelar);
            Application.Run(dlg);
        }

        static void DevolverAlquiler()
        {
            if (!alquileres.Any(a => !a.Devuelto))
            {
                MessageBox.Query("Info", "No hay alquileres pendientes.", "OK");
                return;
            }
            var dlg = new Dialog("Registrar Devolución", 60, 18);

            var pendientes = alquileres.Where(a => !a.Devuelto).ToList();
            var lista = new ListView(pendientes.Select(a =>
            {
                var peli = peliculas.FirstOrDefault(p => p.Id == a.PeliculaId);
                var cli = clientes.FirstOrDefault(c => c.Id == a.ClienteId);
                return $"{peli?.Titulo ?? "?"} - {cli?.Nombre ?? "?"} ({a.FechaAlquiler:d})";
            }).ToList())
            {
                X = 2, Y = 2, Width = 52, Height = 8
            };

            var btnDevolver = new Button("Devolver") { X = 4, Y = 14 };
            var btnCancelar = new Button("Cancelar") { X = 20, Y = 14 };

            btnDevolver.Clicked += () =>
            {
                int idx = lista.SelectedItem;
                if (idx < 0) return;
                var alq = pendientes[idx];
                alq.Devuelto = true;
                alq.FechaDevolucion = DateTime.Now;

                var peli = peliculas.FirstOrDefault(p => p.Id == alq.PeliculaId);
                if (peli != null) peli.Disponible = true;
                GuardarAlquileres();
                GuardarPeliculas();
                MessageBox.Query("Ok", "Película devuelta.", "OK");
                Application.RequestStop();
            };

            btnCancelar.Clicked += () => Application.RequestStop();

            dlg.Add(lista, btnDevolver, btnCancelar);
            Application.Run(dlg);
        }

        // --- ZONA PRIVADA USUARIO ---
        static void ListadoAlquileresUsuario()
        {
            var dlg = new Dialog("🎬 Mis Alquileres", 60, 20);

            var misAlquileres = alquileres
                .Where(a => a.ClienteId == currentUser.Id)
                .ToList();

            var lista = new List<string>();
            foreach (var a in misAlquileres)
            {
                var peli = peliculas.FirstOrDefault(p => p.Id == a.PeliculaId);
                if (peli != null)
                {
                    string estado = a.Devuelto ? "Devuelto" : "En curso";
                    lista.Add($"{peli.Titulo} ({a.FechaAlquiler:d}) - {estado}");
                }
            }

            var listView = new ListView(lista)
            {
                X = 2, Y = 2, Width = 55, Height = 12
            };

            var btnCerrar = new Button("Cerrar")
            {
                X = 20, Y = 16
            };
            btnCerrar.Clicked += () => Application.RequestStop();

            if (lista.Count == 0)
                dlg.Add(new Label(2, 2, "No tienes alquileres registrados."));
            else
                dlg.Add(listView);

            dlg.Add(btnCerrar);
            Application.Run(dlg);
        }

        // --- CRUD USUARIOS ---
        static void ListadoUsuarios()
        {
            var dlg = new Dialog("👥 Listado Usuarios", 60, 18);

            var lista = clientes.Select(c =>
                $"{c.Nombre} - {c.Email} - {(c.Role == "admin" ? "Admin" : (c.EsPremium ? "Premium" : "Usuario"))}"
            ).ToList();

            var listView = new ListView(lista)
            {
                X = 2, Y = 2, Width = 54, Height = 11
            };

            var btnCerrar = new Button("Cerrar") { X = 20, Y = 15 };
            btnCerrar.Clicked += () => Application.RequestStop();

            dlg.Add(listView, btnCerrar);
            Application.Run(dlg);
        }

        static void EditarUsuario()
        {
            var dlg = new Dialog("Editar Usuario", 60, 20);

            var lista = clientes.Select(c => $"{c.Nombre} - {c.Email}").ToList();
            var listView = new ListView(lista)
            {
                X = 2, Y = 2, Width = 50, Height = 8
            };
            dlg.Add(listView);

            var lblInfo = new Label(2, 11, "Selecciona un usuario y pulsa Editar.");
            var btnEditar = new Button("Editar") { X = 2, Y = 14 };
            var btnCerrar = new Button("Cerrar") { X = 16, Y = 14 };

            btnEditar.Clicked += () =>
            {
                int idx = listView.SelectedItem;
                if (idx < 0) return;
                var user = clientes[idx];
                if (user.Role == "admin")
                {
                    MessageBox.ErrorQuery("Error", "No se puede editar el administrador.", "OK");
                    return;
                }
                var editDlg = new Dialog("Modificar Usuario", 50, 16);

                var txtNombre = new TextField(user.Nombre) { X = 10, Y = 2, Width = 35 };
                var txtEmail = new TextField(user.Email) { X = 10, Y = 4, Width = 35 };
                var txtPass = new TextField(user.Password) { X = 12, Y = 6, Width = 28, Secret = true };
                var chkPremium = new CheckBox(10, 8, "Premium", user.EsPremium);

                var btnGuardar = new Button("Guardar") { X = 4, Y = 12 };
                var btnCancel = new Button("Cancelar") { X = 20, Y = 12 };

                btnGuardar.Clicked += () =>
                {
                    if (string.IsNullOrWhiteSpace(txtNombre.Text.ToString()) ||
                        string.IsNullOrWhiteSpace(txtEmail.Text.ToString()) ||
                        string.IsNullOrWhiteSpace(txtPass.Text.ToString()))
                    {
                        MessageBox.ErrorQuery("Error", "Todos los campos son obligatorios.", "OK");
                        return;
                    }
                    // Controlar email duplicado (excepto el mismo usuario)
                    if (clientes.Any(c => c.Email.Equals(txtEmail.Text.ToString(), StringComparison.OrdinalIgnoreCase)
                        && c.Id != user.Id))
                    {
                        MessageBox.ErrorQuery("Error", "Email ya existe.", "OK");
                        return;
                    }
                    user.Nombre = txtNombre.Text.ToString();
                    user.Email = txtEmail.Text.ToString();
                    user.Password = txtPass.Text.ToString();
                    user.EsPremium = chkPremium.Checked;
                    GuardarClientes();
                    MessageBox.Query("Ok", "Usuario editado.", "OK");
                    Application.RequestStop();
                };

                btnCancel.Clicked += () => Application.RequestStop();

                editDlg.Add(
                    new Label(2, 2, "Nombre:"), txtNombre,
                    new Label(2, 4, "Email:"), txtEmail,
                    new Label(2, 6, "Contraseña:"), txtPass,
                    chkPremium, btnGuardar, btnCancel
                );
                Application.Run(editDlg);
            };
            btnCerrar.Clicked += () => Application.RequestStop();

            dlg.Add(lblInfo, btnEditar, btnCerrar);
            Application.Run(dlg);
        }

        static void BorrarUsuario()
        {
            var dlg = new Dialog("Eliminar Usuario", 60, 16);
            var lista = clientes.Select(c => $"{c.Nombre} - {c.Email}").ToList();
            var listView = new ListView(lista)
            {
                X = 2, Y = 2, Width = 50, Height = 8
            };

            var btnBorrar = new Button("Borrar") { X = 2, Y = 12 };
            var btnCerrar = new Button("Cerrar") { X = 16, Y = 12 };

            btnBorrar.Clicked += () =>
            {
                int idx = listView.SelectedItem;
                if (idx < 0) return;
                var user = clientes[idx];

                if (user.Role == "admin")
                {
                    MessageBox.ErrorQuery("Error", "No se puede borrar el administrador.", "OK");
                    return;
                }

                // Si el usuario tiene alquileres NO DEVUELTOS, no puede borrarse
                if (alquileres.Any(a => a.ClienteId == user.Id && !a.Devuelto))
                {
                    MessageBox.ErrorQuery("Error", "No se puede borrar: usuario con alquileres activos.", "OK");
                    return;
                }

                if (MessageBox.Query("Confirmar", $"¿Borrar '{user.Nombre}'?", "Sí", "No") == 0)
                {
                    clientes.RemoveAt(idx);
                    GuardarClientes();
                    MessageBox.Query("Ok", "Usuario eliminado.", "OK");
                    Application.RequestStop();
                }
            };
            btnCerrar.Clicked += () => Application.RequestStop();

            dlg.Add(listView, btnBorrar, btnCerrar);
            Application.Run(dlg);
        }

        static void MostrarPerfil()
        {
            var dlg = new Dialog("Mi perfil", 50, 16);
            var lblNombre = new Label(2, 2, $"Nombre: {currentUser.Nombre}");
            var lblEmail = new Label(2, 4, $"Email: {currentUser.Email}");
            var lblRol = new Label(2, 6, $"Rol: {(currentUser.Role == "admin" ? "Administrador" : (currentUser.EsPremium ? "Premium" : "Usuario"))}");
            var lblFecha = new Label(2, 8, $"Alta: {currentUser.FechaAlta:d}");
            var btnCerrar = new Button("Cerrar") { X = 15, Y = 12 };
            btnCerrar.Clicked += () => Application.RequestStop();

            dlg.Add(lblNombre, lblEmail, lblRol, lblFecha, btnCerrar);
            Application.Run(dlg);
        }

        static void GuardarPeliculas()
        {
            File.WriteAllText(PeliculasPath, JsonSerializer.Serialize(peliculas, new JsonSerializerOptions { WriteIndented = true }));
        }
        static void GuardarClientes()
        {
            File.WriteAllText(ClientesPath, JsonSerializer.Serialize(clientes, new JsonSerializerOptions { WriteIndented = true }));
        }
        static void GuardarAlquileres()
        {
            File.WriteAllText(AlquileresPath, JsonSerializer.Serialize(alquileres, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}

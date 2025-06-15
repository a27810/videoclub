using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Videoclub.Models;

namespace Videoclub.Database
{
    public static class DataStore
    {
        const string PelisFile    = "peliculas.json";
        const string ClientesFile = "clientes.json";
        const string AlqFile      = "alquileres.json";

        static JsonSerializerOptions opts = new JsonSerializerOptions {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static List<Pelicula> LoadPeliculas()
        {
            return Load<List<Pelicula>>(PelisFile);
        }

        public static void SavePeliculas(List<Pelicula> list)
        {
            Save(PelisFile, list);
        }

        public static List<Cliente> LoadClientes()
        {
            return Load<List<Cliente>>(ClientesFile);
        }

        public static void SaveClientes(List<Cliente> list)
        {
            Save(ClientesFile, list);
        }

        public static List<Alquiler> LoadAlquileres()
        {
            return Load<List<Alquiler>>(AlqFile);
        }

        public static void SaveAlquileres(List<Alquiler> list)
        {
            Save(AlqFile, list);
        }

        // genérico
        static T Load<T>(string file) where T : class
        {
            try
            {
                if (!File.Exists(file))
                    return null;

                var json = File.ReadAllText(file);
                return JsonSerializer.Deserialize<T>(json, opts);
            }
            catch
            {
                return null;
            }
        }

        static void Save<T>(string file, T obj)
        {
            try
            {
                var json = JsonSerializer.Serialize(obj, opts);
                File.WriteAllText(file, json);
            }
            catch (Exception ex)
            {
                // opcional: loguear en consola
                Console.Error.WriteLine($"Error guardando {file}: {ex.Message}");
            }
        }
    }
}

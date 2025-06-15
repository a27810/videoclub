using System;

namespace Videoclub.Models
{
    public class Pelicula
    {
        public int     Id              { get; set; }
        public string  Titulo          { get; set; }
        public string  Genero          { get; set; }
        public int     AnioEstreno     { get; set; }
        public int     DuracionMinutos { get; set; }
        public bool    Disponible      { get; set; }
    }
}

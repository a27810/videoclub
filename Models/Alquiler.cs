using System;

namespace Videoclub.Models
{
    public class Alquiler
    {
        public int Id { get; set; }
        public Cliente Cliente { get; set; }
        public Pelicula Pelicula { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal Precio { get; set; }
        public bool Devuelto { get; set; }
    }
}

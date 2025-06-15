using System;

namespace Videoclub.Models
{
    public class Alquiler
    {
        public int       Id              { get; set; }
        public int       PeliculaId      { get; set; }
        public int       UsuarioId       { get; set; }
        public DateTime  FechaAlquiler   { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public decimal   Precio          { get; set; }
    }
}

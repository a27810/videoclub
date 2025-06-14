using System;
using System.Collections.Generic;

namespace Videoclub.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaAlta { get; set; }
        public bool EsPremium { get; set; }
        public List<Alquiler> Alquileres { get; set; } = new List<Alquiler>();
    }
}

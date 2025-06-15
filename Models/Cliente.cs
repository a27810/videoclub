using System;

namespace Videoclub.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }      // "user" o "admin"
        public DateTime FechaAlta { get; set; }
        public bool EsPremium { get; set; }
    }
}

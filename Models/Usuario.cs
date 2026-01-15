using System;
using System.Collections.Generic;

namespace CaceiWeb.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        
        // Navigation properties
        public string RolNombre { get; set; }
        public List<Academia> Academias { get; set; }

        public Usuario()
        {
            FechaCreacion = DateTime.Now;
            Activo = true;
            Academias = new List<Academia>();
        }
    }
}

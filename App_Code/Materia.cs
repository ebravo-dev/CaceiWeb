using System;

namespace CaceiWeb.Models
{
    public class Materia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public int? AcademiaId { get; set; }
        public bool Activo { get; set; }
        
        // Navigation
        public string AcademiaNombre { get; set; }

        public Materia()
        {
            Activo = true;
        }
    }
}

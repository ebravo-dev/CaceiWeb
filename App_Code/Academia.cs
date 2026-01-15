using System;

namespace CaceiWeb.Models
{
    public class Academia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public bool Activo { get; set; }

        public Academia()
        {
            Activo = true;
        }
    }
}

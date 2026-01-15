using System;

namespace CaceiWeb.Models
{
    /// <summary>
    /// Modelo de datos para un registro de captura
    /// Puedes agregar o modificar campos según tus necesidades
    /// </summary>
    public class Registro
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Matricula { get; set; }
        public string Carrera { get; set; }
        public string Semestre { get; set; }
        public string Comentarios { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }

        public Registro()
        {
            FechaRegistro = DateTime.Now;
            Activo = true;
        }
    }
}

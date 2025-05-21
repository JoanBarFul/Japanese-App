using SQLite;
using System;

namespace Japanese_App.Models
{
    [Table("Actividad")]
    public class Actividad
    {
        [PrimaryKey, AutoIncrement]
        public int idActividad { get; set; }

        [MaxLength(100)]
        public string nombre { get; set; }

        public string descripcion { get; set; }

        public string CSV { get; set; }

        public DateTime Fecha { get; set; } 
    }
}

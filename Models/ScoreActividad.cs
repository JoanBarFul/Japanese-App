using SQLite;

namespace Japanese_App.Models
{
    [Table("ScoreActividad")]
    public class ScoreActividad
    {
        [PrimaryKey, AutoIncrement]
        public int idScoreActividad { get; set; }

        // FK a Actividad
        public int idActividad { get; set; }

        public bool isCompletada { get; set; }

        public double porcentajeMaximo { get; set; }
    }
}

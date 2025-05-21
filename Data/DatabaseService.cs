using SQLite;
using Japanese_App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Japanese_App.Data
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);

            // Crear tablas si no existen
            _db.CreateTableAsync<Actividad>().Wait();
            _db.CreateTableAsync<ScoreActividad>().Wait();
        }

        // Obtener todas las actividades
        public async Task<List<Actividad>> ObtenerActividadesAsync()
        {
            var list = await _db.Table<Actividad>().ToListAsync();
            System.Diagnostics.Debug.WriteLine($"Número de actividades en DB: {list.Count}");
            return list;
        }

        // Insertar nueva actividad
        public Task<int> GuardarActividadAsync(Actividad actividad) =>
            _db.InsertAsync(actividad);

        // Eliminar actividad
        public Task<int> EliminarActividadAsync(Actividad actividad) =>
            _db.DeleteAsync(actividad);

        // Obtener lista de scores
        public Task<List<ScoreActividad>> ObtenerScoresAsync() =>
            _db.Table<ScoreActividad>().ToListAsync();

        // Insertar nuevo score
        public Task<int> GuardarScoreAsync(ScoreActividad score) =>
            _db.InsertAsync(score);

        // Actualizar score existente
        public Task<int> ActualizarScoreAsync(ScoreActividad score) =>
            _db.UpdateAsync(score);

        // Eliminar score
        public Task<int> EliminarScoreAsync(ScoreActividad score) =>
            _db.DeleteAsync(score);

        // Contar actividades (nuevo método)
        public Task<int> ContarActividadesAsync() =>
            _db.Table<Actividad>().CountAsync();

        // Insertar actividad de prueba (nuevo método)
        public async Task InsertarActividadDePruebaAsync()
        {
            var actividadPrueba = new Actividad
            {
                nombre = "Actividad de prueba",
                descripcion = "Descripción de prueba para testear",
                Fecha = DateTime.Now,
                CSV = ""
            };

            await _db.InsertAsync(actividadPrueba);
            System.Diagnostics.Debug.WriteLine("Actividad de prueba insertada.");
        }
    }
}

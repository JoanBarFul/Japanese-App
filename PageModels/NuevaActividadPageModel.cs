using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Japanese_App.Models;
using Japanese_App.Data;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Japanese_App.PageModels
{
    public partial class NuevaActividadPageModel : ObservableObject
    {
        private readonly DatabaseService _db;

        public NuevaActividadPageModel(DatabaseService db)
        {
            _db = db;
            Fecha = DateTime.Today;
        }

        [ObservableProperty]
        private string nombre;

        [ObservableProperty]
        private string descripcion;

        [ObservableProperty]
        private DateTime fecha;

        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        private async Task Guardar()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                await AppShell.DisplaySnackbarAsync("El nombre es obligatorio.");
                return;
            }

            IsBusy = true;

            var nuevaActividad = new Actividad
            {
                nombre = Nombre,
                descripcion = Descripcion,
                Fecha = Fecha
            };

            await _db.GuardarActividadAsync(nuevaActividad);

            await AppShell.DisplayToastAsync("Actividad guardada correctamente.");
            IsBusy = false;

            await Shell.Current.GoToAsync(".."); // Volver a la página anterior
        }

        [RelayCommand]
        public async Task ImportarDesdeCsv(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    await AppShell.DisplaySnackbarAsync("Archivo inválido o no encontrado.");
                    return;
                }

                IsBusy = true;

                var actividades = new List<Actividad>();

                using var reader = new StreamReader(filePath);
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var parts = line.Split(',');

                    if (parts.Length >= 3 && DateTime.TryParse(parts[2], out DateTime fechaParsed))
                    {
                        actividades.Add(new Actividad
                        {
                            nombre = parts[0],
                            descripcion = parts[1],
                            Fecha = fechaParsed
                        });
                    }
                }

                foreach (var actividad in actividades)
                {
                    await _db.GuardarActividadAsync(actividad);
                }

                await AppShell.DisplayToastAsync($"Se importaron {actividades.Count} actividades.");
            }
            catch (Exception ex)
            {
                await AppShell.DisplaySnackbarAsync($"Error al importar CSV: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

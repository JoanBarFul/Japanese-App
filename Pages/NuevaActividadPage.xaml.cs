using Japanese_App.PageModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.ApplicationModel;

namespace Japanese_App.Pages
{
    public partial class NuevaActividadPage : ContentPage
    {
        public NuevaActividadPage(NuevaActividadPageModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private async void OnImportCsvClicked(object sender, EventArgs e)
        {
            // DEBUG: Confirmar que el botón fue presionado
            await DisplayAlert("DEBUG", "Botón CSV presionado", "OK");

            await AppShell.DisplayToastAsync("Botón CSV presionado");

            // Solicitar permiso de almacenamiento en Android
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    await AppShell.DisplaySnackbarAsync("Permiso de almacenamiento denegado.");
                    return;
                }
            }

            try
            {
                var customCsvFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".csv" } },
                    { DevicePlatform.Android, new[] { "text/csv" } },
                    { DevicePlatform.iOS, new[] { "public.comma-separated-values-text" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.comma-separated-values-text" } }
                });

                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona un archivo CSV",
                    FileTypes = customCsvFileType
                });

                if (result != null && BindingContext is NuevaActividadPageModel vm)
                {
                    await DisplayAlert("DEBUG", $"Archivo seleccionado: {result.FileName}", "OK");

                    await AppShell.DisplayToastAsync("Archivo seleccionado: " + result.FileName);
                    await vm.ImportarDesdeCsv(result.FullPath);
                }
            }
            catch (Exception ex)
            {
                await AppShell.DisplaySnackbarAsync($"Error al seleccionar el archivo: {ex.Message}");
            }
        }
    }
}

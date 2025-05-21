using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Japanese_App.Pages;
using Microsoft.Maui.Graphics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Japanese_App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Establece el índice del tema según el tema actual de la app
            var currentTheme = Application.Current!.UserAppTheme;
            ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;

            // Registrar rutas para navegación
            Routing.RegisterRoute("nuevaActividadPage", typeof(NuevaActividadPage));
            Routing.RegisterRoute(nameof(QuizPage), typeof(QuizPage));

        }

        // Cambia el tema cuando se selecciona un índice en el control segmentado
        private void SfSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
        }

        // Mostrar un Snackbar con opciones personalizadas
        public static async Task DisplaySnackbarAsync(string message)
        {
            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.Red,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(8),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(12)
            };

            using var cts = new CancellationTokenSource();

            var snackbar = Snackbar.Make(message, null, "OK", TimeSpan.FromSeconds(3), snackbarOptions);
            await snackbar.Show(cts.Token);
        }

        // Mostrar un Toast (excepto en Windows)
        public static async Task DisplayToastAsync(string message)
        {
            try
            {
                var toast = Toast.Make(message, textSize: 16);
                await toast.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error mostrando Toast: " + ex.Message);
            }
        }
    }
}

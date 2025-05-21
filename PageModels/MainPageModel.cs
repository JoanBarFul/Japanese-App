using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Japanese_App.Models;
using Japanese_App.Data;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Japanese_App.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private bool _isNavigatedTo;
        private bool _dataLoaded;

        private readonly ProjectRepository _projectRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly ModalErrorHandler _errorHandler;
        private readonly SeedDataService _seedDataService;
        private readonly DatabaseService _dbService;

        public MainPageModel(
            SeedDataService seedDataService,
            ProjectRepository projectRepository,
            CategoryRepository categoryRepository,
            ModalErrorHandler errorHandler,
            DatabaseService dbService)
        {
            _projectRepository = projectRepository;
            _categoryRepository = categoryRepository;
            _errorHandler = errorHandler;
            _seedDataService = seedDataService;
            _dbService = dbService;

            Today = DateTime.Now.ToString("dddd, MMM d");
        }

        [ObservableProperty]
        private List<CategoryChartData> _todoCategoryData = new();

        [ObservableProperty]
        private List<Brush> _todoCategoryColors = new();

        [ObservableProperty]
        private List<Project> _projects = new();

        [ObservableProperty]
        private List<Actividad> _actividades = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _today;

        private async Task LoadData()
        {
            try
            {
                IsBusy = true;

                Projects = await _projectRepository.ListAsync();

                var chartData = new List<CategoryChartData>();

                var categories = await _categoryRepository.ListAsync();
                foreach (var category in categories)
                {
                    var ps = Projects.Where(p => p.CategoryID == category.ID).ToList();
                    int tasksCount = ps.SelectMany(p => p.Tasks).Count();
                    chartData.Add(new(category.Title, tasksCount));
                }

                // Quitar el último círculo (último elemento)
                if (chartData.Count > 0)
                    chartData.RemoveAt(chartData.Count - 1);

                // Asignar colores rojo, amarillo y verde en orden
                var customColors = new List<Brush> { Colors.Red, Colors.Yellow, Colors.Green };
                var chartColors = new List<Brush>();

                for (int i = 0; i < chartData.Count; i++)
                {
                    chartColors.Add(customColors[i % customColors.Count]);
                }

                TodoCategoryData = chartData;
                TodoCategoryColors = chartColors;

                // Comprobar si hay actividades en la base de datos
                int totalActividades = await _dbService.ContarActividadesAsync();

                if (totalActividades == 0)
                {
                    // Insertar actividad de prueba si no hay datos
                    await _dbService.InsertarActividadDePruebaAsync();
                }

                // Cargar las actividades para actualizar UI
                Actividades = await _dbService.ObtenerActividadesAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task InitData(SeedDataService seedDataService)
        {
            bool isSeeded = Preferences.Default.ContainsKey("is_seeded");

            if (!isSeeded)
            {
                await seedDataService.LoadSeedDataAsync();
                Preferences.Default.Set("is_seeded", true);
            }

            await Refresh();
        }

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                await LoadData();
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private void NavigatedTo() => _isNavigatedTo = true;

        [RelayCommand]
        private void NavigatedFrom() => _isNavigatedTo = false;

        [RelayCommand]
        private async Task Appearing()
        {
            if (!_dataLoaded)
            {
                await InitData(_seedDataService);
                _dataLoaded = true;
            }
            else if (!_isNavigatedTo)
            {
                await Refresh();
            }
        }

        [RelayCommand]
        private async Task AddActividad()
        {
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync("nuevaActividadPage");
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private Task NavigateToProject(Project project) => Shell.Current.GoToAsync($"project?id={project.ID}");

        [RelayCommand]
        private async Task ActividadTapped(Actividad actividad)
        {
            await Shell.Current.GoToAsync(nameof(QuizPage));
        }

        // Comando para empezar un quiz aleatorio
        [RelayCommand]
        private async Task RandomQuiz()
        {
            try
            {
                if (Actividades == null || Actividades.Count == 0)
                    return;

                var random = new Random();
                int index = random.Next(Actividades.Count);
                var actividadAleatoria = Actividades[index];


                await Shell.Current.GoToAsync(nameof(QuizPage));
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
        }
    }
}

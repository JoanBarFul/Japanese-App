using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Japanese_App.PageModels
{
    public class Pregunta
    {
        public string PreguntaTexto { get; set; }
        public string RespuestaCorrecta { get; set; }
        public List<string> Opciones { get; set; }
    }

    public class QuizPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<Pregunta> preguntas;
        private int currentIndex = 0;

        private Pregunta currentQuestion;
        public Pregunta CurrentQuestion
        {
            get => currentQuestion;
            set
            {
                currentQuestion = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentQuestionText));
            }
        }

        private ObservableCollection<string> currentOptions = new();
        public ObservableCollection<string> CurrentOptions
        {
            get => currentOptions;
            set
            {
                currentOptions = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentAnswers));
            }
        }

        // Propiedades para binding más directo en XAML
        public string CurrentQuestionText => CurrentQuestion?.PreguntaTexto ?? string.Empty;
        public ObservableCollection<string> CurrentAnswers => CurrentOptions;

        private string feedbackMessage;
        public string FeedbackMessage
        {
            get => feedbackMessage;
            set { feedbackMessage = value; OnPropertyChanged(); }
        }

        private bool isFeedbackVisible;
        public bool IsFeedbackVisible
        {
            get => isFeedbackVisible;
            set { isFeedbackVisible = value; OnPropertyChanged(); }
        }

        private bool isNextEnabled;
        public bool IsNextEnabled
        {
            get => isNextEnabled;
            set { isNextEnabled = value; OnPropertyChanged(); }
        }

        public ICommand SelectOptionCommand { get; }
        public ICommand NextQuestionCommand { get; }

        public QuizPageModel()
        {
            SelectOptionCommand = new Command<string>(SelectOption);
            NextQuestionCommand = new Command(NextQuestion);

            LoadPreguntasFromCSV();

            ShowQuestion(0);
        }

        private void LoadPreguntasFromCSV()
        {
            string csv = @"Cual es la traducción de ありがとう?;Gracias;Hola;Adiós;Por favor
¿Qué significa 猫 (ねこ)?;Gato;Perro;Pájaro;Ratón
¿Cómo se dice ""gracias"" en japonés?;ありがとう;こんにちは;さようなら;すみません
¿Cual es la traducción de 水 (みず)?;Agua;Fuego;Tierra;Viento";

            preguntas = new List<Pregunta>();

            var lines = csv.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length < 2) continue;

                var pregunta = parts[0];
                var correct = parts[1];
                var opciones = parts.Skip(1).ToList();

                preguntas.Add(new Pregunta
                {
                    PreguntaTexto = pregunta,
                    RespuestaCorrecta = correct,
                    Opciones = opciones
                });
            }
        }

        private void ShowQuestion(int index)
        {
            if (index < 0 || index >= preguntas.Count) return;

            currentIndex = index;
            CurrentQuestion = preguntas[index];

            var rnd = new System.Random();
            var shuffled = CurrentQuestion.Opciones.OrderBy(x => rnd.Next()).ToList();

            CurrentOptions = new ObservableCollection<string>(shuffled);

            FeedbackMessage = string.Empty;
            IsFeedbackVisible = false;
            IsNextEnabled = false;
        }

        private void SelectOption(string selectedOption)
        {
            if (IsFeedbackVisible) return;

            bool correcto = selectedOption == CurrentQuestion.RespuestaCorrecta;
            FeedbackMessage = correcto ? "¡Correcto! ✅" : $"Incorrecto ❌. La respuesta correcta es: {CurrentQuestion.RespuestaCorrecta}";

            IsFeedbackVisible = true;
            IsNextEnabled = true;
        }

        private void NextQuestion()
        {
            if (currentIndex + 1 < preguntas.Count)
            {
                ShowQuestion(currentIndex + 1);
            }
            else
            {
                FeedbackMessage = "¡Has completado el quiz!";
                IsNextEnabled = false;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

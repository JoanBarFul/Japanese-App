using Japanese_App.PageModels;

namespace Japanese_App.Pages;

public partial class QuizPage : ContentPage
{
    public QuizPage()
    {
        InitializeComponent();
        BindingContext = new QuizPageModel();
    }
}

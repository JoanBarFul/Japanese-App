using CommunityToolkit.Mvvm.Input;
using Japanese_App.Models;

namespace Japanese_App.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}
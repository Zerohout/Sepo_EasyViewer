// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Caliburn.Micro;

    public partial class FilmsEditingViewModel : Screen
	{

        private void NotifyChanges()
        {
            NotifyOfPropertyChange(() => CanSaveChanges);
            NotifyOfPropertyChange(() => CanCancelChanges);
        }

	}
}

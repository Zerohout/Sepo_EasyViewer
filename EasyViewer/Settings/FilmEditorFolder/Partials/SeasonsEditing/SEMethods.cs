// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Caliburn.Micro;

    public partial class SeasonsEditingViewModel : Screen
    {
		/// <summary>
		/// Оповестить кнопки редактирования
		/// </summary>
	    private void NotifyEditingButtons()
	    {
			NotifyOfPropertyChange(() => CanSelectEpisode);
			NotifyOfPropertyChange(() => CanEditEpisode);
			NotifyOfPropertyChange(() => CanRemoveEpisode);
			NotifyOfPropertyChange(() => CanCancelSelection);
	    }
		/// <summary>
		/// Оповестить кнопки связанные с изменением данных
		/// </summary>
	    private void NotifyChanges()
	    {
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
	    }
    }
}
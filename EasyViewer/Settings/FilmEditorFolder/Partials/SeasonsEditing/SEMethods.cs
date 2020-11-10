// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Caliburn.Micro;
    using Models.FilmModels;

    public partial class SeasonsEditingViewModel : Screen
    {
		/// <summary>
		/// Оповестить кнопки редактирования
		/// </summary>
	    private void NotifyEditingButtons()
	    {
			NotifyOfPropertyChange(() => CanSelectEpisode);
			NotifyOfPropertyChange(() => CanEditEpisode);
			NotifyOfPropertyChange(() => CanDeleteSelectedEpisodes);
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
// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Caliburn.Micro;

    public partial class FilmsEditingViewModel : Screen
	{
		/// <summary>
		/// Оповестить кнопки зависящие от изменений
		/// </summary>
        private void NotifyChanges()
        {
            NotifyOfPropertyChange(() => CanSaveChanges);
            NotifyOfPropertyChange(() => CanCancelChanges);
        }
		/// <summary>
		/// Оповестить кнопки редактирования сезонов
		/// </summary>
		private void NotifyEditingButtons()
		{
			NotifyOfPropertyChange(() => CanSelectSeason);
			NotifyOfPropertyChange(() => CanEditSeason);
			NotifyOfPropertyChange(() => CanRemoveSeason);
			NotifyOfPropertyChange(() => CanCancelSelection);
		}

	}
}

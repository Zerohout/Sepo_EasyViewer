// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using Caliburn.Micro;

	public partial class EpisodeEditingViewModel : Screen
	{
		/// <summary>
		/// Оповестить кнопки зависящие от изменений
		/// </summary>
		private void NotifyChanges()
		{
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
			EEVM.IsInterfaceUnlock = !HasChanges;
		}
	}
}

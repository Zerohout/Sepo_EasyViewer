// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Caliburn.Micro;
    using Models.FilmModels;

    //using Database;
	
    public partial class EpisodesEditorViewModel : Conductor<Screen>.Collection.OneActive
	{
		/// <summary>
		/// Оповестить об изменениях кнопки навигации
		/// </summary>
		private void NotifyNavigatingButtons()
		{
			NotifyOfPropertyChange(() => CanEditPrevEpisode);
			NotifyOfPropertyChange(() => CanEditNextEpisode);
			NotifyOfPropertyChange(() => CanEditPrevAddress);
			NotifyOfPropertyChange(() => CanEditNextAddress);
		}



		/// <summary>
		/// Сменить активную модель представления
		/// </summary>
		/// <param name="vm">Модель представления</param>
		/// <param name="episode">Эпизод (по умолч. null)</param>
		public void ChangeActiveItem(Screen vm, Episode episode = null)
		{
			if (vm is EpisodeEditingViewModel)
			{
				if (episode == null) return;
				ESVM.ResetSelectedEpisode(episode);
			}
			else
			{
                if (ActiveItem != null && ActiveItem.IsActive) ActiveItem.TryClose();
				ActiveItem = vm;
			}
		}

		
    }
}

// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.ViewingsSettingsFolder.ViewModels
{
    using System.Linq;
    using Caliburn.Micro;
    using Models.FilmModels;
    using static Helpers.DbMethods;

    public partial class ViewingsSettingsViewModel : Screen
    {

        private void NotifyFilmData()
        {
            NotifyOfPropertyChange(() => SelectedFilm);
            NotifyOfPropertyChange(() => SelectedFilmVisibility);
            NotifyOfPropertyChange(() => CanCancelFilmSelection);
        }

        private void NotifySeasonData()
        {
            NotifyOfPropertyChange(() => SelectedSeason);
            NotifyOfPropertyChange(() => SelectedSeasonVisibility);
            NotifyOfPropertyChange(() => CanCancelSeasonSelection);
        }

        private void NotifyEpisodeData()
        {
            NotifyOfPropertyChange(() => SelectedEpisode);
            NotifyOfPropertyChange(() => SelectedEpisodeVisibility);
            NotifyOfPropertyChange(() => CanCancelEpisodeSelection);
        }
    }
}

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using System.Linq;
	using Caliburn.Micro;
    using Models.FilmModels;
    using Newtonsoft.Json;

    public partial class SeasonsEditingViewModel : Screen
    {
        public SeasonsEditingViewModel()
        {

        }

        public SeasonsEditingViewModel(Season season)
        {
            CurrentSeason = season;
            SelectedEpisode = Episodes?.FirstOrDefault();
            SeasonDescription = season.Description;
            NotifyOfPropertyChange(() => CanSelectEpisode);

        }
    }
}

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
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
            SeasonSnapshot = JsonConvert.SerializeObject(season);
            NotifyOfPropertyChange(() => CanSelectEpisode);

        }
    }
}

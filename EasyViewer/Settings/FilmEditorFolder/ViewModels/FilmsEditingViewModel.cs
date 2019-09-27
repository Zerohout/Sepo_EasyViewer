namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Caliburn.Micro;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using static Helpers.SystemVariables;

    public partial class FilmsEditingViewModel : Screen
    {

        public FilmsEditingViewModel()
        {

        }

        public FilmsEditingViewModel(Film film)
        {
            CurrentFilm = film;
            FilmSnapshot = JsonConvert.SerializeObject(film);

            NotifyOfPropertyChange(() => CreateFilmVisibility);
            NotifyOfPropertyChange(() => SaveChangesVisibility);

            if (film.Name != NewFilmName)
            {
                NotifyOfPropertyChange(() => CanSelectSeason);
                //NotifyOfPropertyChange(() => SaveChangesVisibility);
            }
        }

        protected override void OnInitialize()
        {
            DisplayName = "Редактор фильмов";
            base.OnInitialize();
        }
    }
}

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System.Linq;
    using Caliburn.Micro;
    using Helpers;
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
            

            if (film.Id != 0)
            {
                OriginalFilm = DbMethods.GetFilmFromDbById(film.Id);
                NotifyOfPropertyChange(() => CanSelectSeason);
            }
            Seasons = film.Id != 0
                ? new BindableCollection<Season>(OriginalFilm.Seasons)
                : new BindableCollection<Season>();
            SelectedSeason = Seasons.FirstOrDefault();

            NotifyOfPropertyChange(() => CreateFilmVisibility);
            NotifyOfPropertyChange(() => SaveChangesVisibility);
        }

        protected override void OnInitialize()
        {
            DisplayName = "Редактор фильмов";
            base.OnInitialize();
        }
    }
}
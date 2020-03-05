// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System.Linq;
    using Caliburn.Micro;
    using Helpers;
    using Models.FilmModels;
    using static Helpers.SystemVariables;

    public partial class EditorSelectorViewModel : Conductor<Screen>.Collection.OneActive
    {

        #region Фильмы

        /// <summary>
        /// Установить значение фильмов
        /// </summary>
        /// <param name="value">Тип фильма</param>
        public void SetFilms(FilmType? value)
        {
            Films = value == null
                ? new BindableCollection<Film>()
                : new BindableCollection<Film>(GlobalMethods.GetDbCollection<Film>()
                                                            .FindAll(s => s.FilmType == value))
                {
                    new Film { Name = NewFilmName, FilmType = SelectedFilmType ?? FilmType.Видео}
                };
            SelectedFilm = null;
            NotifyOfPropertyChange(() => Films);
            NotifyOfPropertyChange(() => CanCancelFilmTypeSelection);
            NotifyOfPropertyChange(() => FilmsVisibility);
        }

        /// <summary>
        /// Установить модель представления фильма
        /// </summary>
        /// <param name="value">Текущий фильм</param>
        private void SetFilmVM(Film value)
        {
            FEVM = value != null
                ? new FilmsEditingViewModel(value) { Parent = this }
                : null;

            ChangeActiveItem(FEVM);
        }

        /// <summary>
        /// Переустановить выбранный фильм
        /// </summary>
        /// <param name="film">Новое значение фильма</param>
        /// <param name="isTypeChanging">Изменился ли тип фильма</param>
        public void ResetSelectedFilm(Film film, bool isTypeChanging)
        {
            SelectedFilmType = isTypeChanging is true
                ? film.FilmType
                : SelectedFilmType;

            SelectedFilm = Films.First(f => f.Name == film.Name);
        }

        #endregion

        #region Сезоны

        /// <summary>
        /// Установить значение сезонов
        /// </summary>
        /// <param name="value">Текущий фильм</param>
        private void SetSeasons(Film value)
        {
            Seasons = value == null
                ? new BindableCollection<Season>()
                : value.Name == NewFilmName
                    ? new BindableCollection<Season>()
                    : new BindableCollection<Season>(value.Seasons);
            SelectedSeason = null;

            NotifyOfPropertyChange(() => Seasons);
            NotifyOfPropertyChange(() => CanCancelFilmSelection);
            NotifyOfPropertyChange(() => SeasonsVisibility);
        }

        /// <summary>
        /// Установить модель представления сезона
        /// </summary>
        /// <param name="value">Текущий сезон</param>
        private void SetSeasonVM(Season value)
        {
            SEVM = value != null
                ? new SeasonsEditingViewModel(value) { Parent = this }
                : null;

            if (value == null)
            {
                ChangeActiveItem(SelectedFilm == null
                                     ? (Screen)SEVM
                                     : FEVM);
            }
            else
            {
                ChangeActiveItem(SEVM);
            }
        }

        /// <summary>
        /// Переустановить выбранный сезон
        /// </summary>
        /// <param name="season">Новое значение сезона</param>
        public void ResetSelectedSeason(Season season)
        {
            SetSeasons(SelectedFilm);
            SelectedSeason = Seasons.First(s => s.Number == season.Number);
        }

        #endregion

        #region Эпизоды

        /// <summary>
        /// Установить значения эпизодов
        /// </summary>
        /// <param name="value">Текущий сезон</param>
        private void SetEpisodes(Season value)
        {
            Episodes = value == null
                ? new BindableCollection<Episode>()
                : new BindableCollection<Episode>(value.Episodes);

            SelectedEpisode = null;

            NotifyOfPropertyChange(() => Episodes);
            NotifyOfPropertyChange(() => CanCancelSeasonSelection);
            NotifyOfPropertyChange(() => EpisodesVisibility);
        }

        /// <summary>
        /// Установить модель представления эпизода
        /// </summary>
        /// <param name="value">Текущий эпизод</param>
        private void SetEpisodeVM(Episode value)
        {
            EEVM = value != null
                ? new EpisodesEditorViewModel(SelectedEpisode) { Parent = this }
                : null;

            if (value == null)
            {
                ChangeActiveItem(SelectedSeason == null
                                     ? (Screen)EEVM
                                     : SEVM);
            }
            else
            {
                ChangeActiveItem(EEVM);
            }
        }

        /// <summary>
        /// Переустановить выбранный эпизод
        /// </summary>
        /// <param name="episode">Новое значение эпизода</param>
        public void ResetSelectedEpisode(Episode episode)
        {
            SetEpisodes(SelectedSeason);
            SelectedEpisode = Episodes.First(e => e.FullNumber == episode.FullNumber);
        }

        #endregion

        #region Общие

        /// <summary>
        /// Сменить активную VM
        /// </summary>
        /// <param name="viewModel">Текущая модель представления</param>
        /// <returns></returns>
        private void ChangeActiveItem(Screen viewModel)
        {
            ActiveItem?.TryClose();

            if (viewModel == null) return;
            ActiveItem = viewModel;
        }

        #endregion

    }
}

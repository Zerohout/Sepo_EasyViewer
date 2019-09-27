// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Caliburn.Micro;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using static Helpers.SystemVariables;
    using static Helpers.GlobalMethods;

    public partial class FilmsEditingViewModel : Screen
    {
        private Film _currentFilm;
        private string _filmSnapshot;
        private Season _selectedSeason;

        #region Свойства фильмов

        /// <summary>
        /// Список типов фильма
        /// </summary>
        public BindableCollection<FilmType> FilmTypes =>
            new BindableCollection<FilmType>(Enum.GetValues(typeof(FilmType)).Cast<FilmType>());

        public BindableCollection<Season> Seasons => new BindableCollection<Season>(CurrentFilm.Seasons);

        /// <summary>
        /// Текущий фильм
        /// </summary>
        public Film CurrentFilm
        {
            get => _currentFilm;
            set
            {
                _currentFilm = value;
                NotifyOfPropertyChange(() => CurrentFilm);
            }
        }

        /// <summary>
        /// Экземпляр фильма для отслеживания изменений
        /// </summary>

        public string FilmSnapshot
        {
            get => _filmSnapshot;
            set
            {
                _filmSnapshot = value;
                NotifyOfPropertyChange(() => FilmSnapshot);
            }
        }

        #endregion

        #region Свойства сезонов

        /// <summary>
        /// Выбранный сезон
        /// </summary>
        public Season SelectedSeason
        {
            get => _selectedSeason;
            set
            {
                _selectedSeason = value;
                NotifyOfPropertyChange(() => SelectedSeason);
                NotifyOfPropertyChange(() => CanEditSeason);
                NotifyOfPropertyChange(() => CanCancelSelection);
                NotifyOfPropertyChange(() => CanSelectSeason);
            }
        }

        #endregion

        #region Общие свойства

        /// <summary>
        /// Модель представления селектора редактора
        /// </summary>
        private EditorSelectorViewModel ESVM => Parent as EditorSelectorViewModel;

        /// <summary>
        /// Изображение фильма
        /// </summary>
        public BitmapImage Logo => LoadImage(CurrentFilm.ImageBytes?.ToArray());

        /// <summary>
        /// Флаг наличия изменений
        /// </summary>
        public bool HasChanges => IsEquals(CurrentFilm, JsonConvert.DeserializeObject<Film>(FilmSnapshot)) is false;

        /// <summary>
        /// Свойство Visibility контролов создания
        /// </summary>
        public Visibility CreateFilmVisibility => CurrentFilm.Name == NewFilmName
            ? Visibility.Visible
            : Visibility.Collapsed;

        /// <summary>
        /// Свойство Visibility контролов редактирования
        /// </summary>
        public Visibility SaveChangesVisibility => CurrentFilm.Name != NewFilmName
            ? Visibility.Visible
            : Visibility.Collapsed;

        #endregion

    }
}

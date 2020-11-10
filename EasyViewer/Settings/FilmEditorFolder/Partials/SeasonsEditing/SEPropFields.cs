// ReSharper disable once CheckNamespace

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System.Linq;
    using Caliburn.Micro;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using static Helpers.GlobalMethods;

    public partial class SeasonsEditingViewModel : Screen
    {
        private Season _currentSeason;
        private Episode _selectedEpisode;
        private string _seasonDescription;
        private int? _addingEpisodeValue = 1;
        private BindableCollection<Episode> _episodes;
        private BindableCollection<Episode> _selectedEpisodes = new BindableCollection<Episode>();

        #region Свойства сезонов

        /// <summary>
        /// Текущий сезон
        /// </summary>
        public Season CurrentSeason
        {
            get => _currentSeason;
            set
            {
                _currentSeason = value;
                NotifyOfPropertyChange(() => CurrentSeason);
            }
        }

        /// <summary>
        /// Экземпляр сезона для отслеживания изменений
        /// </summary>
        public string SeasonDescription
        {
            get => _seasonDescription;
            set
            {
                _seasonDescription = value;
                NotifyOfPropertyChange(() => SeasonDescription);
            }
        }

        #endregion

        #region Свойства эпизодов

        /// <summary>
        /// Количество создаваемых эпизодов по умолчанию за раз
        /// </summary>
        public int? AddingEpisodeValue
        {
            get => _addingEpisodeValue;
            set
            {
                if (value == null || value < 1) value = 1;
                if (value > 99) value = 99;

                _addingEpisodeValue = value;
                NotifyOfPropertyChange(() => AddingEpisodeValue);
            }
        }

        /// <summary>
        /// Список эпизодов текущего сезона
        /// </summary>
        public BindableCollection<Episode> Episodes
        {
            get => _episodes;
            set
            {
                _episodes = new BindableCollection<Episode>(value.OrderBy(e => e.Number));
                NotifyOfPropertyChange(() => Episodes);
            }
        }

        public BindableCollection<Episode> SelectedEpisodes
        {
            get => _selectedEpisodes;
            set
            {
                _selectedEpisodes = value;
                NotifyOfPropertyChange(() => SelectedEpisodes);
                NotifyOfPropertyChange(() => CanDeleteSelectedEpisodes);
                NotifyOfPropertyChange(() => CanEditEpisode);
                NotifyOfPropertyChange(() => CanCancelSelection);
                NotifyOfPropertyChange(() => CanSelectEpisode);
            }
        }

        /// <summary>
        /// Выбранный эпизод
        /// </summary>
        public Episode SelectedEpisode
        {
            get => _selectedEpisode;
            set
            {
                _selectedEpisode = value;
                NotifyOfPropertyChange(() => SelectedEpisode);
            }
        }

        #endregion

        #region Общие свойства

        /// <summary>
        /// Модель представления селектора редактора
        /// </summary>
        private EditorSelectorViewModel ESVM => Parent as EditorSelectorViewModel;

        /// <summary>
        /// Флаг наличия изменений
        /// </summary>
        public bool HasChanges =>
            string.IsNullOrWhiteSpace(SeasonDescription) is false && CurrentSeason.Description != SeasonDescription;

        #endregion
    }
}
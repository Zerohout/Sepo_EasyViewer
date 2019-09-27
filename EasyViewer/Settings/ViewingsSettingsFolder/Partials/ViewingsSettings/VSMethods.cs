// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.ViewingsSettingsFolder.ViewModels
{
    using Caliburn.Micro;
    using Models.FilmModels;

    public partial class ViewingsSettingsViewModel : Screen
    {

        /// <summary>
        /// Загрузка данных (начальная и при смене значений объектов)
        /// </summary>
        private void LoadData()
        {
            if (_selectedFilm == null)
            {
                LoadFilmData();
                return;
            }

            if (_selectedSeason == null)
            {
                LoadSeasonData();
                return;
            }

            if (_selectedEpisode == null)
            {
                LoadEpisodeData();
                return;
            }

            //if(_selectedVoiceOver != null)
            //	return;

            LoadVoiceOverData();

        }

        /// <summary>
        /// Загрузка фильма и связанных с ним данных
        /// </summary>
        private void LoadFilmData()
        {
            if (IdList.FilmId == 0)
            {
                // Начальная загрузка списка фильмов при загрузке VM
                LoadFilmList();
                return;
            }

            // Установка значения выбранного фильма с null на notNull
            //_selectedFilm = _films.First(c => c.FilmId == IdList.FilmId);
            NotifyFilmData();

            // Загрузка списка сезонов выбранного фильма
            LoadSeasonList();
        }

        /// <summary>
        /// Загрузка сезона и связанных с нима данных
        /// </summary>
        private void LoadSeasonData()
        {
            //if(_selectedFilm.FilmId != IdList.FilmId)
            //{
            //	// Смена значения фильма с notNull на notNull
            //	// при условии, что оно не равно текущему
            //	_selectedFilm = _films.First(c => c.FilmId == IdList.FilmId);
            //	NotifyFilmData();
            //}

            if (IdList.SeasonId == 0)
            {
                // Загрузка списка сезонов выбранного фильма
                LoadSeasonList();
                return;
            }

            // Установка значения выбранного сезона с null на notNull

            //_selectedSeason = _seasons.First(s => s.SeasonId == IdList.SeasonId);

            NotifySeasonData();

            LoadEpisodeList();
        }

        /// <summary>
        /// Загрузка эпизода и связанных с ним данных
        /// </summary>
        private void LoadEpisodeData()
        {
            //if(_selectedSeason.SeasonId != IdList.SeasonId)
            //{
            //	// Смена значения сезона с notNull на notNull
            //	// при условии, что оно не равно текущему
            //	_selectedSeason = _seasons.First(s => s.SeasonId == IdList.SeasonId);

            //	NotifySeasonData();
            //}

            if (IdList.EpisodeId == 0)
            {
                // Загрузка списка эпизодов выбранного сезона
                LoadEpisodeList();
                return;
            }

            // Установка значения выбранного эпизода с null на notNull
            //_selectedEpisode = _episodes.First(e => e.EpisodeId == IdList.EpisodeId);
            NotifyEpisodeData();

            LoadEpisodeVoiceOverList();
        }

        private void LoadVoiceOverData()
        {
            //if(_selectedEpisode.EpisodeId != IdList.EpisodeId)
            //{
            //	// Смена значения эпизода с notNull на notNull
            //	// при условии, что оно не равно текущему
            //	_selectedEpisode = _episodes.First(e => e.EpisodeId == IdList.EpisodeId);
            //	NotifyEpisodeData();
            //}

            if (IdList.VoiceOverId == 0)
            {
                // Загрузка списка озвучек выбранного эпизода
                LoadEpisodeVoiceOverList();
                return;
            }

            // Установка значения выбранной озвучки с null на notNull
            //_selectedVoiceOver = _voiceOvers.First(vo => vo.FilmVoiceOverId == IdList.VoiceOverId);
            NotifyVoiceOverData();
        }

        /// <summary>
        /// Загрузка списка фильмов из БД
        /// </summary>
        public void LoadFilmList()
        {
            //Films = new BindableCollection<Film>(CvDbContext.Films.ToList());
        }

        /// <summary>
        /// Загрузка списка сезонов выбранного фильма из БД
        /// </summary>
        public void LoadSeasonList()
        {
            //Seasons = new BindableCollection<Season>(
            //	CvDbContext.Seasons
            //	                 .Where(cs => cs.FilmId == IdList.FilmId)
            //				 .ToList());
        }

        /// <summary>
        /// Загрузка списка эпизодов выбранного сезона из БД
        /// </summary>
        public void LoadEpisodeList()
        {
            //Episodes = new BindableCollection<Episode>(
            //	CvDbContext.Episodes
            //	         .Where(ce => ce.SeasonId == IdList.SeasonId)
            //	         .ToList());
        }

        /// <summary>
        /// Загрузка списка озвучек выбранного эпизода из БД
        /// </summary>
        public void LoadEpisodeVoiceOverList()
        {
            //var voiceOvers = new BindableCollection<FilmVoiceOver>(
            //	CvDbContext.VoiceOvers
            //	           .Include(vo => vo.Episodes)
            //	           .Include(vo => vo.CheckedEpisodes)
            //	           .Where(vo => vo.Episodes
            //	                          .Any(ce => ce.EpisodeId == IdList.EpisodeId))
            //	           .ToList());

            //var totalCount = voiceOvers.Count;
            //var count = 0;

            //while(count < totalCount)
            //{
            //	voiceOvers[count++].SelectedEpisodeId = IdList.EpisodeId;
            //}

            //VoiceOvers = new BindableCollection<FilmVoiceOver>(voiceOvers);
        }

        /// <summary>
        /// Изменить выбранный м/ф и все связанные данные
        /// </summary>
        /// <param name="value">Конечное значение м/ф</param>
        private void ChangeSelectedFilm(Film value)
        {
            if (IsDesignTime)
            {
                _selectedFilm = value;
                NotifyOfPropertyChange(() => SelectedFilm);
                return;
            }

            if (_selectedFilm == value)
                return;

            //IdList.FilmId = value?.FilmId ?? 0;
            ChangeSelectedSeason(null);


            if (value == null)
            {
                _selectedFilm = null;
                NotifyFilmData();
            }
            else
            {

                LoadData();

            }
        }

        /// <summary>
        /// Изменить выбранный сезон и все связные данные
        /// </summary>
        /// <param name="value">Конечное значение сезона</param>
        private void ChangeSelectedSeason(Season value)
        {
            if (IsDesignTime)
            {
                _selectedSeason = value;
                NotifyOfPropertyChange(() => SelectedSeason);
                return;
            }

            if (_selectedSeason == value)
                return;

            //IdList.SeasonId = value?.SeasonId ?? 0;
            ChangeSelectedEpisode(null);

            if (value == null)
            {
                _selectedSeason = null;
                NotifySeasonData();
            }
            else
            {
                LoadData();
            }
        }

        /// <summary>
        /// Изменить выбранный эпизод и все связанные данные
        /// </summary>
        /// <param name="value">Конечное значение эпизода</param>
        private void ChangeSelectedEpisode(Episode value)
        {
            if (IsDesignTime)
            {
                _selectedEpisode = value;
                NotifyOfPropertyChange(() => SelectedEpisode);
                return;
            }

            if (_selectedEpisode == value)
                return;

            //IdList.EpisodeId = value?.EpisodeId ?? 0;
            //ChangeSelectedVoiceOver(null);

            if (value == null)
            {
                _selectedEpisode = null;
                EpisodeIndexes.CurrentIndex = -1;
                NotifyEpisodeData();
            }
            else
            {
                EpisodeIndexes.CurrentIndex = Episodes.IndexOf(value);
                LoadData();
            }

        }

        private void ChangeSelectedVoiceOver()
        {
            //if(IsDesignTime)
            //{
            //	_selectedVoiceOver = value;
            //	NotifyOfPropertyChange(() => SelectedEpisode);
            //	return;
            //}

            //if(_selectedVoiceOver == value)
            //	return;

            //IdList.VoiceOverId = value?.FilmVoiceOverId ?? 0;

            //if(value == null)
            //{
            //	_selectedVoiceOver = null;
            //	NotifyVoiceOverData();
            //}
            //else
            //{
            //	LoadData();
            //}
        }

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

        private void NotifyVoiceOverData()
        {
            //NotifyOfPropertyChange(() => SelectedVoiceOver);
            //NotifyOfPropertyChange(() => CanCancelVoiceOverSelection);
        }
    }
}

// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.ViewingsSettingsFolder.ViewModels
{
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Models.SerialModels;

    public partial class ViewingsSettingsViewModel : Screen
	{

		/// <summary>
		/// Загрузка данных (начальная и при смене значений объектов)
		/// </summary>
		private async void LoadData()
		{
			if(_selectedSerial == null)
			{
				await LoadSerialData();
				return;
			}

			if(_selectedSeason == null)
			{
				await LoadSeasonData();
				return;
			}

			if(_selectedEpisode == null)
			{
				await LoadEpisodeData();
				return;
			}

			if(_selectedVoiceOver != null)
				return;

			await LoadVoiceOverData();

		}

		/// <summary>
		/// Загрузка м/с и связанных с ним данных
		/// </summary>
		private Task LoadSerialData()
		{
			if(IdList.SerialId == 0)
			{
				// Начальная загрузка списка м/с при загрузке VM
				LoadSerialList();
				return Task.CompletedTask;
			}

			// Установка значения выбранного м/с с null на notNull
			_selectedSerial = _Serials.First(c => c.SerialId == IdList.SerialId);
			NotifySerialData();

			// Загрузка списка сезонов выбранного м/с
			LoadSeasonList();
			return Task.CompletedTask;
		}

		/// <summary>
		/// Загрузка сезона и связанных с нима данных
		/// </summary>
		private Task LoadSeasonData()
		{
			if(_selectedSerial.SerialId != IdList.SerialId)
			{
				// Смена значения м/с с notNull на notNull
				// при условии, что оно не равно текущему
				_selectedSerial = _Serials.First(c => c.SerialId == IdList.SerialId);
				NotifySerialData();
			}

			if(IdList.SeasonId == 0)
			{
				// Загрузка списка сезонов выбранного м/с
				LoadSeasonList();
				return Task.CompletedTask;
			}

			// Установка значения выбранного сезона с null на notNull

			_selectedSeason = _seasons.First(s => s.SeasonId == IdList.SeasonId);

			NotifySeasonData();

			LoadEpisodeList();

			return Task.CompletedTask;
		}

		/// <summary>
		/// Загрузка эпизода и связанных с ним данных
		/// </summary>
		private Task LoadEpisodeData()
		{
			if(_selectedSeason.SeasonId != IdList.SeasonId)
			{
				// Смена значения сезона с notNull на notNull
				// при условии, что оно не равно текущему
				_selectedSeason = _seasons.First(s => s.SeasonId == IdList.SeasonId);

				NotifySeasonData();
			}

			if(IdList.EpisodeId == 0)
			{
				// Загрузка списка эпизодов выбранного сезона
				LoadEpisodeList();
				return Task.CompletedTask;
			}

			// Установка значения выбранного эпизода с null на notNull
			_selectedEpisode = _episodes.First(e => e.EpisodeId == IdList.EpisodeId);
			NotifyEpisodeData();

			LoadEpisodeVoiceOverList();
			return Task.CompletedTask;
		}

		private Task LoadVoiceOverData()
		{
			if(_selectedEpisode.EpisodeId != IdList.EpisodeId)
			{
				// Смена значения эпизода с notNull на notNull
				// при условии, что оно не равно текущему
				_selectedEpisode = _episodes.First(e => e.EpisodeId == IdList.EpisodeId);
				NotifyEpisodeData();
			}

			if(IdList.VoiceOverId == 0)
			{
				// Загрузка списка озвучек выбранного эпизода
				LoadEpisodeVoiceOverList();
				return Task.CompletedTask;
			}

			// Установка значения выбранной озвучки с null на notNull
			_selectedVoiceOver = _voiceOvers.First(vo => vo.SerialVoiceOverId == IdList.VoiceOverId);
			NotifyVoiceOverData();
			return Task.CompletedTask;
		}

		/// <summary>
		/// Загрузка списка м/с из БД
		/// </summary>
		public async void LoadSerialList()
		{
			using(var ctx = new CVDbContext(Helpers.Variables.AppDataPath))
			{
				Serials = new BindableCollection<Serial>(await ctx.Serials.ToListAsync());
			}
		}

		/// <summary>
		/// Загрузка списка сезонов выбранного м/с из БД
		/// </summary>
		public async void LoadSeasonList()
		{
			using(var ctx = new CVDbContext(Helpers.Variables.AppDataPath))
			{
				Seasons = new BindableCollection<Season>(
					await ctx.Seasons
							 .Where(cs => cs.SerialId == IdList.SerialId)
							 .ToListAsync());
			}
		}

		/// <summary>
		/// Загрузка списка эпизодов выбранного сезона из БД
		/// </summary>
		public async void LoadEpisodeList()
		{
			using(var ctx = new CVDbContext(Helpers.Variables.AppDataPath))
			{
				Episodes = new BindableCollection<Episode>(
					await ctx.Episodes
							 .Where(ce => ce.SeasonId == IdList.SeasonId)
							 .ToListAsync());
			}
		}

		/// <summary>
		/// Загрузка списка озвучек выбранного эпизода из БД
		/// </summary>
		public async void LoadEpisodeVoiceOverList()
		{
			BindableCollection<SerialVoiceOver> voiceOvers;

			using(var ctx = new CVDbContext(Helpers.Variables.AppDataPath))
			{
				voiceOvers = new BindableCollection<SerialVoiceOver>(
					await ctx.VoiceOvers
					         .Include(vo => vo.Episodes)
							 .Include(vo => vo.CheckedEpisodes)
							 .Where(vo => vo.Episodes
											.Any(ce => ce.EpisodeId == IdList.EpisodeId))
							 .ToListAsync());
			}

			var totalCount = voiceOvers.Count;
			var count = 0;

			while(count < totalCount)
			{
				voiceOvers[count++].SelectedEpisodeId = IdList.EpisodeId;
			}

			VoiceOvers = new BindableCollection<SerialVoiceOver>(voiceOvers);
		}

		/// <summary>
		/// Изменить выбранный м/ф и все связанные данные
		/// </summary>
		/// <param name="value">Конечное значение м/ф</param>
		private void ChangeSelectedSerial(Serial value)
		{
			if(IsDesignTime)
			{
				_selectedSerial = value;
				NotifyOfPropertyChange(() => SelectedSerial);
				return;
			}

			if(_selectedSerial == value)
				return;

			IdList.SerialId = value?.SerialId ?? 0;
			ChangeSelectedSeason(null);


			if(value == null)
			{
				_selectedSerial = null;
				NotifySerialData();
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
			if(IsDesignTime)
			{
				_selectedSeason = value;
				NotifyOfPropertyChange(() => SelectedSeason);
				return;
			}

			if(_selectedSeason == value)
				return;

			IdList.SeasonId = value?.SeasonId ?? 0;
			ChangeSelectedEpisode(null);

			if(value == null)
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
			if(IsDesignTime)
			{
				_selectedEpisode = value;
				NotifyOfPropertyChange(() => SelectedEpisode);
				return;
			}

			if(_selectedEpisode == value)
				return;

			IdList.EpisodeId = value?.EpisodeId ?? 0;
			ChangeSelectedVoiceOver(null);

			if(value == null)
			{
				_selectedEpisode = null;
				NotifyEpisodeData();
			}
			else
			{
				LoadData();
			}
		}

		private void ChangeSelectedVoiceOver(SerialVoiceOver value)
		{
			if(IsDesignTime)
			{
				_selectedVoiceOver = value;
				NotifyOfPropertyChange(() => SelectedEpisode);
				return;
			}

			if(_selectedVoiceOver == value)
				return;

			IdList.VoiceOverId = value?.SerialVoiceOverId ?? 0;

			if(value == null)
			{
				_selectedVoiceOver = null;
				NotifyVoiceOverData();
			}
			else
			{
				LoadData();
			}
		}

		private void NotifySerialData()
		{
			NotifyOfPropertyChange(() => SelectedSerial);
			NotifyOfPropertyChange(() => SelectedSerialVisibility);
			NotifyOfPropertyChange(() => CanCancelSerialSelection);
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
			NotifyOfPropertyChange(() => SelectedVoiceOver);
			NotifyOfPropertyChange(() => CanCancelVoiceOverSelection);
		}
	}
}

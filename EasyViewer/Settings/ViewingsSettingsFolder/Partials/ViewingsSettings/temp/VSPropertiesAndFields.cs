// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.ViewingsSettingsFolder.ViewModels
{
    using System.Windows;
    using Caliburn.Micro;
    using SerialEditorFolder.ViewModels;
    using Models.SerialModels;

    public partial class ViewingsSettingsViewModel : Screen, ISettingsViewModel
	{
		private BindableCollection<Serial> _Serials = new BindableCollection<Serial>();
		private BindableCollection<Season> _seasons = new BindableCollection<Season>();
		private BindableCollection<Episode> _episodes = new BindableCollection<Episode>();
		private BindableCollection<SerialVoiceOver> _voiceOvers = new BindableCollection<SerialVoiceOver>();

		private Visibility _btnVisibility;

		public Visibility BtnVisibility
		{
			get => _btnVisibility;
			set
			{
				_btnVisibility = value;
				NotifyOfPropertyChange(() => BtnVisibility);
			}
		}



		private bool _isSerialListEnable;
		private bool _isSeasonListEnable;
		private bool _isEpisodeListEnable;

		private Serial _selectedSerial;
		private Season _selectedSeason;
		private Episode _selectedEpisode;
		private SerialVoiceOver _selectedVoiceOver;

		private (int SerialId, int SeasonId, int EpisodeId, int VoiceOverId) IdList = (0, 0, 0, 0);


		#region Flags

		public bool IsDesignTime { get; set; }

		#endregion


		#region Lists content

		/// <summary>
		/// Список м/с
		/// </summary>
		public BindableCollection<Serial> Serials
		{
			get => _Serials;
			set
			{
				_Serials = value;
				NotifyOfPropertyChange(() => Serials);
			}
		}

		/// <summary>
		/// список сезонов выбранного м/с
		/// </summary>
		public BindableCollection<Season> Seasons
		{
			get => _seasons;
			set
			{
				_seasons = value;
				NotifyOfPropertyChange(() => Seasons);
			}
		}

		/// <summary>
		/// Список эпизодов выбранного сезона
		/// </summary>
		public BindableCollection<Episode> Episodes
		{
			get => _episodes;
			set
			{
				_episodes = value;
				NotifyOfPropertyChange(() => Episodes);
			}
		}

		/// <summary>
		/// Список озвучек выбранного эпизода
		/// </summary>
		public BindableCollection<SerialVoiceOver> VoiceOvers
		{
			get => _voiceOvers;
			set
			{
				_voiceOvers = value;
				NotifyOfPropertyChange(() => VoiceOvers);
			}
		}

		/// <summary>
		/// Выбранный м/с
		/// </summary>
		public Serial SelectedSerial
		{
			get => _selectedSerial;
			set => ChangeSelectedSerial(value);
		}

		/// <summary>
		/// Выбранный сезон
		/// </summary>
		public Season SelectedSeason
		{
			get => _selectedSeason;
			set => ChangeSelectedSeason(value);
		}

		/// <summary>
		/// Выбранный эпизод
		/// </summary>
		public Episode SelectedEpisode
		{
			get => _selectedEpisode;
			set => ChangeSelectedEpisode(value);
		}

		public SerialVoiceOver SelectedVoiceOver
		{
			get => _selectedVoiceOver;
			set => ChangeSelectedVoiceOver(value);

		}

		#endregion

		#region Lists data

		public bool IsSerialListEnable
		{
			get => _isSerialListEnable;
			set
			{
				_isSerialListEnable = value;
				NotifyOfPropertyChange(() => IsSerialListEnable);
			}
		}

		public bool IsSeasonListEnable
		{
			get => _isSeasonListEnable;
			set
			{
				_isSeasonListEnable = value;
				NotifyOfPropertyChange(() => IsSeasonListEnable);
			}
		}

		public bool IsEpisodeListEnable
		{
			get => _isEpisodeListEnable;
			set
			{
				_isEpisodeListEnable = value;
				NotifyOfPropertyChange(() => IsEpisodeListEnable);
			}
		}

		#endregion

		#region Visibility

		public Visibility SelectedSerialVisibility =>
			SelectedSerial == null
				? Visibility.Hidden
				: Visibility.Visible;

		public Visibility SelectedSeasonVisibility =>
			SelectedSeason == null
				? Visibility.Hidden
				: Visibility.Visible;

		public Visibility SelectedEpisodeVisibility =>
			SelectedEpisode == null
				? Visibility.Hidden
				: Visibility.Visible;



		#endregion


		public bool HasChanges { get; }

		public void SaveChanges()
		{

		}

		public bool CanSaveChanges => true;

	}
}

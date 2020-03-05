// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using Models.FilmModels;
	using Caliburn.Micro;

	public partial class EpisodesEditorViewModel : Conductor<Screen>.Collection.OneActive
	{
		private Episode _currentEpisode;
		private EpAddress _selectedAddress;
		private EpisodeEditingViewModel EEditingVM;
		private bool _isInterfaceUnlock = true;
		private BindableCollection<EpAddress> _addresses;

		/// <summary>
		/// Родительская модель представления
		/// </summary>
		public EditorSelectorViewModel ESVM => Parent as EditorSelectorViewModel;

		/// <summary>
		/// Текущий эпизод
		/// </summary>
		public Episode CurrentEpisode
		{
			get => _currentEpisode;
			set
			{
				_currentEpisode = value;
				NotifyOfPropertyChange(() => CurrentEpisode);
				NotifyNavigatingButtons();
				NotifyOfPropertyChange(() => CanEditAddress);
				NotifyOfPropertyChange(() => CanRemoveAddress);
				NotifyOfPropertyChange(() => CanCancelAddressSelection);
			}
		}

		/// <summary>
		/// Список адресов эпизода
		/// </summary>
		public BindableCollection<EpAddress> Addresses
		{
			get => _addresses;
			set
			{
				_addresses = value;
				NotifyOfPropertyChange(() => Addresses);
			}
		}



		
		//public BindableCollection<EpAddress> Addresses => new BindableCollection<EpAddress>(CurrentEpisode.Addresses);
		/// <summary>
		/// Выбранный адрес
		/// </summary>
		public EpAddress SelectedAddress
		{
			get => _selectedAddress;
			set
			{
				_selectedAddress = value;
				NotifyOfPropertyChange(() => SelectedAddress);
				NotifyOfPropertyChange(() => CanEditAddress);
				NotifyOfPropertyChange(() => CanRemoveAddress);	
				NotifyOfPropertyChange(() => CanCancelAddressSelection);
				NotifyOfPropertyChange(() => CanEditPrevAddress);
				NotifyOfPropertyChange(() => CanEditNextAddress);
				NotifyOfPropertyChange(() => CanSelectAddress);
			}
		}

		/// <summary>
		/// Флаг статуса блокировки интерфейса
		/// </summary>
		public bool IsInterfaceUnlock
		{
			get => _isInterfaceUnlock;
			set
			{
				_isInterfaceUnlock = value;
				NotifyOfPropertyChange(() => IsInterfaceUnlock);
			}
		}		
	}
}

// ReSharper disable CheckNamespace

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Models.FilmModels;
    using Caliburn.Micro;

    public partial class EpisodesEditorViewModel : Conductor<Screen>.Collection.OneActive
    {
        private Episode _currentEpisode;
        private AddressInfo _selectedAddressInfo;
        private EpisodeEditingViewModel EEditingVM;
        private bool _isInterfaceUnlock = true;
        private BindableCollection<AddressInfo> _addresses = new BindableCollection<AddressInfo>();
        private BindableCollection<AddressInfo> _selectedAddresses = new BindableCollection<AddressInfo>();

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
                NotifyOfPropertyChange(() => CanEditNextEpisode);
                NotifyOfPropertyChange(() => CanEditPrevEpisode);
            }
        }

        /// <summary>
        /// Список адресов эпизода
        /// </summary>
        public BindableCollection<AddressInfo> Addresses
        {
            get => _addresses;
            set
            {
                _addresses = value;
                NotifyOfPropertyChange(() => Addresses);
                NotifyOfPropertyChange(() => CanSelectAddress);
                NotifyOfPropertyChange(() => CanEditNextAddress);
                NotifyOfPropertyChange(() => CanEditPrevAddress);
                NotifyOfPropertyChange(() => CanSetDefaultAddress);
            }
        }

        public BindableCollection<AddressInfo> SelectedAddresses
        {
            get => _selectedAddresses;
            set
            {
                _selectedAddresses = value;
                NotifyOfPropertyChange(() => SelectedAddresses);
                NotifyOfPropertyChange(() => CanSelectAddress);
                NotifyOfPropertyChange(() => CanCancelAddressSelection);
                NotifyOfPropertyChange(() => CanRemoveAddress);
                NotifyOfPropertyChange(() => CanEditAddress);
                NotifyOfPropertyChange(() => CanEditNextAddress);
                NotifyOfPropertyChange(() => CanEditPrevAddress);
                NotifyOfPropertyChange(() => CanSetDefaultAddress);
            }
        }


        /// <summary>
        /// Выбранный адрес
        /// </summary>
        public AddressInfo SelectedAddressInfo
        {
            get => _selectedAddressInfo;
            set
            {
                _selectedAddressInfo = value;
                NotifyOfPropertyChange(() => SelectedAddressInfo);
                NotifyOfPropertyChange(() => CanEditPrevAddress);
                NotifyOfPropertyChange(() => CanEditNextAddress);
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
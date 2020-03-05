// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using System;
	using System.Linq;
	using System.Windows;
	using Caliburn.Micro;
	using Models.FilmModels;
	using Newtonsoft.Json;
	using static Helpers.GlobalMethods;
	using static Helpers.SystemVariables;

	public partial class AddressEditingViewModel : Screen
	{
		private EpAddress _currentAddress;
		private int? _newJumperNumber;
		private Jumper _selectedJumper;
		private Uri _newAddress;
		private bool _isInterfaceUnlocked = true;

		#region Свойства адреса

		/// <summary>
		/// Текущий адрес
		/// </summary>
		public EpAddress CurrentAddress
		{
			get => _currentAddress;
			set
			{
				_currentAddress = value;
				NotifyOfPropertyChange(() => CurrentAddress);
			}
		}

		/// <summary>
		/// Строковый экземпляр адреса для отслеживания изменений
		/// </summary>
		public string AddressSnapshot { get; set; }

		/// <summary>
		/// Новый адрес
		/// </summary>
		public Uri NewAddress
		{
			get => _newAddress;
			set
			{
				_newAddress = value;
				NotifyOfPropertyChange(() => NewAddress);
			}
		}

		/// <summary>
		/// Флаг наличия изменений свойств адреса
		/// </summary>
		private bool HasAddressChanges =>
			IsEquals(CurrentAddress, JsonConvert.DeserializeObject(AddressSnapshot)) is false ||
			CurrentAddress.Address != NewAddress;

		#endregion

		#region Свойства джампера

		/// <summary>
		/// Список джамперов
		/// </summary>
		public BindableCollection<Jumper> Jumpers => new BindableCollection<Jumper>(_currentAddress.Jumpers);

		/// <summary>
		/// Выбранный джампер
		/// </summary>
		public Jumper SelectedJumper
		{
			get => _selectedJumper;
			set
			{
				_selectedJumper = value;
				if (value != null)
				{
					JumperSnapshot = JsonConvert.SerializeObject(value);
					NewJumperNumber = value.Number;
				}
				NotifyOfPropertyChange(() => SelectedJumper);
				NotifyOfPropertyChange(() => CanSelectJumper);
				NotifyOfPropertyChange(() => CanEditJumper);
				NotifyOfPropertyChange(() => CanRemoveJumper);
				NotifyOfPropertyChange(() => CanCancelSelection);

			}
		}
		/// <summary>
		/// СТроковый экземпляр джампера для отслеживания изменений
		/// </summary>
		public string JumperSnapshot { get; set; }

		/// <summary>
		/// Свойство Visibility интерфейса редактирования джампера
		/// </summary>
		public Visibility JumperEditingVisibility => IsInterfaceUnlocked != true
			? Visibility.Visible
			: Visibility.Hidden;

		/// <summary>
		/// Новый номер джампера
		/// </summary>
		public int? NewJumperNumber
		{
			get => _newJumperNumber;
			set
			{
				if (value == null || value < 0)
				{
					value = 1;
				}
				_newJumperNumber = value;
				NotifyOfPropertyChange(() => NewJumperNumber);
			}
		}

		/// <summary>
		/// Список типов фильма
		/// </summary>
		public BindableCollection<JumperMode> JumperModes =>
			new BindableCollection<JumperMode>(Enum.GetValues(typeof(JumperMode)).Cast<JumperMode>());

		/// <summary>
		/// Флаг наличия изменений свойств джампера
		/// </summary>
		private bool HasJumperChanges
		{
			get
			{
				if (SelectedJumper == null)
				{
					return false;
				}

				return IsEquals(SelectedJumper, JsonConvert.DeserializeObject(JumperSnapshot)) is false
					|| SelectedJumper.Number != NewJumperNumber;
			}
		}

		#endregion

		public EpisodesEditorViewModel EEVM => Parent as EpisodesEditorViewModel;

		/// <summary>
		/// Флаг состояния разблокировки интерфейса (активация при изменении данных джампера)
		/// </summary>
		public bool IsInterfaceUnlocked
		{
			get => _isInterfaceUnlocked;
			set
			{
				_isInterfaceUnlocked = value;
				NotifyOfPropertyChange(() => IsInterfaceUnlocked);
				NotifyOfPropertyChange(() => JumperEditingVisibility);
			}
		}
		
	}
}

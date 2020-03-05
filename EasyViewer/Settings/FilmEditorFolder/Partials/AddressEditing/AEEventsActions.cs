// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using System;
	using System.Linq;
	using System.Windows.Controls;
	using System.Windows.Input;
	using Caliburn.Micro;
	using EasyViewer.ViewModels;
	using Models.FilmModels;
	using Newtonsoft.Json;
	using Vlc.DotNet.Wpf;
	using static Helpers.GlobalMethods;
	using static Helpers.SystemVariables;

	public partial class AddressEditingViewModel : Screen
	{
		#region Общие действия

		/// <summary>
		/// Двойной клик по текстовому полю
		/// </summary>
		/// <param name="source"></param>
		public void TBoxDoubleClick(TextBox source)
		{
			source.SelectAll();
		}

		/// <summary>
		/// Действие при изменении текста в TextBox'е
		/// </summary>
		public void TextChanged()
		{
			NotifyOfPropertyChange(() => CurrentAddress);
			NotifyOfPropertyChange(() => CanSetAddress);
			NotifyChanges();
		}

		/// <summary>
		/// Проверка является ливведенный сивол числом
		/// </summary>
		/// <param name="e"></param>
		public void NumericValidation(KeyEventArgs e)
		{
			e.Handled = (e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43 ||
						 e.Key.GetHashCode() >= 74 && e.Key.GetHashCode() <= 83) is false;
			NotifyChanges();
		}

		/// <summary>
		/// Сохранить изменения
		/// </summary>
		public void SaveChanges()
		{
			if (CanSaveChanges is false) return;

			var film = GetDbCollection<Film>().First(f => f.Name == EEVM.ESVM.SelectedFilm.Name);
			var index = EEVM.Addresses.IndexOf(CurrentAddress);
			film.Episodes
				.First(e => e.FullNumber == EEVM.CurrentEpisode.FullNumber).Addresses[index] = CurrentAddress;
			UpdateDbCollection(obj: film);
			AddressSnapshot = JsonConvert.SerializeObject(CurrentAddress);
			NotifyChanges();
			if (IsInterfaceUnlocked)
			{
				WinMan.ShowDialog(new DialogViewModel("Изменения адреса успешно сохранены", DialogType.INFO));
				return;
			}

			IsInterfaceUnlocked = true;
			CancelSelection();
			WinMan.ShowDialog(new DialogViewModel("Изменения джампера успешно сохранены", DialogType.INFO));
		}

		public bool CanSaveChanges => HasAddressChanges || HasJumperChanges;
		/// <summary>
		/// Отменить изменения
		/// </summary>
		public void CancelChanges()
		{
			if (CanCancelChanges is false) return;
			CurrentAddress = JsonConvert.DeserializeObject<EpAddress>(AddressSnapshot);
			NotifyChanges();
			if (IsInterfaceUnlocked) return;

			IsInterfaceUnlocked = true;
			CancelSelection();
		}

		public bool CanCancelChanges => HasAddressChanges || HasJumperChanges;

		#endregion

		#region Действия адреса

		/// <summary>
		/// Установить адрес
		/// </summary>
		public void SetAddress()
		{
			if (CanSetAddress is false) return;

			if (CheckAddress(NewAddress))
			{
				WinMan.ShowDialog(new DialogViewModel("Адрес успешно обновлен", DialogType.INFO));
				CurrentAddress.Address = NewAddress;
			}
			else
			{
				WinMan.ShowDialog(new DialogViewModel("Адрес некорректный или отсутствует интернет соединение",
					DialogType.INFO));
			}

			NotifyChanges();
			NotifyOfPropertyChange(() => CanSetAddress);
		}

		public bool CanSetAddress => CurrentAddress.Address != NewAddress;

		#endregion

		#region Действия джампера

		/// <summary>
		/// Выбрать первый джампер (Проблема с неактивным ListBox'ом при загрузке)
		/// </summary>
		public void SelectJumper()
		{
			if (CanSelectJumper is false) return;
			SelectedJumper = Jumpers.First();
		}

		public bool CanSelectJumper => Jumpers.Count > 0 && SelectedJumper != null;

		/// <summary>
		/// Добавить джампер
		/// </summary>
		public void AddJumper()
		{
			var film = GetDbCollection<Film>().First(f => f.Name == EEVM.ESVM.SelectedFilm.Name);
			var startTime = Jumpers.LastOrDefault()?.EndTime ?? new TimeSpan() + new TimeSpan(1000);
			var jumper = new Jumper
			{
				JumperMode = JumperModes.First(), 
				Number = Jumpers.Count + 1, 
				StartTime = startTime,
				EndTime = startTime + new TimeSpan(1000)
			};
			film.Episodes
				.First(e => e.FullNumber == EEVM.CurrentEpisode.FullNumber).Addresses
				.First(a => a.Address == CurrentAddress.Address).Jumpers.Add(jumper);
			UpdateDbCollection(obj: film);
			CurrentAddress.Jumpers.Add(jumper);
			NotifyOfPropertyChange(() => Jumpers);
		}

		/// <summary>
		/// Редактировать джампер
		/// </summary>
		public void EditJumper()
		{
			if (CanEditJumper is false) return;
			JumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);
			IsInterfaceUnlocked = false;
			Vlc = new VlcControl();
			Vlc.SourceProvider.CreatePlayer(VlcDataPath);
		}

		public bool CanEditJumper => SelectedJumper != null;
		/// <summary>
		/// Удалить джампер
		/// </summary>
		public void RemoveJumper()
		{
			if (CanRemoveJumper is false) return;
			var film = GetDbCollection<Film>().First(f => f.Name == EEVM.ESVM.SelectedFilm.Name);
			film.Episodes
				.First(e => e.FullNumber == EEVM.CurrentEpisode.FullNumber).Addresses
				.First(a => a.Address == CurrentAddress.Address).Jumpers.Remove(SelectedJumper);
			UpdateDbCollection(obj: film);
			Jumpers.Remove(SelectedJumper);
			NotifyChanges();
		}

		public bool CanRemoveJumper => SelectedJumper != null;
		/// <summary>
		/// Отменить выбор джампера
		/// </summary>
		public void CancelSelection()
		{
			if (CanCancelSelection is false) return;
			SelectedJumper = null;
		}

		public bool CanCancelSelection => SelectedJumper != null;

		/// <summary>
		/// Установить номере джампера
		/// </summary>
		public void SetJumperNumber()
		{
			if (CanSetJumperNumber is false) return;

			var (status, reason) = CheckJumperNumber(NewJumperNumber);

			if (status)
			{
				SelectedJumper.Number = (int)NewJumperNumber;
			}

			WinMan.ShowDialog(new DialogViewModel(reason, DialogType.INFO));
			NotifyChanges();
		}

		public bool CanSetJumperNumber => SelectedJumper?.Number != NewJumperNumber;

		#endregion

	}
}
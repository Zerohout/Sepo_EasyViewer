// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using System.Collections.Generic;
	using System.Linq;
	using Caliburn.Micro;
	using Models.FilmModels;
	using Screen = Caliburn.Micro.Screen;
	using static Helpers.GlobalMethods;

	public partial class EpisodesEditorViewModel : Conductor<Screen>.Collection.OneActive
	{
		/// <summary>
		/// Выбрать первый адрес в списке (решается баг с потерей фокуса курсора)
		/// </summary>
		public void SelectAddress()
		{
			if (CanSelectAddress is false) return;

			SelectedAddress = CurrentEpisode.Addresses.First();
		}

		public bool CanSelectAddress => Addresses.Count > 0 && SelectedAddress == null;
		/// <summary>
		/// Добавить новый адрес
		/// </summary>
		public void AddAddress()
		{
			var address = new EpAddress {Name = $"{CurrentEpisode.Name}_A{Addresses.Count + 1}"};
			CurrentEpisode.Addresses.Add(address);
			Addresses = new BindableCollection<EpAddress>(CurrentEpisode.Addresses);
			var episode = GetEpisodeFromDb(ESVM.SelectedFilm.Name, CurrentEpisode.FullNumber);
			episode.Addresses.Add(address);
			UpdateDbEpisode(episode);
			SelectedAddress = Addresses.FirstOrDefault();
			NotifyOfPropertyChange(() => CanSelectAddress);
		}
		/// <summary>
		/// Редактировать выбранный адрес
		/// </summary>
		public void EditAddress()
		{
			if (CanEditAddress is false) return;
			ChangeActiveItem(new AddressEditingViewModel(SelectedAddress, this));
		}

		public bool CanEditAddress => SelectedAddress != null;
		/// <summary>
		/// Удалить выбранный адрес
		/// </summary>
		public void RemoveAddress()
		{
			if (CanRemoveAddress is false) return;
			var episode = GetEpisodeFromDb(ESVM.SelectedFilm.Name, CurrentEpisode.FullNumber);
			episode.Addresses.RemoveAt(episode.Addresses.FindIndex(a => a.Name == SelectedAddress.Name));
			UpdateDbEpisode(episode);
			Addresses.Remove(SelectedAddress);
			SelectedAddress = Addresses.LastOrDefault();
			NotifyOfPropertyChange(() => Addresses);
		}

		public bool CanRemoveAddress => SelectedAddress != null;
		/// <summary>
		/// Отменить выбор адреса
		/// </summary>
		public void CancelAddressSelection()
		{
			if (CanCancelAddressSelection is false) return;
			SelectedAddress = null;
		}

		public bool CanCancelAddressSelection => SelectedAddress != null;

		/// <summary>
		/// Действия при нажатии клавиши
		/// </summary>
		/// <param name="eventArgs"></param>
		public void KeyDown(object eventArgs)
		{

		}
		/// <summary>
		/// Редактировать предыдущий эпизод
		/// </summary>
		public void EditPrevEpisode()
		{
			if (CanEditPrevEpisode is false) return;
			var prevEpisode = ESVM.Episodes[ESVM.Episodes.IndexOf(CurrentEpisode) - 1];
			ESVM.ResetSelectedEpisode(prevEpisode);
		}
		public bool CanEditPrevEpisode => CurrentEpisode.FullNumber != ESVM.Episodes.First()?.FullNumber;
		/// <summary>
		/// Редактировать следующий эпизод
		/// </summary>
		public void EditNextEpisode()
		{
			if (CanEditNextEpisode is false) return;
			var nextEpisode = ESVM.Episodes[ESVM.Episodes.IndexOf(CurrentEpisode) + 1];
			ESVM.ResetSelectedEpisode(nextEpisode);
		}
		public bool CanEditNextEpisode => CurrentEpisode.FullNumber != ESVM.Episodes.Last()?.FullNumber;
		/// <summary>
		/// Редактировать предыдущий адрес
		/// </summary>
		public void EditPrevAddress()
		{
			if (CanEditPrevAddress is false) return;
			SelectedAddress = Addresses[Addresses.IndexOf(SelectedAddress) - 1];
			ChangeActiveItem(new AddressEditingViewModel(SelectedAddress, this));
			NotifyNavigatingButtons();
		}
		public bool CanEditPrevAddress => SelectedAddress?.Address != Addresses.FirstOrDefault()?.Address;
		/// <summary>
		/// Редактировать следующий адрес
		/// </summary>
		public void EditNextAddress()
		{
			if (CanEditNextAddress is false) return;
			SelectedAddress = Addresses[Addresses.IndexOf(SelectedAddress) + 1];
			
			ChangeActiveItem(new AddressEditingViewModel(SelectedAddress, this));
			NotifyNavigatingButtons();
		}
		public bool CanEditNextAddress => SelectedAddress?.Name != Addresses.LastOrDefault()?.Name;

	}
}

// ReSharper disable CheckNamespace

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using Caliburn.Micro;
    using Helpers;
    using Models.FilmModels;
    using Screen = Caliburn.Micro.Screen;
    using static Helpers.DbMethods;

    public partial class EpisodesEditorViewModel : Conductor<Screen>.Collection.OneActive
    {
        /// <summary>
        /// Выбрать первый адрес в списке (решается баг с потерей фокуса курсора)
        /// </summary>
        public void SelectAddress()
        {
            if (CanSelectAddress is false) return;

            SelectedAddressInfo = Addresses.FirstOrDefault();
        }

        public bool CanSelectAddress => Addresses.Count > 0 && SelectedAddresses.Count == 0;

        /// <summary>
        /// Добавить новый адрес
        /// </summary>
        public void AddAddress()
        {
            var address = new AddressInfo
            {
                Name = $"{CurrentEpisode.Name}_A{Addresses.Count + 1}",
                Film = ESVM.SelectedFilm,
                Season = ESVM.SelectedSeason,
                Episode = CurrentEpisode
            };

            InsertEntityToDb(address);
            Addresses = new BindableCollection<AddressInfo>(CurrentEpisode.AddressInfoList.OrderBy(a => a.Name));
            SelectedAddressInfo = Addresses.FirstOrDefault();
        }

        /// <summary>
        /// Редактировать выбранный адрес
        /// </summary>
        public void EditAddress()
        {
            if (CanEditAddress is false) return;
            ChangeActiveItem(new AddressEditingViewModel(SelectedAddressInfo, this));
        }

        public bool CanEditAddress => SelectedAddresses.Count == 1;

        /// <summary>
        /// Удалить выбранный адрес
        /// </summary>
        public void RemoveAddress()
        {
            if (CanRemoveAddress is false) return;
            for (var i = 0; i < SelectedAddresses.Count; i++)
            {
                FullyAddressInfoDeleting(SelectedAddresses[i]);
            }
            Addresses = new BindableCollection<AddressInfo>(CurrentEpisode.AddressInfoList);
            SelectedAddressInfo = Addresses.LastOrDefault();
        }

        public bool CanRemoveAddress => SelectedAddresses.Count > 0;

        public void SetDefaultAddress()
        {
            if(CanSetDefaultAddress is false) return;
            CurrentEpisode.AddressInfo = SelectedAddressInfo;
            UpdateDbCollection(CurrentEpisode);
            NotifyOfPropertyChange(()=> CanSetDefaultAddress);
        }

        public bool CanSetDefaultAddress =>
            SelectedAddresses.Count == 1 && (CurrentEpisode.AddressInfo == null || GlobalMethods.IsEquals(CurrentEpisode.AddressInfo,SelectedAddressInfo) is false);

        /// <summary>
        /// Отменить выбор адреса
        /// </summary>
        public void CancelAddressSelection()
        {
            if (CanCancelAddressSelection is false) return;
            SelectedAddressInfo = null;
            if(ActiveItem.IsActive) ActiveItem.TryClose();
        }

        public bool CanCancelAddressSelection => SelectedAddresses.Count > 0;

        /// <summary>
        /// Действия при нажатии клавиши
        /// </summary>
        /// <param name="eventArgs"></param>
        public void KeyDown(object eventArgs)
        {
        }

        /// <summary>
        /// Изменение выбора в ListBox
        /// </summary>
        /// <param name="sender"></param>
        public void SelectionChanged(ListBox sender)
        {
            SelectedAddresses = new BindableCollection<AddressInfo>(sender.SelectedItems.Cast<AddressInfo>());
            if (SelectedAddresses.Count == 1) sender.ScrollIntoView(sender.SelectedItem);
        }

        /// <summary>
        /// Редактировать предыдущий эпизод
        /// </summary>
        public void EditPrevEpisode()
        {
            if (CanEditPrevEpisode is false) return;
            SystemVariables.IsEditDefaultAddressInfo = true;
            var prevEpisode = ESVM.Episodes[ESVM.Episodes.IndexOf(CurrentEpisode) - 1];
            ESVM.ResetSelectedEpisode(prevEpisode);
        }

        public bool CanEditPrevEpisode => CurrentEpisode.FullNumber != ESVM.Episodes.FirstOrDefault()?.FullNumber;

        /// <summary>
        /// Редактировать следующий эпизод
        /// </summary>
        public void EditNextEpisode()
        {
            if (CanEditNextEpisode is false) return;
            SystemVariables.IsEditDefaultAddressInfo = true;
            var nextEpisode = ESVM.Episodes[ESVM.Episodes.IndexOf(CurrentEpisode) + 1];
            ESVM.ResetSelectedEpisode(nextEpisode);
        }

        public bool CanEditNextEpisode => CurrentEpisode.FullNumber != ESVM.Episodes.LastOrDefault()?.FullNumber;

        /// <summary>
        /// Редактировать предыдущий адрес
        /// </summary>
        public void EditPrevAddress()
        {
            if (CanEditPrevAddress is false) return;
            SelectedAddressInfo = Addresses[Addresses.IndexOf(SelectedAddressInfo) - 1];
            ChangeActiveItem(new AddressEditingViewModel(SelectedAddressInfo, this));
            NotifyNavigatingButtons();
        }

        public bool CanEditPrevAddress => SelectedAddresses.Count == 1 && SelectedAddressInfo?.Link != Addresses.FirstOrDefault()?.Link;

        /// <summary>
        /// Редактировать следующий адрес
        /// </summary>
        public void EditNextAddress()
        {
            if (CanEditNextAddress is false) return;
            SelectedAddressInfo = Addresses[Addresses.IndexOf(SelectedAddressInfo) + 1];

            ChangeActiveItem(new AddressEditingViewModel(SelectedAddressInfo, this));
            NotifyNavigatingButtons();
        }

        public bool CanEditNextAddress => SelectedAddresses.Count == 1 &&  SelectedAddressInfo?.Link != Addresses.LastOrDefault()?.Link;

        public override void TryClose(bool? dialogResult = null)
        {
            if(ActiveItem != null && ActiveItem.IsActive) ActiveItem.TryClose();
            base.TryClose(dialogResult);
        }
    }
}
// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.ViewingsSettingsFolder.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Helpers;
    using Models.FilmModels;

    public partial class ViewingsSettingsViewModel : Screen
    {
        public void SeasonCheckValidation(Season season)
        {
            DbMethods.UpdateDbCollection(season);
        }

        public void EpisodeCheckValidation(Episode episode)
        {
            DbMethods.UpdateDbCollection(episode);
        }

        public void AddressInfoCheck(AddressInfo addressInfo)
        {
            var episode = DbMethods.GetEpisodeFromDbById(addressInfo.Episode.Id);
            episode.AddressInfo = addressInfo;
            DbMethods.UpdateDbCollection(episode);
            AddressInfoList = new BindableCollection<AddressInfo>(SelectedEpisode.AddressInfoList);
        }

        public void AddressInfoUncheck(AddressInfo addressInfo)
        {
            var episode = DbMethods.GetEpisodeFromDbById(addressInfo.Episode.Id);
            episode.AddressInfo = AddressInfoList.FirstOrDefault(a => a.Id != addressInfo.Id);
            DbMethods.UpdateDbCollection(episode);
            AddressInfoList = new BindableCollection<AddressInfo>(SelectedEpisode.AddressInfoList);
        }

        #region Cancel selection buttons
        /// <summary>
        /// Снять выделение с выбранного фильма
        /// </summary>
        public void CancelFilmSelection() => SelectedFilm = null;
        public bool CanCancelFilmSelection => SelectedFilm != null;

        /// <summary>
        /// Снять выделение с выбранного сезона
        /// </summary>
        public void CancelSeasonSelection() => SelectedSeason = null;
        public bool CanCancelSeasonSelection => SelectedSeason != null;

        /// <summary>
        /// Снять выделение с выбранного эпизода
        /// </summary>
        public void CancelEpisodeSelection() => SelectedEpisode = null;
        public bool CanCancelEpisodeSelection => SelectedEpisode != null;

        /// <summary>
        /// Снять выделение с выбранной озвучки
        /// </summary>
        public void CancelVoiceOverSelection() => SelectedAddressInfo = null;
        public bool CanCancelVoiceOverSelection => SelectedAddressInfo != null;

        #endregion

    }
}

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System.Linq;
    using System.Windows.Navigation;
    using Caliburn.Micro;
    using Helpers;
    using Models.FilmModels;

    public partial class EpisodesEditorViewModel : Conductor<Screen>.Collection.OneActive
    {
        public EpisodesEditorViewModel()
        {
        }

        public EpisodesEditorViewModel(Episode episode, bool isEditDefaultAddressInfo)
        {
            CurrentEpisode = episode;
            EEditingVM = new EpisodeEditingViewModel(episode, this);
            ActiveItem = EEditingVM;
            Addresses = new BindableCollection<AddressInfo>(episode.AddressInfoList);
            if (Addresses.Count == 0) return;
            var defaultAddressInfo = DbMethods.GetAddressInfoFromDbById(episode.AddressInfo.Id);
            if (episode.AddressInfo != null)
            {
                ChangeActiveItem(new AddressEditingViewModel(defaultAddressInfo, this));
            }
            else
            {
                ChangeActiveItem(new AddressEditingViewModel(Addresses.FirstOrDefault(), this));
            }

            SelectedAddressInfo = Addresses.FirstOrDefault(a => a.Id == defaultAddressInfo.Id);
        }

        protected override void OnInitialize()
        {
            DisplayName = "Редактор эпизодов";

            base.OnInitialize();
        }
    }
}
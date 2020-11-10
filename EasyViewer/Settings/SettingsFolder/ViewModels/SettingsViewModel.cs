namespace EasyViewer.Settings.SettingsFolder.ViewModels
{
    using System.Collections.Generic;
    using Caliburn.Micro;
    using EasyViewer.ViewModels;
    using FilmEditorFolder.ViewModels;
    using MainMenu.ViewModels;
    using ViewingsSettingsFolder.ViewModels;
    using WatchingSettingsFolder.ViewModels;
    using static Helpers.SystemVariables;


    public class SettingsViewModel : Conductor<Screen>.Collection.OneActive
    {
        public SettingsViewModel()
        {
        }

        protected override void OnInitialize()
        {
            AddSettingsToList();
            base.OnInitialize();
        }

        private void AddSettingsToList()
        {
            Settings.Clear();
            Settings.AddRange(new List<Screen>
            {
                new WatchingSettingsViewModel {DisplayName = "Основные настройки", Parent = this},
                new EditorSelectorViewModel {DisplayName = "Редактор фильмов", Parent = this},
                new ViewingsSettingsViewModel {DisplayName = "Настройки просмотра", Parent = this}
            });
        }

        private BindableCollection<Screen> _settings = new BindableCollection<Screen>();

        public BindableCollection<Screen> Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                NotifyOfPropertyChange(() => Settings);
            }
        }

        public void ChangeActiveItem(Screen viewModel)
        {
            ActiveItem?.TryClose();

            ActiveItem = viewModel;
        }

        //private Screen lastActiveItem;

        public void SelectionChanged()
        {
            if (ActiveItem == null) return;

            if (ActiveItem is EditorSelectorViewModel cevm)
            {
                if (cevm.ActiveItem == null) return;

                //if (cevm.ActiveItem is FilmsEditingViewModel ce)
                //{
                //	ce.UpdateVoiceOverList();
                //	return;
                //}

                //if (cevm.ActiveItem is EpisodesEditingViewModel ee)
                //{
                //	ee.UpdateVoiceOverList();
                //	if (ee.IsNotEditing is false)
                //	{

                //	}

                //}

                //return;
            }

            //if (ActiveItem is VoiceOversEditingViewModel voe)
            //{
            //	//voe.UpdateVoiceOverList();
            //}
        }

        public void BackToMainMenu()
        {
            foreach (var activeItem in Settings)
            {
                activeItem.TryClose();
            }

            ActiveItem?.TryClose();
            ((MainViewModel) Parent).ChangeActiveItem(new MainMenuViewModel());
        }

        private bool CancelBackToMainMenu(ISettingsViewModel svm)
        {
            if (svm.HasChanges)
            {
                var dvm = new DialogViewModel(
                    $"В меню \"{svm.DisplayName}\" имеются не сохраненные изменения\nСохранить их?",
                    DialogType.SaveChanges);

                WinMan.ShowDialog(dvm);

                switch (dvm.DialogResult)
                {
                    case DialogResult.YesAction:
                        svm.SaveChanges();
                        svm.TryClose();
                        break;
                    case DialogResult.NoAction:
                        svm.TryClose();
                        break;
                    default:
                        return true;
                }

                return false;
            }

            return false;
        }
    }
}
namespace EasyViewer.MainMenu.ViewModels
{
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Caliburn.Micro;
    using EasyViewer.ViewModels;
    using Helpers;
    using Models.FilmModels;
    using static Helpers.SystemVariables;
    using Screen = Caliburn.Micro.Screen;

    public partial class MainMenuViewModel : Screen
    {
        public MainMenuViewModel()
        {
        }

        protected override async void OnInitialize()
        {
            
            HotReg.RegisterGlobalHotkey(PauseButtonAction, Keys.Pause, ModifierKeys.None);
            HotReg.RegisterGlobalHotkey(PauseButtonAction, Keys.End, ModifierKeys.None);
            HotReg.RegisterGlobalHotkey(async () => await IncreaseWatchingEpisodes(), Keys.PageUp, ModifierKeys.None);
            HotReg.RegisterGlobalHotkey(async () => await DecreaseWatchingEpisodes(), Keys.PageDown, ModifierKeys.None);
            HotReg.RegisterGlobalHotkey(CollapseMainWindow, Keys.PageDown, ModifierKeys.Control);
            //HotReg.RegisterGlobalHotkey(Start, Keys.P, ModifierKeys.Alt);
            //HotReg.RegisterGlobalHotkey(() => { IsSwitchEpisode = true; }, Keys.Right, ModifierKeys.Control);
            //Helper.HotReg.RegisterGlobalHotkey(() => EpisodeCountString = (EpisodeCount - 1).ToString(), Keys.Delete, ModifierKeys.Shift);

            await LoadData();
            base.OnInitialize();
        }
    }
}
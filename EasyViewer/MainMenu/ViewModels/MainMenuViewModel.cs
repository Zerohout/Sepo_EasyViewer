namespace EasyViewer.MainMenu.ViewModels
{
    using System.Windows.Forms;
    using System.Windows.Input;
    using static Helpers.SystemVariables;
    using Screen = Caliburn.Micro.Screen;

    public partial class MainMenuViewModel : Screen
    {
        public MainMenuViewModel()
        {

        }

        protected override void OnInitialize()
        {
            HotReg.RegisterGlobalHotkey(PauseButtonAction, Keys.Pause, ModifierKeys.None);
            //HotReg.RegisterGlobalHotkey(Start, Keys.P, ModifierKeys.Alt);
            //HotReg.RegisterGlobalHotkey(() => { IsSwitchEpisode = true; }, Keys.Right, ModifierKeys.Control);
            //Helper.HotReg.RegisterGlobalHotkey(() => EpisodeCountString = (EpisodeCount - 1).ToString(), Keys.Delete, ModifierKeys.Shift);

            AppVal.WS = Helpers.GlobalMethods.LoadOrCreateWatchingSettings();
            LoadFilms();
            SetCheckedEpisodes();
            base.OnInitialize();
        }
    }
}

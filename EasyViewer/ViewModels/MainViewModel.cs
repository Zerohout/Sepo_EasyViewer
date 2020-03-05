namespace EasyViewer.ViewModels
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Helpers;
    using MainMenu.ViewModels;
    using static Helpers.GlobalMethods;
    using static Helpers.SystemVariables;
    using Application = System.Windows.Application;
    using Screen = Caliburn.Micro.Screen;

    public class MainViewModel : Conductor<Screen>.Collection.OneActive
    {

        protected override void OnViewLoaded(object view)
        {
            HotReg = new HotkeysRegistrator(GetView() as Window);
            HotReg.RegisterGlobalHotkey(Exit, Keys.Pause, ModifierKeys.Shift);

            Tray.Icon = new Icon("AppData\\Images\\trayIcon.ico");
            Tray.Visible = false;
            Tray.DoubleClick += TrayDoubleClick;

            PreLaunchPreparation();


            ActiveItem = new MainMenuViewModel()
            {
                Parent = this
            };
            base.OnViewLoaded(view);
        }

        public void TrayDoubleClick(object sender, EventArgs e)
        {
            ActivateDeactivateTray((Window)GetView());
            //((Window)GetView()).Show();
            //Tray.Visible = false;
        }

        public void PreLaunchPreparation()
        {
            if (Directory.Exists(AppDataPath) is false)
            {
                Directory.CreateDirectory(AppDataPath);
            }

            if (Directory.Exists(ImagesPath) is false)
            {
                Directory.CreateDirectory(ImagesPath);
            }

            if (Directory.Exists($"{AppPath}\\{ErrorLogsFolderName}") is false)
            {
                Directory.CreateDirectory($"{AppPath}\\{ErrorLogsFolderName}");
            }

            if (Directory.Exists($"{AppPath}\\{DevelopmentDataFolderName}") is false)
            {
	            Directory.CreateDirectory($"{AppPath}\\{DevelopmentDataFolderName}");
            }
        }

        public void ChangeActiveItem(Screen viewModel)
        {
            ActiveItem?.TryClose();
            ActiveItem = viewModel;
        }

        public void MouseDown()
        {
            (GetView() as Window)?.DragMove();
        }

        /// <summary>
        /// Закрытие программы
        /// </summary>
        public void Exit()
        {
            ActiveItem?.TryClose();
            Tray.Dispose();
            HotReg?.UnregisterHotkeys();

            Application.Current.Shutdown();
        }
    }
}

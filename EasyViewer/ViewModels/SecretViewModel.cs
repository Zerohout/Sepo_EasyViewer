namespace EasyViewer.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Caliburn.Micro;

    public partial class SecretViewModel : Screen
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();
            pb = new PasswordBox();
        }

        public void Accept()
        {
            if (GetHash(pass) == "0E87C1DC8B10CF2989AC77169E1C961B")
            {
                TryClose(true);
            }

            pb.Password = string.Empty;

        }

        private string Second(string str, int num1, int num2) => $"{str}{Math.Pow(num1 + num2, powPow)}";

        private readonly Random rnd = new Random();

        public char PasswordChar => (char)rnd.Next(33, 127);

        private string pass;
        private int step = 2;

        private PasswordBox pb;

        public void PasswordChanged(PasswordBox sender, RoutedEventArgs args)
        {
            pb = sender;
            pass = sender.Password;
            NotifyOfPropertyChange(() => PasswordChar);
        }

        public void Exit()
        {
            TryClose(false);
        }
        private int powPow = 4;
        private int stopCount = 5;
    }
}

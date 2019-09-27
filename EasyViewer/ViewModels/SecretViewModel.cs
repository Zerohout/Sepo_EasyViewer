namespace EasyViewer.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Caliburn.Micro;

    public partial class SecretViewModel : Screen
    {
        public void Accept()
        {
            if (GetHash(pass) == "348E74A75DFE134CFED7724A845EC689")
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

        private PasswordBox pb = new PasswordBox();

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

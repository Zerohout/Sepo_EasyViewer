// ReSharper disable CheckNamespace
#pragma warning disable 649
namespace EasyViewer.ViewModels
{
    using System;
    using Caliburn.Micro;

    public partial class SecretViewModel : Screen
    {
        private int startStep;
        private int stop;
        private string tlas = "wisely";

        private string CreateW(string str)
        {
            var result = "";
            for (var i = startStep; i < str.Length; i += step)
            {
                result += str[i];
            }

            return result;
        }

        private string First(string str, int num1, int num2) => $"{str}{Math.Atan2(num1, num2)}";
    }
}

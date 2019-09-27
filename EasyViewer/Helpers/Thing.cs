// ReSharper disable once CheckNamespace
namespace EasyViewer.ViewModels
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Caliburn.Micro;

    public partial class SecretViewModel : Screen
    {
        private string GetHash(string str)
        {
            var result = BitConverter
                         .ToString(new MD5CryptoServiceProvider()
                                       .ComputeHash(Encoding.Unicode
                                                            .GetBytes(str += tlas)))
                         .Replace("-", string.Empty);
            var word = CreateW(result);
            if (stop >= stopCount)
            {
                return BitConverter
                    .ToString(new MD5CryptoServiceProvider()
                                  .ComputeHash(Encoding.Unicode
                                                       .GetBytes(Second(str + word, stop * stopCount / stop, stopCount))))
                       .Replace("-", string.Empty);
            }

            stop++;
            return GetHash(First(result + word, stop, stopCount));
        }

        //private string First(string str, int num1, int num2) => $"{str}{Math.Atan2(num1, num2)}";

        //private string Second(string str, int num1, int num2) => $"{str}{Math.Pow(num1 + num2, powPow)}";

    }
}

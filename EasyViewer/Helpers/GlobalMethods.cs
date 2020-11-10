namespace EasyViewer.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using LiteDB;
    using Models.FilmModels;
    using Models.SettingModels;
    using Newtonsoft.Json;
    using static SystemVariables;

    public static class GlobalMethods
    {
        /// <summary>
        /// Сравнить два объекта
        /// </summary>
        /// <param name="obj1">Первый объект</param>
        /// <param name="obj2">Второй объект</param>
        /// <returns></returns>
        public static bool IsEquals(object obj1, object obj2)
        {
            var first = JsonConvert.SerializeObject(obj1);
            var second = JsonConvert.SerializeObject(obj2);
            return first.Equals(second);
        }

        /// <summary>
        /// Активация/Деактивация трея
        /// </summary>
        /// <param name="view">Сворачиваемое представление</param>
        public static void ActivateDeactivateTray(Window view)
        {
            if (Tray.Visible)
            {
                Tray.Visible = false;
                view.Show();
                view.Activate();
            }
            else
            {
                Tray.Visible = true;
                view.Hide();
            }
        }

        /// <summary>
        /// Перевод слова на латиницу
        /// </summary>
        /// <param name="source"> это входная строка для транслитерации </param>
        /// <returns>получаем строку после транслитерации</returns>
        public static string TranslateFileName(string source)
        {
            var result = "";

            foreach (var ch in source)
            {
                if (TranslitDictionary.TryGetValue(ch.ToString(), out var ss))
                {
                    result += ss;
                }
                else
                {
                    result += ch;
                }
            }

            return result;
        }
    }
}
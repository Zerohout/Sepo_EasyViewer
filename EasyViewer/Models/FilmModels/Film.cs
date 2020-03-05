namespace EasyViewer.Models.FilmModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using static Helpers.SystemVariables;

    /// <summary>
    /// фильм (мультфильм, кинофильм, видео и т.д.)
    /// </summary>
    [Serializable]
    public class Film
    {
        public Film()
        {
            Seasons = new List<Season>();
        }
        /// <summary>
        /// ID фильма
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }
        /// <summary>
        /// Название фильма
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание фильма
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Флаг выбора фильма
        /// </summary>
        public bool Checked { get; set; } = true;
        /// <summary>
        /// Тип фильма
        /// </summary>
        public FilmType FilmType { get; set; }
        /// <summary>
        /// Список сезонов фильма
        /// </summary>
        [JsonIgnore]
        public List<Season> Seasons { get; set; }
        /// <summary>
        /// Получить список эпизодов фильма
        /// </summary>
        [JsonIgnore]
        public List<Episode> Episodes => Seasons.SelectMany(s => s.Episodes).ToList();

        /// <summary>
        /// Получить (все/количество) (все/выбранные) эпизоды (с учётом/без учета) интервалов простоя
        /// </summary>
        /// <param name="isChecked">True - выбранные, false - все эпизоды</param>
        /// <param name="nonRepeatInterval">Интервал простоя эпизода в днях (-1 - без учета интервала)</param>
        /// <param name="count">Количество эпизодов с учетом опций (0 - все)</param>
        /// <returns></returns>
        public List<Episode> GetEpisodes(bool isChecked = false, int nonRepeatInterval = -1, int count = 0)
        {
            var result = Seasons.Where(s => !isChecked || s.Checked)
                                 .SelectMany(s => s.Episodes
                                                   .Where(e => !isChecked || e.Checked)
                                                   .Where(e => DateTime.Now.Subtract(
                                                                   e.LastDateViewed).TotalDays > nonRepeatInterval))
                                 .ToList();
            if (count <= 0 ||
                count > result.Count) count = result.Count;

            return result.Take(count).ToList();
        }
        ///// <summary>
        ///// Получить (общую/фактическую) длительность (выбранных/всех) эпизодов 
        ///// </summary>
        ///// <param name="isTotal">True - общая, false - фактическая длительность</param>
        ///// <param name="isChecked">True - выбранные, false - все эпизоды</param>
        ///// <param name="nonRepeatInterval">Интервал простоя эпизода в днях (-1 - без учета интервала)</param>
        ///// <param name="count">Количество эпизодов с учетом опций (0 - все)</param>
        ///// <returns></returns>
        //public TimeSpan GetEpisodesDuration(bool isTotal = true, bool isChecked = false, int nonRepeatInterval = -1, int count = 0)
        //{
        //    var episodes = GetEpisodes(isChecked, nonRepeatInterval, count);

        //    return TimeSpan.FromSeconds(episodes.Sum(e => isTotal
        //                                                 ? e.TotalDuration.TotalSeconds
        //                                                 : e.ActualDuration.TotalSeconds));
        //}
    }
}

namespace EasyViewer.Models.FilmModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Сезон фильма
    /// </summary>
    [Serializable]
    public class Season
    {
        public Season()
        {
            Episodes = new List<Episode>();
        }
        /// <summary>
        /// Номер сезона
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Название сезона
        /// </summary>
        [JsonIgnore]
        public string Name => $"Сезон №{Number}";
        /// <summary>
        /// Описание сезона
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Флаг выбора сезона
        /// </summary>
        public bool Checked { get; set; } = true;
        /// <summary>
        /// Эпизоды сезона
        /// </summary>
        [JsonIgnore]
        public List<Episode> Episodes { get; set; }
        /// <summary>
        /// Получить (все/количество) (все/выбранные) эпизоды
        /// </summary>
        /// <param name="isChecked">True - только выбранные, false - все эпизоды</param>
        /// <param name="nonRepeatInterval">Интервал простоя эпизода в днях (-1 - без учета интервала)</param>
        /// <param name="count">Количество эпизодов с учетом опций (0 - все)</param>
        /// <returns></returns>
        public List<Episode> GetEpisodes(bool isChecked = false, int nonRepeatInterval = -1, int count = 0)
        {
            if (count <= 0 ||
                count > Episodes.Count) count = Episodes.Count;

            return Episodes.Where(e => !isChecked || e.Checked)
                           .Where(e => DateTime.Now.Subtract(
                                           e.LastDateViewed).TotalDays > nonRepeatInterval)
                           .Take(count).ToList();
        }
        ///// <summary>
        ///// Получить (общую/фактическую) длительность (всех/определенного количества)  (выбранных/всех) эпизодов 
        ///// </summary>
        ///// <param name="isTotal">True - общая, false - фактическая длительность</param>
        ///// <param name="isChecked">True - выбранные, false - все эпизоды</param>
        ///// <param name="nonRepeatInterval">Интервал простоя эпизода в днях (-1 - без учета интервала)</param>
        ///// <param name="count">Количество эпизодов с учетом опций (0 - все)</param>
        ///// <returns></returns>
        //public TimeSpan GetEpisodesDuration(bool isTotal = true, bool isChecked = false, int nonRepeatInterval = -1, int count = 0)
        //{
        //    if (count <= 0 || count > Episodes.Count) count = Episodes.Count;

        //    var episodes = GetEpisodes(isChecked, nonRepeatInterval, count);

        //    return TimeSpan.FromSeconds(episodes.Sum(e => isTotal
        //                                         ? e.TotalDuration.TotalSeconds
        //                                         : e.ActualDuration.TotalSeconds));
        //}
    }
}

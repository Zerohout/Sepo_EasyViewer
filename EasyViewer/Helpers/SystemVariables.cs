namespace EasyViewer.Helpers
{
	using System;
	using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using Caliburn.Micro;
    using Vlc.DotNet.Core;
    using Vlc.DotNet.Wpf;
    using Timer = System.Timers.Timer;

    public static class SystemVariables
    {
        public const string MMBackgroundUri = "../../Resources/Images/HDMMBackground.png";
        public const string MMBackgroundOnExitUri = "../../Resources/Images/HDMMBackgroundOnExit.png";
        public const string DefaultLogoImageName = "DefaultLogo.png";


        public const string NewFilmName = "_Новый_фильм";
        public const string SP = "Южный парк";
		public static CancellationTokenSource AddingFilmCancellationTokenSource;
		public static CancellationToken AddingFilmToken;

        /// <summary>
		/// Папка нахождения приложения
		/// </summary>
		public static string AppPath = new FileInfo(
            Assembly.GetEntryAssembly()?.Location ??
            AppDomain.CurrentDomain.BaseDirectory).DirectoryName;
        /// <summary>
        /// Название папки с данными приложения
        /// </summary>
        public const string AppDataFolderName = "AppData";
        /// <summary>
        /// Название папки с логами ошибок
        /// </summary>
        public const string ErrorLogsFolderName = "ErrorLogs";
		/// <summary>
		/// Название папки с данными разработки
		/// </summary>
        public const string DevelopmentDataFolderName = "DevelopmentData";
        /// <summary>
        /// Путь до папки с данными приложения
        /// </summary>
        public static readonly string AppDataPath = $"{AppPath}\\{AppDataFolderName}";
        /// <summary>
        /// Путь до папки с изображениями
        /// </summary>
        public static readonly string ImagesPath = $"{AppDataPath}\\Images";
        /// <summary>
        /// Путь до папки с базой данных
        /// </summary>
        public static readonly string DBPath = $"{AppDataPath}\\db.db";
        /// <summary>
        /// Путь к библиотекам VLC
        /// </summary>
        public static readonly DirectoryInfo VlcDataPath = new DirectoryInfo(
            Path.Combine(AppDataPath, "libvlc",
                         IntPtr.Size == 4
                             ? "win-x86"
                             : "win-x64"));
        /// <summary>
        /// Программный трей
        /// </summary>
        public static NotifyIcon Tray = new NotifyIcon();
        /// <summary>
        /// Названия файла с ошибкой
        /// </summary>
        public static readonly string LogName = $"{DateTime.Today:d}_{DateTime.Now.Hour:00}.{DateTime.Now.Minute:00}.{DateTime.Now.Second:00}.txt";

        /// <summary>
        /// Менеджер окон
        /// </summary>
        public static WindowManager WinMan = new WindowManager();

        public static IntPtr HWND;
        /// <summary>
        /// Регистратор горячий клавиш
        /// </summary>
        public static HotkeysRegistrator HotReg;


        public static AppValues AppVal = new AppValues();

        /// <summary>
        /// Статус загрузки
        /// </summary>
        public enum LoadingStatus
        {
            CreatingSeasons,
            CreatingEpisodes,
            CreatingAddresses,
            AddingDurations
        }

        /// <summary>
        /// Тип фильма
        /// </summary>
        public enum FilmType
        {
            Мультсериал,
            Киносериал,
            Мультфильм,
            Кинофильм,
            Видео,
            Плейлист
        }

        /// <summary>
        /// Тип диалогового окна
        /// </summary>
        public enum DialogType
        {
            /// <summary>
            /// Сохранение изменений/данных
            /// </summary>
            SaveChanges,
            /// <summary>
            /// Отмена изменений
            /// </summary>
            CancelChanges,
            /// <summary>
            /// Удаление объекта
            /// </summary>
            RemoveObject,
            /// <summary>
            /// Перезапись файла
            /// </summary>
            OverwriteFile,
            /// <summary>
            /// Вопрос пользователю
            /// </summary>
            Question,
            /// <summary>
            /// Информационное окно
            /// </summary>
            Info,
            /// <summary>
            /// Обработка ошибки
            /// </summary>
            Error
        }

        /// <summary>
        /// Результат диалогового окна
        /// </summary>
        public enum DialogResult
        {
            /// <summary>
            /// Положительный результат
            /// </summary>
            YesAction,
            /// <summary>
            /// Отрицательный результат
            /// </summary>
            NoAction,
            /// <summary>
            /// Результат отмены
            /// </summary>
            CancelAction
        }

        /// <summary>
        /// Режим джампера
        /// </summary>
        public enum JumperMode
        {
            /// <summary>
            /// Пропуск
            /// </summary>
            Skip,
            /// <summary>
            /// Обеззвучивание
            /// </summary>
            Mute,
            /// <summary>
            /// Повышение громкости
            /// </summary>
            IncreaseVolume,
            /// <summary>
            /// Понижение громкости
            /// </summary>
            LowerVolume
        }

        public static bool IsEditDefaultAddressInfo { get; set; }

       public static Dictionary<string, string> TranslitDictionary = new Dictionary<string, string>
       {
            {"А","A"},
            {"а","a"},
            {"Б","B"},
            {"б","b"},
            {"В","V"},
            {"в","v"},
            {"Г","G"},
            {"г","g"},
            {"Д","D"},
            {"д","d"},
            {"Е","E"},
            {"е","e"},
            {"Ё","Yo"},
            {"ё","yo"},
            {"Ж","Zh"},
            {"ж","zh"},
            {"З","Z"},
            {"з","z"},
            {"И","I"},
            {"и","i"},
            {"Й","Y"},
            {"й","y"},
            {"К","K"},
            {"к","k"},
            {"Л","L"},
            {"л","l"},
            {"М","M"},
            {"м","m"},
            {"Н","N"},
            {"н","n"},
            {"О","O"},
            {"о","o"},
            {"П","P"},
            {"п","p"},
            {"Р","R"},
            {"р","r"},
            {"С","S"},
            {"с","s"},
            {"Т","T"},
            {"т","t"},
            {"У","U"},
            {"у","u"},
            {"Ф","F"},
            {"ф","f"},
            {"Х","H"},
            {"х","h"},
            {"Ц","Ts"},
            {"ц","ts"},
            {"Ч","Ch"},
            {"ч","ch"},
            {"Ш","Sh"},
            {"ш","sh"},
            {"Щ","Sch"},
            {"щ","sch"},
            {"ъ","`"},
            {"Ы","I"},
            {"ы","i"},
            {"ь","'"},
            {"Э","E"},
            {"э","e"},
            {"Ю","Yu"},
            {"ю","yu"},
            {"Я","Ya"},
            {"я","ya"},
            {" ", "_" }
        };
    }
}

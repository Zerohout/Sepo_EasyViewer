using System;

namespace EasyViewer.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
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

        public const string NewFilmName = "Новый_фильм";


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
        public static readonly string ErrorLogName = $"{DateTime.Today:d}_{DateTime.Now.Hour:00}.{DateTime.Now.Minute:00}.{DateTime.Now.Second:00}.txt";

        /// <summary>
        /// Менеджер окон
        /// </summary>
        public static WindowManager WinMan = new WindowManager();

        /// <summary>
        /// Главный таймер
        /// </summary>
        public static Timer MainTimer = new Timer();

        public static IntPtr HWND;
        /// <summary>
        /// Регистратор горячий клавиш
        /// </summary>
        public static HotkeysRegistrator HotReg;

        public static VlcControl Vlc;

        public static VlcMediaPlayer VlcPlayer => Vlc?.SourceProvider.MediaPlayer;

        public static AppValues AppVal = new AppValues();

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
            SAVE_CHANGES,
            /// <summary>
            /// Отмена изменений
            /// </summary>
            CANCEL_CHANGES,
            /// <summary>
            /// Удаление объекта
            /// </summary>
            REMOVE_OBJECT,
            /// <summary>
            /// Перезапись файла
            /// </summary>
            OVERWRITE_FILE,
            /// <summary>
            /// Вопрос пользователю
            /// </summary>
            QUESTION,
            /// <summary>
            /// Информационное окно
            /// </summary>
            INFO,
            /// <summary>
            /// Обработка ошибки
            /// </summary>
            ERROR
        }

        /// <summary>
        /// Результат диалогового окна
        /// </summary>
        public enum DialogResult
        {
            /// <summary>
            /// Положительный результат
            /// </summary>
            YES_ACTION,
            /// <summary>
            /// Отрицательный результат
            /// </summary>
            NO_ACTION,
            /// <summary>
            /// Результат отмены
            /// </summary>
            CANCEL_ACTION
        }

        /// <summary>
        /// Режим работы видеоплеера
        /// </summary>
        public enum VideoPlayerMode
        {
            /// <summary>
            /// Проигрывание
            /// </summary>
            Viewing,
            /// <summary>
            /// Предпросмотр
            /// </summary>
            Preview
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

        public enum SPVoices
        {
            MTV,
            Paramount,
            Original,
            Goblin,

        }

       public static Dictionary<string, string> TranslitDictionary = new Dictionary<string, string>
        {
            {"а","a"},
            {"б","b"},
            {"в","v"},
            {"г","g"},
            {"д","d"},
            {"е","e"},
            {"ё","yo"},
            {"ж","zh"},
            {"з","z"},
            {"и","i"},
            {"й","y"},
            {"к","k"},
            {"л","l"},
            {"м","m"},
            {"н","n"},
            {"о","o"},
            {"п","p"},
            {"р","r"},
            {"с","s"},
            {"т","t"},
            {"у","u"},
            {"ф","f"},
            {"х","h"},
            {"ц","ts"},
            {"ч","ch"},
            {"ш","sh"},
            {"щ","sch"},
            {"ъ","`"},
            {"ы","i"},
            {"ь","'"},
            {"э","e"},
            {"ю","yu"},
            {"я","ya"},
            {" ", "_" }
        };
    }
}

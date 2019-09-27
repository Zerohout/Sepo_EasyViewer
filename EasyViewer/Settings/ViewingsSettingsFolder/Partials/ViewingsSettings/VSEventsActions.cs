﻿// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.ViewingsSettingsFolder.ViewModels
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;

    public partial class ViewingsSettingsViewModel : Screen
    {

        public void KeyDown(KeyEventArgs e)
        {
            switch (e.KeyboardDevice.Modifiers)
            {
                case ModifierKeys.Control:
                    switch (e.Key)
                    {
                        case Key.OemPlus:
                            if (Episodes.Count > 0)
                            {
                                if (EpisodeIndexes.CurrentIndex < EpisodeIndexes.EndIndex)
                                {
                                    SelectedEpisode = Episodes[EpisodeIndexes.CurrentIndex + 1];
                                    //return;
                                }
                            }
                            break;
                        case Key.OemMinus:
                            if (Episodes.Count > 0)
                            {
                                if (EpisodeIndexes.CurrentIndex > 0)
                                {
                                    SelectedEpisode = Episodes[EpisodeIndexes.CurrentIndex - 1];
                                    //return;
                                }
                            }

                            break;
                    }

                    break;
                case ModifierKeys.None:
                    break;
            }
        }


        public void SeasonCheckValidation()
        {
            //CvDbContext.ChangeTracker.DetectChanges();

            //if(CvDbContext.ChangeTracker.HasChanges())
            //{
            //	CvDbContext.SaveChanges();

            //}
        }

        public void EpisodeCheckValidation()
        {
            //CvDbContext.ChangeTracker.DetectChanges();

            //if(CvDbContext.ChangeTracker.HasChanges())
            //{
            //	CvDbContext.SaveChanges();

            //}
        }

        public void VoiceOverCheck()
        {
            //CvDbContext.ChangeTracker.DetectChanges();

            //if (CvDbContext.ChangeTracker.HasChanges())
            //{
            //	CvDbContext.SaveChanges();
            //}

        }

        public void VoiceOverUncheck()
        {

        }

        #region Selection actions

        public void FilmSelectionChanged()
        {
        }

        public void SeasonSelectionChanged(ListBox lb)
        {
            lb.ScrollIntoView(lb.SelectedItem);
        }

        private (int CurrentIndex, int EndIndex) EpisodeIndexes;

        public void EpisodeSelectionChanged(ListBox lb)
        {
            lb.ScrollIntoView(lb.SelectedItem);
        }

        public void VoiceOverSelectionChanged(ListBox lb)
        {
            lb.ScrollIntoView(lb.SelectedItem);
        }

        #endregion


        #region Cancel selection buttons
        /// <summary>
        /// Снять выделение с выбранного фильма
        /// </summary>
        public void CancelFilmSelection() => SelectedFilm = null;
        public bool CanCancelFilmSelection => SelectedFilm != null;

        /// <summary>
        /// Снять выделение с выбранного сезона
        /// </summary>
        public void CancelSeasonSelection() => SelectedSeason = null;
        public bool CanCancelSeasonSelection => SelectedSeason != null;

        /// <summary>
        /// Снять выделение с выбранного эпизода
        /// </summary>
        public void CancelEpisodeSelection() => SelectedEpisode = null;
        public bool CanCancelEpisodeSelection => SelectedEpisode != null;

        ///// <summary>
        ///// Снять выделение с выбранной озвучки
        ///// </summary>
        //public void CancelVoiceOverSelection() => SelectedVoiceOver = null;
        //public bool CanCancelVoiceOverSelection => SelectedVoiceOver != null;

        #endregion

    }
}

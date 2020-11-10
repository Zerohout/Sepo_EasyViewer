namespace EasyViewer.Helpers.Creators.SouthPark
{
    using System;

    public static class SPOnline
    {
        private const string kub = "kubik";
        private const string parSPOnline = "paramount";
        private const string mtv = "mtv";
        private const string gob = "goblin";
        private const string jaskier = "jaskier";

        /// <summary>
        /// Получить адрес эпизода с сайта SPOnline
        /// </summary>
        /// <param name="seasonNum">Номер сезона</param>
        /// <param name="episodeNum">Номер эпизода</param>
        /// <returns></returns>
        public static Uri GetSPEpAddressSPOnline(int seasonNum, int episodeNum)
        {
            var ip = GetSPSeasonIpSPOnline(seasonNum);
            var voice = GetSpVoiceNameSpOnline(seasonNum, episodeNum);

            return new Uri(
                $"http://{ip}/video/spark/s{seasonNum:00}-{voice}/{GetSpAddressNum(seasonNum, episodeNum, voice)}.mp4");
        }

        public static string GetSpAddressNum(int seasonNum, int episodeNum, string voice)
        {
            switch (voice)
            {
                case mtv:
                    switch (seasonNum)
                    {
                        case 9:
                            return $"{seasonNum}{episodeNum:00}";
                        default:
                            return $"{episodeNum:00}";
                    }
                case parSPOnline:
                    switch (seasonNum)
                    {
                        case 15:
                            return $"{seasonNum}{episodeNum:00}";
                        default:
                            return $"{episodeNum:00}";
                    }
                case kub:
                    switch (seasonNum)
                    {
                        case 15:
                        case 17:
                            return $"{seasonNum}{episodeNum:00}";
                        default:
                            return $"{episodeNum:00}";
                    }
                default:
                    switch (seasonNum)
                    {
                        case 23:
                            return $"{episodeNum:00}";

                        default: return "00";
                    }
            }
        }


        public static string GetSpVoiceNameSpOnline(int seasonNum, int episodeNum)
        {
            switch (seasonNum)
            {
                case 1:
                case 10:
                    return parSPOnline;
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 20:
                case 21:
                case 22:
                    return kub;
                case 19:
                    switch (episodeNum)
                    {
                        case 8:
                        case 9:
                        case 10:
                            return parSPOnline;
                        default:
                            return kub;
                    }
                case 23:
                    switch (episodeNum)
                    {
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                            return jaskier;
                        default:
                            return kub;
                    }
                case 24:
                    return jaskier;
                default:
                    return mtv;
            }
        }

        /// <summary>
        /// Получить название озвучки сезона фильма Южный Парк
        /// </summary>
        /// <param name="seasonNum">Номер сезона</param>
        /// <param name="voiceNum">Номер озвучки</param>
        /// <returns></returns>
        public static string GetSPEpVoiceNameSPOnline(int seasonNum, int voiceNum = 1)
        {
            switch (seasonNum)
            {
                case 1:
                    switch (voiceNum)
                    {
                        case 1:
                            return parSPOnline;
                        default:
                            return gob;
                    }
                case 2:
                    switch (voiceNum)
                    {
                        case 1:
                            return mtv;
                        default:
                            return gob;
                    }
                case 14:
                    switch (voiceNum)
                    {
                        case 1:
                            return kub;
                        default:
                            return mtv;
                    }
                case 10:
                    return parSPOnline;
                case 19:
                    switch (voiceNum)
                    {
                        case 1:
                            return parSPOnline;
                        default:
                            return kub;
                    }
                case 15:
                case 16:
                case 17:
                case 18:
                case 20:
                case 21:
                case 22:
                    switch (voiceNum)
                    {
                        case 1:
                            return kub;
                        default:
                            return parSPOnline;
                    }
                case 23:
                    return jaskier;
                default:
                    return mtv;
            }
        }

        /// <summary>
        /// Получить номер эпизода фильма Южный Парк для адреса
        /// </summary>
        /// <param name="seasonNum">Номер сезона</param>
        /// <param name="episodeNum">Номер эпизода</param>
        /// <param name="voice">Название озвучки</param>
        /// <returns></returns>
        private static string GetSPEpAddressNumSPOnline(int seasonNum, int episodeNum, string voice)
        {
            switch (voice)
            {
                case gob:
                    switch (seasonNum)
                    {
                        case 1:
                            return $"{seasonNum}{episodeNum:00}";
                        default:
                            return $"{episodeNum:00}";
                    }
                case mtv:
                    switch (seasonNum)
                    {
                        case 9:
                            return $"{seasonNum}{episodeNum:00}";
                        case 14:
                            switch (episodeNum)
                            {
                                case 5:
                                case 6:
                                    return null;
                                default:
                                    return $"{episodeNum:00}";
                            }
                        default:
                            return $"{episodeNum:00}";
                    }
                case parSPOnline:
                    switch (seasonNum)
                    {
                        case 15:
                            switch (episodeNum)
                            {
                                case 3:
                                    return null;
                                default:
                                    return $"{seasonNum}{episodeNum:00}";
                            }
                        case 22:
                            switch (episodeNum)
                            {
                                case 4:
                                    return null;
                                default:
                                    return $"{episodeNum:00}";
                            }
                        default:
                            return $"{episodeNum:00}";
                    }
                case kub:
                    switch (seasonNum)
                    {
                        case 15:
                            switch (episodeNum)
                            {
                                case 3:
                                    return "";
                                default:
                                    return $"{seasonNum}{episodeNum:00}";
                            }
                        case 17:
                            return $"{seasonNum}{episodeNum:00}";
                        default:
                            return $"{episodeNum:00}";
                    }
                case jaskier:
                    switch (seasonNum)
                    {
                        case 23:
                            switch (episodeNum)
                            {
                                default:
                                    return $"{episodeNum:00}";
                            }
                        default: return "";
                    }
                default:
                    return "";
            }
        }

        /// <summary>
        /// Получить количество озвучек эпизода фильма Южный Парк
        /// </summary>
        /// <param name="seasonNum">Номер сезона</param>
        /// <param name="episodeNum">Номер эпизода</param>
        /// <returns></returns>
        public static int GetSPEpVoicesCountSPOnline(int seasonNum, int episodeNum)
        {
            switch (seasonNum)
            {
                case 1:
                case 2:
                case 16:
                case 17:
                case 18:
                case 20:
                case 21:
                    return 2;
                case 14:
                    switch (episodeNum)
                    {
                        case 5:
                        case 6:
                            return 1;
                        default:
                            return 2;
                    }
                case 15:
                    switch (episodeNum)
                    {
                        case 3:
                            return 0;
                        default:
                            return 2;
                    }
                case 19:
                    switch (episodeNum)
                    {
                        case 8:
                        case 9:
                        case 10:
                            return 1;
                        default:
                            return 2;
                    }
                case 22:
                    switch (episodeNum)
                    {
                        case 4:
                        case 10:
                            return 1;
                        default:
                            return 2;
                    }

                default:
                    return 1;
            }
        }

        /// <summary>
        /// Получить ip-адрес сезона фильма Южный Парк
        /// </summary>
        /// <param name="seasonNum">Номер сезона</param>
        /// <returns></returns>
        private static string GetSPSeasonIpSPOnline(int seasonNum)
        {
            switch (seasonNum)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 20:
                case 21:
                case 22:
                    return "213.32.1.33";
                case 23:
                case 24:
                    return "client394.gcdn.uk";
                default:
                    return "89.163.225.137";
            }
        }
    }
}
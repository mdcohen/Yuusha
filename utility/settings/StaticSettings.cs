using System;

namespace Yuusha.Utility.Settings
{
    /// <summary>
    /// These settings are only changed in code.
    /// </summary>
    public static class StaticSettings
    {
        public static string ServerVersion = "0.0.0";
        public static string ClientName = "Yuusha"; // 6/16/2018 conversion to MonoGame
        public static string ClientVersion = "1.7"; // 1/20/2019
        public static bool RoundDelayEnabled = false;
        public static double RoundDelayLength = 5000;
        public static bool IgnoreHeightDrawText = true; // BitmapFont class draws text anyway even if rectangle isn't large enough
    }
}

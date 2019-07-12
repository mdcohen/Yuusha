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
        public static string ClientVersionFromServer = ""; // received in IO
        public static bool RoundDelayEnabled = false;
        public static double RoundDelayLength = 3000;
        public static bool IgnoreHeightDrawText = false; // BitmapFont class draws text anyway even if rectangle isn't large enough

        public static string DecryptionPassPhrase1 = "d9KqgmVHBG4oipcquzdc"; // account names
        public static string DecryptionPassPhrase2 = "5ZUS0gDgHnNjmFfO15b4"; // account passwords
        public static string DecryptionPassPhrase3 = "wVsbE4k1oxlHfqLI2oL0"; // most recent used account name

        public static bool DisplayMouseCursorCoordinates = false;
    }
}

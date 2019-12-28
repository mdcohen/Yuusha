using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.Utility.Settings
{
    /// <summary>
    /// User settings are unique for each account.
    /// </summary>
    public class UserSettings
    {
        public bool AudioEnabled = true;
        public bool SoundEffects = true; // master setting for sound
        public bool BackgroundAmbience = true; // currently ambience and music since they are both controlled by MediaPlayer
        public bool FullScreen = false;
        public string DefaultGameDisplayMode = "Yuusha";
        public bool AutoDisplayNews = true;
        public bool NewRoundNotification = true;
        public bool HideMouseCursor = true; // hide mouse cursor when not in use for x seconds
        public bool TextColorFiltering = true; // for scrollable text boxes, display specified colors when text matches filters
        public bool TextSoundCues = false; // play sounds when specific text is detected
        public bool DisplayDamageFog = true;

        public bool AgreedEULA = false; // saved server side when someone agrees to EULA then logs in

        //public Dictionary<string, Color> DisplayTextColors = new Dictionary<string, Color>();

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        public void Save()
        {
            try
            {
                if (Account.Name.Length > 0)
                {
                    string fileName = Utils.AccountFileName;
                    string dirName = Utils.StartupPath + Utils.AccountsFolder + Account.Name + "\\";

                    Stream stream = File.Create(dirName + fileName);

                    XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
                    serializer.Serialize(stream, this);
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        public void OnLoad()
        {
            Client.GameDisplayMode = (Enums.EGameDisplayMode)Enum.Parse(typeof(Enums.EGameDisplayMode), DefaultGameDisplayMode, true);
        }

        /// <summary>
        /// Loads settings from a file.
        /// </summary>
        public static UserSettings Load()
        {
            try
            {
                string fileName = Utils.AccountFileName;
                string dirName = Utils.StartupPath + Utils.AccountsFolder + Account.Name + "\\";

                UserSettings settings = new UserSettings();

                if (Account.Name == "" || !File.Exists(dirName + fileName))
                {
                    settings.OnLoad();
                    return settings;
                }

                Stream stream = File.OpenRead(dirName + fileName);
                XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
                settings = (UserSettings)serializer.Deserialize(stream);
                settings.OnLoad();
                stream.Close();
                return settings;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return new UserSettings();
            }
        }
    }
}

using System;
using System.Xml.Serialization;
using System.IO;
using Color = Microsoft.Xna.Framework.Color;

namespace Yuusha.Utility.Settings
{
    public class ClientSettings
    {
        public string ServerName = "Dragon's Spine";
        public string ServerHost = "www.dragonsspine.com";
        public int ServerPort = 3000;
        public bool ShowSplash = false;
        public string DefaultFont = "courier16";
        public string DefaultPopUpColor = "Cyan";
        public string DefaultDropDownMenuFont = "courier12";
        public bool DisplayChantingTextCue = true;
        public bool StartFullScreen = false;
        public bool PlayAudioWhenClientDeactivated = true;

        // Window positions.
        public int OptionsWindowX = 50;
        public int OptionsWindowY = 50;
        public int MacrosWindowX = 300;
        public int MacrosWindowY = 60;
        public int NewsWindowX = 500;
        public int NewsWindowY = 50;

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        public void Save()
        {
            try
            {
                if (Account.Name.Length > 0)
                {
                    string fileName = Utils.SettingsFileName;
                    string dirName = Utils.StartupPath;

                    Stream stream = File.Create(dirName + fileName);

                    XmlSerializer serializer = new XmlSerializer(typeof(ClientSettings));
                    serializer.Serialize(stream, this);
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        /// <summary>
        /// Loads settings from a file.
        /// </summary>
        public static ClientSettings Load()
        {
            try
            {
                string fileName = Utils.SettingsFileName;
                string dirName = Utils.StartupPath;

                if (!File.Exists(dirName + fileName))
                {
                    return new ClientSettings();
                }

                Stream stream = File.OpenRead(dirName + fileName);
                XmlSerializer serializer = new XmlSerializer(typeof(ClientSettings));
                ClientSettings settings = (ClientSettings)serializer.Deserialize(stream);
                stream.Close();
                return settings;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return new ClientSettings();
            }
        }
    }
}

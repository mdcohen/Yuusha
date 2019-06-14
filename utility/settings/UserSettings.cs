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
        public bool SoundEffects = true; // master setting for sound
        public bool FullScreen = false;
        public string DefaultGameDisplayMode = "IOK";
        public bool AutoDisplayNews = true;
        public bool NewRoundNotification = true;
        public bool HideMouseCursor = true; // hide mouse cursor when not in use for x seconds
        public bool TextColorFiltering = true; // for scrollable text boxes, display specified colors when text matches filters
        public bool TextSoundCues = false; // play sounds when specific text is detected
        public bool DisplayDamageFog = true;

        #region Color Settings
        public Color Color_Gui_Chaotic_Fore = Color.White;
        public Color Color_Gui_Chaotic_Back = Color.DarkMagenta;
        public Color Color_Gui_Evil_Fore = Color.White;
        public Color Color_Gui_Evil_Back = Color.DarkRed;
        public Color Color_Gui_ChaoticEvil_Fore = Color.Silver;
        public Color Color_Gui_ChaoticEvil_Back = Color.DarkRed;
        public Color Color_Gui_Neutral_Fore = Color.White;
        public Color Color_Gui_Neutral_Back = Color.ForestGreen;
        public Color Color_Gui_Lawful_Fore = Color.White;
        public Color Color_Gui_Lawful_Back = Color.Black;
        public Color Color_Gui_Amoral_Fore = Color.White;
        public Color Color_Gui_Amoral_Back = Color.Black;
        public Color Color_Gui_CreatureLetter_Fore = Color.White;
        public Color Color_Gui_Loot_Fore = Color.Gold;

        public Color Color_Gui_Spell_Warming_TextCue = Color.PaleGoldenrod;

        public Color ColorDropDownMenuBorder = Color.MediumOrchid; // 6/8/2019 not being drawn
        public int DropDownMenuBorderWidth = 1;
        public Color ColorDropDownMenu = Color.Gray;
        public Color ColorDropDownMenuItemBackground = Color.Black;
        public Color ColorDropDownMenuTitleText = Color.LightCyan;
        public Color ColorDropDownMenuItemText = Color.PaleGreen;
        public Color ColorDropDownMenuItemHighlight = Color.White;
        public Color ColorDropDownMenuItemTextDisabled = Color.DarkGray;

        public byte GridBoxWindowVisualKeyAlpha = 255;
        public byte GridBoxWindowBorderAlpha = 255;
        public string GridBoxWindowFont = "courier16";

        public Color GridBoxWindowTintColor = Color.DarkSlateGray;
        public Color GridBoxTitleTextColor = Color.White;
        public Color GridBoxTitleTintColor = Color.DimGray;
        public Color GridBoxTitleCloseBoxTintColor = Color.LightSlateGray;
        public Color GridBoxBorderTintColor = Color.DimGray;

        public Color DragAndDropTextColor = Color.White;
        public Color DragAndDropTextOverColor = Color.White;
        public bool DragAndDropHasTextOverColor = false;
        public Color DragAndDropTintOverColor = Color.White;
        public bool DragAndDropHasTintOverColor = false;

        public int GridBoxTitleCloseBoxDistanceFromRight = 20;
        public int GridBoxTitleCloseBoxDistanceFromTop = 5;
        public int GridBoxTitleCloseBoxWidth = 18;
        public int GridBoxTitleCloseBoxHeight = 18;
        public int GridBoxTitleHeight = 26;
        public int GridBoxBorderWidth = 2;
        public int GridBoxButtonsBorderWidth = 1;

        public Color AcceptingGridBoxBorderColor = Color.PaleGreen;
        public Color AcceptingGridBoxTitleColor = Color.PaleGreen;
        public Color AcceptingGridBoxTitleTextColor = Color.Black;

        public Color PrivateMessageWindowTitleTextColor = Color.White;
        public Color PrivateMessageWindowTitleTintColor = Color.Gray;
        public Color PrivateMessageWindowTintColor = Color.DarkGray;
        public Color PrivateMessageWindowTitleCloseBoxTintColor = Color.LightSlateGray;
        public Color PrivateMessageWindowTitleCropBoxTintColor = Color.LightSlateGray;
        public Color PrivateMessageBorderTintColor = Color.Gray;

        public int PrivateMessageWindowTitleCloseBoxDistanceFromRight = 18;
        public int PrivateMessageWindowTitleCloseBoxDistanceFromTop = 1;
        public int PrivateMessageWindowTitleCloseBoxWidth = 16;
        public int PrivateMessageWindowTitleCloseBoxHeight = 16;

        public int PrivateMessageWindowTitleCropBoxDistanceFromRight = 34;
        public int PrivateMessageWindowTitleCropBoxDistanceFromTop = 2;
        public int PrivateMessageWindowTitleCropBoxWidth = 16;
        public int PrivateMessageWindowTitleCropBoxHeight = 16;

        public int PrivateMessageWindowTitleHeight = 18;
        public int PrivateMessageWindowBorderWidth = 2;

        public Color DefaultMouseCursorTextCueColor = Color.PapayaWhip;

        public Color HotButtonText_ForeColor = Color.Lime;
        public Color HotButtonText_BackColor = Color.Purple;
        #endregion

        public Color TargetBorderColor = Color.WhiteSmoke;
        public int TargetBorderSize = 1;
        public int MapTileBorderSize = 1;
        public Color MapTileBorderColor = Color.Goldenrod;
        public int DefaultWindowTitleHeight = 18;

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
            Client.GameDisplayMode = (Enums.EGameDisplayMode)Enum.Parse(typeof(Enums.EGameDisplayMode), this.DefaultGameDisplayMode, true);
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

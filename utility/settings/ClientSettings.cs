using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using Color = Microsoft.Xna.Framework.Color;

namespace Yuusha.Utility.Settings
{
    [Serializable]
    public class ClientSettings
    {
        public string ServerName = "Dragon's Spine";
        public string ServerHost = "www.dragonsspine.com";
        public int ServerPort = 3000;
        public bool ShowSplash = false;
        public string DefaultFont = "lemon16"; //"courier16";
        public string DefaultHUDNumbersFont = "changaone26";
        public Color ColorDefaultPopUpFore = Color.GhostWhite;
        public Color ColorDefaultPopUpBack = Color.Black;
        public byte DefaultPopUpBackAlpha = 200; // foreground text is always 255
        public byte DefaultPopUpFadeOutSpeed = 1;
        public byte DefaultPopUpFadeInSpeed = 5;
        public string DefaultPopUpFont = "lemon12";
        public string DefaultDropDownMenuFont = "lemon12";
        public string DefaultOnClickSound = "GUISounds/click_short";
        public bool DisplayChantingConversationBubble = true;
        public bool DisplayConversationBubbles = true;
        public int ConversationBubbleFadeOutSpeed = 2;
        public bool FullScreenPreferred = true;

        // audio related
        public bool PlayAudioWhenClientDeactivated = true;
        public bool PlayPrivateMessageSounds = true;
        public bool PlayModemDialOnConnect = true;
        public bool DisplaySoundIndicators = true;
        public bool DisplaySoundIndicatorsNearby = false;
        public int SoundIndicatorDimensions = 40;
        public Color SoundIndicatorTintColor = Color.Azure;
        public Color SoundIndicatorTextColor = Color.Goldenrod;
        public string SoundIndicatorFont = "courier14";

        // private messages specific
        public bool EchoPrivateMessagesToConference = true;
        public bool EchoPrivateMessagesToGame = true;
        public bool DisplayPrivateMessageWindows = true;

        // gui behavior
        public bool DisplaySpellEffectLabels = true;
        public bool DisplaySpellEffectNameOnLabels = false;
        public bool EchoGroundItemsOnExamination = false;
        public bool GroupSimiliarItemsInGridBoxes = true;
        public bool AlwaysOpenGridBoxWindowsUponActivity = false;
        public bool ShowMapDisplayWindowBorderWhenFogOfWarVisible = true; // display map border around main display map when FogOfWar visible
        public double DoubleClickTimerDelay = 500; // the lower the number the less time required to detect a double click
        public bool AllowDoubleClickMovementToNonVisibleCells = true;

        // saving accounts and passwords (encrypted)
        public List<Encrypt.EncryptedKeyValuePair<string, string>> StoredAccounts = new List<Encrypt.EncryptedKeyValuePair<string, string>>();
        public string MostRecentStoredAccount = "";

        // Window positions.
        public int OptionsWindowX = 50;
        public int OptionsWindowY = 50;
        public int MacrosWindowX = 300;
        public int MacrosWindowY = 60;
        public int NewsWindowX = 500;
        public int NewsWindowY = 50;

        #region Critter List Color Settings
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
        #endregion

        //public bool FogOfWar = true;

        public Color MovementFootstepsColor = Color.WhiteSmoke;
        public Color TargetBorderColor = Color.WhiteSmoke;
        public int TargetBorderSize = 1;
        public int MapTileBorderSize = 1;
        public Color MapTileBorderColor = Color.Goldenrod;
        public int DefaultWindowTitleHeight = 21;
        public string DefaultWindowTitleFont = "lemon12";

        public Color DefaultMouseCursorTextCueColor = Color.PapayaWhip;

        public Color HotButtonPopUpText_ForeColor = Color.Lime;
        public Color HotButtonPopUpText_BackColor = Color.Purple;
        public byte HotButtonPopUpText_BackColorAlpha = 255;

        public Color TalentsHotButtonPopUpText_ForeColor = new Color(0, 225, 217);
        public Color TalentsHotButtonPopUpText_BackColor = new Color(94, 0, 31);
        public byte TalentsHotButtonPopUpText_BackColorAlpha = 255;

        public Color GridBoxWindowTintColor = Color.LightSteelBlue;
        public Color GridBoxTitleTextColor = Color.White;
        public Color GridBoxTitleTintColor = Color.DimGray;
        public Color GridBoxTitleCloseBoxTintColor = Color.LightSlateGray;
        public Color GridBoxBorderTintColor = Color.DimGray;

        public int GridBoxTitleCloseBoxDistanceFromRight = 20;
        public int GridBoxTitleCloseBoxDistanceFromTop = 5;
        public int GridBoxTitleCloseBoxWidth = 18;
        public int GridBoxTitleCloseBoxHeight = 18;
        public int GridBoxTitleHeight = 26;
        public int GridBoxBorderWidth = 2;
        public int GridBoxButtonsBorderWidth = 1;

        public Color DragAndDropBorderColor = Color.OldLace;
        public Color DragAndDropTextColor = Color.White;
        public Color DragAndDropTextOverColor = Color.White;
        public bool DragAndDropHasTextOverColor = false;
        public Color DragAndDropTintOverColor = Color.White;
        public bool DragAndDropHasTintOverColor = false;

        public byte GridBoxWindowVisualKeyAlpha = 175;
        public byte GridBoxWindowBorderAlpha = 255;
        public string GridBoxWindowFont = "lemon14";

        public Color AcceptingGridBoxBorderColor = Color.PaleGreen;
        public int AcceptingGridBoxBorderWidth = 2;
        public Color AcceptingGridBoxTitleColor = Color.PaleGreen;
        public Color AcceptingGridBoxTitleTextColor = Color.Black;

        public Color ColorDropDownMenuBorder = Color.MediumOrchid; // 6/8/2019 not being drawn -- 9/8/2019 figured out why it wasn't being drawn
        public int DropDownMenuBorderWidth = 1;
        public Color ColorDropDownMenu = Color.Gray;
        public Color ColorDropDownMenuItemBackground = Color.Transparent;
        public Color ColorDropDownMenuItemDisabledText = Color.LightGray;
        public Color ColorDropDownMenuTitleText = Color.LightCyan;
        public Color ColorDropDownMenuItemText = Color.PaleGreen;
        public Color ColorDropDownMenuSeparator = Color.Black;
        public Color ColorDropDownMenuLabelText = Color.Black;
        public Color ColorDropDownMenuItemHighlight = Color.White;
        public Color ColorDropDownMenuItemTextDisabled = Color.DarkGray;

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

        public bool ContainsStoredAccount(string key, out Encrypt.EncryptedKeyValuePair<string, string> kvPair)
        {
            foreach (Encrypt.EncryptedKeyValuePair<string, string> kvPair2 in StoredAccounts)
                if (kvPair2.Key == key)
                {
                    kvPair = kvPair2;
                    return true;
                }

            kvPair = new Encrypt.EncryptedKeyValuePair<string, string>("", "");
            return false;
        }
    }
}

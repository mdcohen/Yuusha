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
        public bool SoundEffects = true;        
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

        public Color ColorTargetBorder = Color.WhiteSmoke;
        public Color ColorDropDownMenuBorder = Color.White;
        public Color ColorDropDownMenu = Color.Gray;
        public Color ColorDropDownMenuItemBackground = Color.Gray;
        public Color ColorDropDownMenuTitleText = Color.LightCyan;
        public Color ColorDropDownMenuItemText = Color.White;
        public Color ColorDropDownMenuItemHighlight = Color.LightGray;

        public Color ColorHotButtonText_Fore = Color.Lime;
        public Color ColorHotButtonText_Back = Color.Purple;

        // TextTypes
        //Default,
        //PlayerChat,
        //Enter,
        //Exit,
        //Header,
        //Status,
        //System,
        //Help,
        //Private,
        //Listing,
        //Error,
        //Friend,
        //Page,
        //Attuned,
        //NPCChat,
        //Death,
        //SpellCast,
        //SpellWarm,
        //CombatHit,
        //CombatMiss,
        //SpellHit,
        //SpellMiss
        public Color ColorTextType_Default = Color.White;
        public Color ColorTextType_PlayerChat = Color.MediumSlateBlue;
        public Color ColorTextType_Enter = Color.Lime;
        public Color ColorTextType_Exit = Color.LimeGreen;
        public Color ColorTextType_Header = Color.White;
        public Color ColorTextType_Status = Color.White;
        public Color ColorTextType_System = Color.White;
        public Color ColorTextType_Help = Color.White;
        public Color ColorTextType_Private = Color.Aquamarine;
        public Color ColorTextType_Listing = Color.White;
        public Color ColorTextType_Error = Color.White;
        public Color ColorTextType_Friend = Color.White;
        public Color ColorTextType_Page = Color.White;
        public Color ColorTextType_Attuned = Color.White;
        public Color ColorTextType_NPCChat = Color.White;
        public Color ColorTextType_Death = Color.White;
        public Color ColorTextType_SpellCast = Color.White;
        public Color ColorTextType_SpellWarm = Color.White;
        public Color ColorTextType_CombatHit = Color.White;
        public Color ColorTextType_CombatMiss = Color.White;
        public Color ColorTextType_SpellHit = Color.White;
        public Color ColorTextType_SpellMiss = Color.White;
        #endregion

        public int TargetBorderSize = 1;

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

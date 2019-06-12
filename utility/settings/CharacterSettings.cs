using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Yuusha.Utility.Settings
{
    public class CharacterSettings
    {
        #region Options Window Public Variables
        public string DoubleLeftClickNearbyTarget = "fight %t;bash %t";
        public string DoubleLeftClickDistantTarget = "belt shield;throw hammer at %t";
        // Keys used if NumLock is enabled.
        public string NumLock0 = "search all;scoop coins";
        public string NumLock1 = "sw";
        public string NumLock2 = "s";
        public string NumLock3 = "se";
        public string NumLock4 = "w";
        public string NumLock5 = ";";
        public string NumLock6 = "e";
        public string NumLock7 = "nw";
        public string NumLock8 = "n";
        public string NumLock9 = "ne";
        public string NumPadDivide = "search all;scoop all gems";
        public string NumPadMultiply = "belt left;remove ring";
        public string NumPadSubtract = "belt left;remove 2 ring from right";
        public string NumPadAdd = "look in sack;quaff";
        public string NumPadDelete = "ashtug ninda anghizidda arrflug";

        public string FunctionKey1 = "";
        public string FunctionKey2 = "";
        public string FunctionKey3 = "";
        public string FunctionKey4 = "";
        public string FunctionKey5 = "";
        public string FunctionKey6 = "";
        public string FunctionKey7 = "";
        public string FunctionKey8 = "";
        public string FunctionKey9 = "";
        public string FunctionKey10 = "";
        public string FunctionKey11 = "";
        public string FunctionKey12 = "";

        public string CritterListDropDownMenuItem1 = "fight %t;look at %t";
        public string CritterListDropDownMenuItem2 = "cast %t;look at %t";
        public string CritterListDropDownMenuItem3 = "look closely at %t";
        public string CritterListDropDownMenuItem4 = "jumpkick %t;look at %t";
        public string CritterListDropDownMenuItem5 = "shoot %t;look at %t";
        #endregion

        #region Vertical HotButtons
        // TODO Make these a list and add serialization/deserialization of List<T> available.
        public string VerticalHotButtonVisualKey0 = "WhiteSpace";
        public string VerticalHotButtonVisualKey1 = "WhiteSpace";
        public string VerticalHotButtonVisualKey2 = "WhiteSpace";
        public string VerticalHotButtonVisualKey3 = "WhiteSpace";
        public string VerticalHotButtonVisualKey4 = "WhiteSpace";
        public string VerticalHotButtonVisualKey5 = "WhiteSpace";
        public string VerticalHotButtonVisualKey6 = "WhiteSpace";
        public string VerticalHotButtonVisualKey7 = "WhiteSpace";
        public string VerticalHotButtonVisualKey8 = "WhiteSpace";
        public string VerticalHotButtonVisualKey9 = "WhiteSpace";
        public string VerticalHotButtonVisualKey10 = "WhiteSpace";
        public string VerticalHotButtonVisualKey11 = "WhiteSpace";
        public string VerticalHotButtonVisualKey12 = "WhiteSpace";
        public string VerticalHotButtonVisualKey13 = "WhiteSpace";
        public string VerticalHotButtonVisualKey14 = "WhiteSpace";
        public string VerticalHotButtonVisualKey15 = "WhiteSpace";
        public string VerticalHotButtonVisualKey16 = "WhiteSpace";
        public string VerticalHotButtonVisualKey17 = "WhiteSpace";
        public string VerticalHotButtonVisualKey18 = "WhiteSpace";
        public string VerticalHotButtonVisualKey19 = "WhiteSpace";

        public string VerticalHotButtonText0 = "";
        public string VerticalHotButtonText1 = "";
        public string VerticalHotButtonText2 = "";
        public string VerticalHotButtonText3 = "";
        public string VerticalHotButtonText4 = "";
        public string VerticalHotButtonText5 = "";
        public string VerticalHotButtonText6 = "";
        public string VerticalHotButtonText7 = "";
        public string VerticalHotButtonText8 = "";
        public string VerticalHotButtonText9 = "";
        public string VerticalHotButtonText10 = "";
        public string VerticalHotButtonText11 = "";
        public string VerticalHotButtonText12 = "";
        public string VerticalHotButtonText13 = "";
        public string VerticalHotButtonText14 = "";
        public string VerticalHotButtonText15 = "";
        public string VerticalHotButtonText16 = "";
        public string VerticalHotButtonText17 = "";
        public string VerticalHotButtonText18 = "";
        public string VerticalHotButtonText19 = "";

        public string VerticalHotButtonCommand0 = "";
        public string VerticalHotButtonCommand1 = "";
        public string VerticalHotButtonCommand2 = "";
        public string VerticalHotButtonCommand3 = "";
        public string VerticalHotButtonCommand4 = "";
        public string VerticalHotButtonCommand5 = "";
        public string VerticalHotButtonCommand6 = "";
        public string VerticalHotButtonCommand7 = "";
        public string VerticalHotButtonCommand8 = "";
        public string VerticalHotButtonCommand9 = "";
        public string VerticalHotButtonCommand10 = "";
        public string VerticalHotButtonCommand11 = "";
        public string VerticalHotButtonCommand12 = "";
        public string VerticalHotButtonCommand13 = "";
        public string VerticalHotButtonCommand14 = "";
        public string VerticalHotButtonCommand15 = "";
        public string VerticalHotButtonCommand16 = "";
        public string VerticalHotButtonCommand17 = "";
        public string VerticalHotButtonCommand18 = "";
        public string VerticalHotButtonCommand19 = "";
        #endregion

        #region Horizontal HotButtons
        // TODO Make these a list and add serialization/deserialization of List<T> available.
        public string HorizontalHotButtonVisualKey0 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey1 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey2 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey3 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey4 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey5 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey6 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey7 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey8 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey9 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey10 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey11 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey12 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey13 = "WhiteSpace";
        public string HorizontalHotButtonVisualKey14 = "WhiteSpace";

        public string HorizontalHotButtonText0 = "";
        public string HorizontalHotButtonText1 = "";
        public string HorizontalHotButtonText2 = "";
        public string HorizontalHotButtonText3 = "";
        public string HorizontalHotButtonText4 = "";
        public string HorizontalHotButtonText5 = "";
        public string HorizontalHotButtonText6 = "";
        public string HorizontalHotButtonText7 = "";
        public string HorizontalHotButtonText8 = "";
        public string HorizontalHotButtonText9 = "";
        public string HorizontalHotButtonText10 = "";
        public string HorizontalHotButtonText11 = "";
        public string HorizontalHotButtonText12 = "";
        public string HorizontalHotButtonText13 = "";
        public string HorizontalHotButtonText14 = "";

        public string HorizontalHotButtonCommand0 = "";
        public string HorizontalHotButtonCommand1 = "";
        public string HorizontalHotButtonCommand2 = "";
        public string HorizontalHotButtonCommand3 = "";
        public string HorizontalHotButtonCommand4 = "";
        public string HorizontalHotButtonCommand5 = "";
        public string HorizontalHotButtonCommand6 = "";
        public string HorizontalHotButtonCommand7 = "";
        public string HorizontalHotButtonCommand8 = "";
        public string HorizontalHotButtonCommand9 = "";
        public string HorizontalHotButtonCommand10 = "";
        public string HorizontalHotButtonCommand11 = "";
        public string HorizontalHotButtonCommand12 = "";
        public string HorizontalHotButtonCommand13 = "";
        public string HorizontalHotButtonCommand14 = "";
        #endregion

        // Window positions.
        public int HorizontalHotButtonWindowX = 189;
        public int HorizontalHotButtonWindowY = 708;
        public int VerticalHotButtonWindowX = 880;
        public int VerticalHotButtonWindowY = 22;

        #region Load/Save code
        /// <summary>
        /// Saves the current character settings.
        /// </summary>
        /// <param name="filename">The filename to save to</param>
        public void Save()
        {
            if (Character.CurrentCharacter == null)
                return;

            try
            {
                string fileName = Utils.GetCharacterFileName(Character.CurrentCharacter.Name);
                string dirName = Utils.StartupPath + Utils.AccountsFolder + Account.Name + "\\";

                Stream stream = File.Create(dirName + fileName);

                XmlSerializer serializer = new XmlSerializer(typeof(CharacterSettings));
                serializer.Serialize(stream, this);
                stream.Close();
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        /// <summary>
        /// Loads settings from a file.
        /// </summary>
        /// <param name="filename">The filename to load</param>
        public static CharacterSettings Load()
        {
            if (Character.CurrentCharacter == null)
                return new CharacterSettings();

            try
            {
                string fileName = Utils.GetCharacterFileName(Character.CurrentCharacter.Name);
                string dirName = Utils.StartupPath + Utils.AccountsFolder + Account.Name + "\\";

                if (!File.Exists(dirName + fileName))
                {
                    return new CharacterSettings();
                }

                Stream stream = File.OpenRead(dirName + fileName);
                XmlSerializer serializer = new XmlSerializer(typeof(CharacterSettings));
                CharacterSettings settings = (CharacterSettings)serializer.Deserialize(stream);
                stream.Close();
                return settings;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return new CharacterSettings();
            }
        }
        #endregion
    }
}

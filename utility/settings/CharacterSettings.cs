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

        public string FunctionKey1 = "test";
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

        public string CritterListDropDownMenuItem1 = "kick %t;look at %t";
        public string CritterListDropDownMenuItem2 = "backstab %t;look at %t";
        public string CritterListDropDownMenuItem3 = "steal coins from %t";
        public string CritterListDropDownMenuItem4 = "jumpkick %t;look at %t";
        public string CritterListDropDownMenuItem5 = "shoot %t;look at %t";
        #endregion

        #region Vertical HotButtons
        // TODO Make these a list and add serialization/deserialization of List<T> available.
        public string VerticalHotButtonVisualKey0 = "hotbuttonicon_0";
        public string VerticalHotButtonVisualKey1 = "hotbuttonicon_1";
        public string VerticalHotButtonVisualKey2 = "hotbuttonicon_2";
        public string VerticalHotButtonVisualKey3 = "hotbuttonicon_3";
        public string VerticalHotButtonVisualKey4 = "hotbuttonicon_4";
        public string VerticalHotButtonVisualKey5 = "hotbuttonicon_5";
        public string VerticalHotButtonVisualKey6 = "hotbuttonicon_6";
        public string VerticalHotButtonVisualKey7 = "hotbuttonicon_7";
        public string VerticalHotButtonVisualKey8 = "hotbuttonicon_8";
        public string VerticalHotButtonVisualKey9 = "hotbuttonicon_9";
        public string VerticalHotButtonVisualKey10 = "hotbuttonicon_10";
        public string VerticalHotButtonVisualKey11 = "hotbuttonicon_11";
        public string VerticalHotButtonVisualKey12 = "hotbuttonicon_12";
        public string VerticalHotButtonVisualKey13 = "hotbuttonicon_13";
        public string VerticalHotButtonVisualKey14 = "hotbuttonicon_14";
        public string VerticalHotButtonVisualKey15 = "hotbuttonicon_15";
        public string VerticalHotButtonVisualKey16 = "hotbuttonicon_16";
        public string VerticalHotButtonVisualKey17 = "hotbuttonicon_17";
        public string VerticalHotButtonVisualKey18 = "hotbuttonicon_18";
        public string VerticalHotButtonVisualKey19 = "hotbuttonicon_19";

        public string VerticalHotButtonText0 = "$0";
        public string VerticalHotButtonText1 = "$1";
        public string VerticalHotButtonText2 = "$2";
        public string VerticalHotButtonText3 = "$3";
        public string VerticalHotButtonText4 = "$4";
        public string VerticalHotButtonText5 = "$5";
        public string VerticalHotButtonText6 = "$6";
        public string VerticalHotButtonText7 = "$7";
        public string VerticalHotButtonText8 = "$8";
        public string VerticalHotButtonText9 = "$9";
        public string VerticalHotButtonText10 = "$10";
        public string VerticalHotButtonText11 = "$11";
        public string VerticalHotButtonText12 = "$12";
        public string VerticalHotButtonText13 = "$13";
        public string VerticalHotButtonText14 = "$14";
        public string VerticalHotButtonText15 = "$15";
        public string VerticalHotButtonText16 = "$16";
        public string VerticalHotButtonText17 = "$17";
        public string VerticalHotButtonText18 = "$18";
        public string VerticalHotButtonText19 = "$19";
        #endregion

        #region Horizontal HotButtons
        // TODO Make these a list and add serialization/deserialization of List<T> available.
        public string HorizontalHotButtonVisualKey0 = "hotbuttonicon_20";
        public string HorizontalHotButtonVisualKey1 = "hotbuttonicon_21";
        public string HorizontalHotButtonVisualKey2 = "hotbuttonicon_22";
        public string HorizontalHotButtonVisualKey3 = "hotbuttonicon_23";
        public string HorizontalHotButtonVisualKey4 = "hotbuttonicon_24";
        public string HorizontalHotButtonVisualKey5 = "hotbuttonicon_25";
        public string HorizontalHotButtonVisualKey6 = "hotbuttonicon_26";
        public string HorizontalHotButtonVisualKey7 = "hotbuttonicon_27";
        public string HorizontalHotButtonVisualKey8 = "hotbuttonicon_28";
        public string HorizontalHotButtonVisualKey9 = "hotbuttonicon_29";
        public string HorizontalHotButtonVisualKey10 = "hotbuttonicon_30";
        public string HorizontalHotButtonVisualKey11 = "hotbuttonicon_31";
        public string HorizontalHotButtonVisualKey12 = "hotbuttonicon_32";
        public string HorizontalHotButtonVisualKey13 = "hotbuttonicon_33";
        public string HorizontalHotButtonVisualKey14 = "hotbuttonicon_34";

        public string HorizontalHotButtonText0 = "$0";
        public string HorizontalHotButtonText1 = "$1";
        public string HorizontalHotButtonText2 = "$2";
        public string HorizontalHotButtonText3 = "$3";
        public string HorizontalHotButtonText4 = "$4";
        public string HorizontalHotButtonText5 = "$5";
        public string HorizontalHotButtonText6 = "$6";
        public string HorizontalHotButtonText7 = "$7";
        public string HorizontalHotButtonText8 = "$8";
        public string HorizontalHotButtonText9 = "$9";
        public string HorizontalHotButtonText10 = "$10";
        public string HorizontalHotButtonText11 = "$11";
        public string HorizontalHotButtonText12 = "$12";
        public string HorizontalHotButtonText13 = "$13";
        public string HorizontalHotButtonText14 = "$14";
        #endregion

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
